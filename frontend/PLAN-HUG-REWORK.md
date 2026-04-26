# Frontend Implementation Plan: Hug Suggest + Accept Rework

Read `../PLAN.md` for the full overview. This document covers frontend-only implementation steps.

## Prerequisites

- Read `../AGENTS.md` for frontend stack details (Vue 3, Bun, shadcn-vue, Tailwind v4, etc.)
- Package manager is **Bun**, not npm/yarn.
- All UI text is hardcoded Russian (no i18n framework).
- shadcn-vue components live in `src/components/ui/` — edit directly when needed.
- Build: `bun run build`, Dev: `bun dev`, Lint: `bun lint`, Format: `bun format`

---

## Step 1: API Client Updates

### 1.1 Update `src/api/client.ts`

**Update `hugsApi`:**

```typescript
hugsApi: {
  // Renamed: was "send", now suggests a hug (returns pending hug)
  suggest: (userId: string) => api.post(`/hugs/${userId}`),

  // New: accept a pending hug suggestion
  accept: (hugId: string) => api.post(`/hugs/${hugId}/accept`),

  // New: decline a pending hug suggestion
  decline: (hugId: string) => api.post(`/hugs/${hugId}/decline`),

  // New: cancel your outgoing pending hug
  cancel: (hugId: string) => api.post(`/hugs/${hugId}/cancel`),

  // New: get inbox (pending incoming suggestions)
  getInbox: () => api.get('/hugs/inbox'),

  // New: get outgoing pending hug (if any)
  getOutgoing: () => api.get('/hugs/outgoing'),

  // New: get inbox count (for badge)
  getInboxCount: () => api.get('/hugs/inbox/count'),

  // Existing (unchanged)
  getCooldown: (userId: string) => api.get(`/hugs/cooldown/${userId}`),
  upgradeCooldown: (userId: string) => api.post(`/hugs/cooldown/${userId}/upgrade`),
  getHistory: () => api.get('/hugs/history'),
  getFeed: (limit = 50) => api.get(`/hugs/feed?limit=${limit}`),
  getActivity: () => api.get('/hugs/activity'),
},
```

---

## Step 2: TypeScript Types

### 2.1 Update `src/stores/hugs.ts` types

**Add new interfaces:**

```typescript
interface PendingHugInboxItem {
  id: string
  giver_id: string
  receiver_id: string
  giver_username: string
  giver_gender?: string
  created_at: string
}

interface OutgoingPendingHug {
  id: string
  giver_id: string
  receiver_id: string
  receiver_username: string
  receiver_gender?: string
  created_at: string
}

// WebSocket typed message wrapper
interface WSMessage {
  type: 'hug_completed' | 'hug_suggestion' | 'hug_declined' | 'hug_cancelled' | 'hug_expired' | 'inbox_count'
  data: any
}
```

---

## Step 3: WebSocket Composable

### 3.1 Create `src/composables/useWebSocket.ts`

Extract WebSocket logic from `FeedView.vue` into a shared composable that connects on app mount:

```typescript
// src/composables/useWebSocket.ts
import { ref, onUnmounted } from 'vue'

const socket = ref<WebSocket | null>(null)
const connected = ref(false)
const listeners = new Map<string, Set<(data: any) => void>>()

export function useWebSocket() {
  function connect() {
    // Get JWT token from localStorage
    const token = localStorage.getItem('token')
    const proto = window.location.protocol === 'https:' ? 'wss:' : 'ws:'
    const url = `${proto}//${window.location.host}/api/v1/ws?token=${token}`

    const ws = new WebSocket(url)

    ws.onopen = () => { connected.value = true }

    ws.onmessage = (event) => {
      const msg: WSMessage = JSON.parse(event.data)
      const handlers = listeners.get(msg.type)
      if (handlers) {
        handlers.forEach(fn => fn(msg.data))
      }
    }

    ws.onclose = () => {
      connected.value = false
      setTimeout(connect, 3000) // auto-reconnect
    }

    ws.onerror = () => ws.close()

    socket.value = ws
  }

  function on(type: string, handler: (data: any) => void) {
    if (!listeners.has(type)) listeners.set(type, new Set())
    listeners.get(type)!.add(handler)

    // Return cleanup function
    return () => listeners.get(type)?.delete(handler)
  }

  function disconnect() {
    socket.value?.close()
    socket.value = null
  }

  return { connected, connect, disconnect, on }
}
```

**Usage in App.vue**: Connect on mount when authenticated. This ensures all pages receive WebSocket events.

**Usage in individual components**: Call `on('hug_suggestion', handler)` to subscribe to specific event types.

---

## Step 4: Pinia Store Updates

### 4.1 Update `src/stores/hugs.ts`

**Add new state:**

```typescript
const inbox = ref<PendingHugInboxItem[]>([])
const outgoingHug = ref<OutgoingPendingHug | null>(null)
const inboxCount = ref(0)
```

**Add new actions:**

```typescript
async function fetchInbox() {
  const { data } = await api.hugsApi.getInbox()
  inbox.value = data
  inboxCount.value = data.length
  return data
}

async function fetchOutgoing() {
  const { data } = await api.hugsApi.getOutgoing()
  outgoingHug.value = data
  return data
}

async function fetchInboxCount() {
  const { data } = await api.hugsApi.getInboxCount()
  inboxCount.value = data.count
  return data.count
}

async function acceptHug(hugId: string) {
  const { data } = await api.hugsApi.accept(hugId)
  // Remove from inbox
  inbox.value = inbox.value.filter(h => h.id !== hugId)
  inboxCount.value = Math.max(0, inboxCount.value - 1)
  // Refresh balance (both users get +1 coin)
  await fetchBalance()
  return data
}

async function declineHug(hugId: string) {
  const { data } = await api.hugsApi.decline(hugId)
  // Remove from inbox
  inbox.value = inbox.value.filter(h => h.id !== hugId)
  inboxCount.value = Math.max(0, inboxCount.value - 1)
  return data
}

async function cancelOutgoing(hugId: string) {
  const { data } = await api.hugsApi.cancel(hugId)
  outgoingHug.value = null
  return data
}
```

**Update `sendHug`** → rename to `suggestHug`:**

```typescript
async function suggestHug(userId: string) {
  const { data } = await api.hugsApi.suggest(userId)
  // The hug is now pending, not completed
  // Set outgoing pending hug
  outgoingHug.value = {
    id: data.id,
    giver_id: data.giver_id,
    receiver_id: data.receiver_id,
    receiver_username: data.receiver_username,
    created_at: data.created_at,
  }
  return data
}
```

---

## Step 5: Component Updates

### 5.1 Update `src/components/HugButton.vue`

**New behavior:**
- Clicking "Обнять" now suggests a hug (pending state)
- If user already has a global pending hug → button disabled: "Ожидание ответа..."
- After suggesting to this specific user → button shows "Ожидание..." until accepted/declined/cancelled
- Toast on suggest: "Ты предложил(а) обнимашку {username}!"
- No explosion on suggest (explosion only on accept)

**Implementation changes:**
- Import `outgoingHug` from the store
- Computed: `hasGlobalPending = !!outgoingHug.value`
- Computed: `isPendingWithThisUser = outgoingHug.value?.receiver_id === userId`
- Button disabled states:
  1. Cooldown active → show timer (existing)
  2. `isPendingWithThisUser` → show "Ожидание..." with a spinner/clock
  3. `hasGlobalPending` (but not this user) → show "Ожидание ответа..." disabled
  4. Ready → show "Обнять" (existing)

**Remove explosion from send flow** — explosion now only happens on accept (in InboxSection).

### 5.2 Create `src/components/InboxSection.vue`

New component for the dashboard showing incoming hug suggestions.

**Template structure:**
```
<Card>
  <CardHeader>
    <CardTitle>Входящие объятия <Badge>{{ count }}</Badge></CardTitle>
  </CardHeader>
  <CardContent>
    <!-- Empty state -->
    <div v-if="!inbox.length">Нет входящих объятий</div>

    <!-- List of pending suggestions -->
    <TransitionGroup name="inbox">
      <div v-for="item in inbox" :key="item.id" class="inbox-item">
        <Avatar>{{ initials(item.giver_username) }}</Avatar>
        <div>
          <RouterLink :to="`/user/${item.giver_id}`">{{ item.giver_username }}</RouterLink>
          предлагает обнимашку
        </div>
        <span class="time">{{ relativeTime(item.created_at) }}</span>
        <div class="actions">
          <Button variant="yellow" @click="accept(item)">Принять</Button>
          <Button variant="ghost" @click="decline(item)">Отклонить</Button>
        </div>
      </div>
    </TransitionGroup>
  </CardContent>
</Card>
```

**Accept flow:**
1. Call `hugsStore.acceptHug(item.id)`
2. On success:
   - Trigger `HugExplosion` animation (teleported to body, centered on the "Принять" button)
   - Toast: "Обнимашка с {username} принята!" (or similar)
   - Item animates out of the list
   - Balance refreshes (+1 coin shown in header)

**Decline flow:**
1. Call `hugsStore.declineHug(item.id)`
2. Toast: "Обнимашка отклонена"
3. Item animates out

### 5.3 Create `src/components/OutgoingHugBanner.vue`

New component showing above the inbox on the dashboard when user has an outgoing pending hug.

**Template:**
```
<div v-if="outgoingHug" class="outgoing-banner">
  <Heart class="icon" />
  <span>
    Ты предложил(а) обнимашку
    <RouterLink :to="`/user/${outgoingHug.receiver_id}`">
      {{ outgoingHug.receiver_username }}
    </RouterLink>
  </span>
  <span class="time">{{ relativeTime(outgoingHug.created_at) }}</span>
  <Button variant="ghost" size="sm" @click="cancel">Отменить</Button>
</div>
```

**Cancel flow:**
1. Call `hugsStore.cancelOutgoing(outgoingHug.id)`
2. Toast: "Предложение отменено"
3. Banner disappears

### 5.4 Update `src/views/DashboardView.vue`

**Add new sections (in order from top):**
1. **Stats cards** (existing, but update labels — see Step 6)
2. **Rank + daily reward** (existing)
3. **`OutgoingHugBanner`** (new — shows when user has pending outgoing hug)
4. **`InboxSection`** (new — shows incoming pending hug suggestions)
5. **Hug history** (existing, update text — see Step 6)
6. **Quick links** (existing)

**On mount:**
- Fetch inbox: `hugsStore.fetchInbox()`
- Fetch outgoing: `hugsStore.fetchOutgoing()`

**WebSocket integration:**
- Subscribe to `hug_suggestion`: add to inbox, increment count
- Subscribe to `hug_declined`: clear outgoing hug, toast "Твоя обнимашка была отклонена"
- Subscribe to `hug_cancelled`: (shouldn't happen on dashboard since user is the one cancelling, but handle for completeness)
- Subscribe to `hug_completed`: if user is the initiator and someone just accepted their hug, clear outgoing hug, toast "Обнимашка с {user} принята!"

### 5.5 Update `src/views/FeedView.vue`

**Replace inline WebSocket with composable:**
- Remove the raw `WebSocket` creation code
- Use `useWebSocket().on('hug_completed', handler)` to subscribe to feed events
- The composable is already connected at app level; FeedView just subscribes to feed events
- Keep all the scroll-aware pending/flush logic, just change the data source

**Feed items remain unchanged** — they only show completed hugs (backend filters).

### 5.6 Update `src/views/UserProfileView.vue`

- Update stat labels (see Step 6)
- HugButton now suggests instead of sending
- Cooldown upgrade still works (shared cooldown, either user can upgrade)

### 5.7 Update `src/components/AppSidebar.vue` and `src/components/AppBottomNav.vue`

**Add inbox count badge:**
- Next to the "Главная" / dashboard link, show a small badge with `inboxCount` if > 0
- Use a small colored dot or number badge (shadcn `Badge` component)
- The count updates in real-time via WebSocket `hug_suggestion` events

### 5.8 Update `src/App.vue`

**Connect WebSocket on auth:**
- On app mount (when authenticated), call `useWebSocket().connect()`
- On logout, call `useWebSocket().disconnect()`
- Also fetch initial inbox count: `hugsStore.fetchInboxCount()`

---

## Step 6: Text Changes

### 6.1 Dashboard (`DashboardView.vue`)

| Location | Old text | New text |
|----------|----------|----------|
| Stats card 2 label | "Отправлено" | "Инициировано" |
| Stats card 3 label | "Получено" | "Принято" |
| History: sent hug | "Ты обнял(а) {user}" | "Ты инициировал(а) с {user}" |
| History: received hug | "{user} обнял(а) тебя" | "Ты принял(а) от {user}" |
| History: sent icon description | (ArrowUp = sent) | (ArrowUp = initiated) |
| History: received icon description | (ArrowDown = received) | (ArrowDown = accepted) |

### 6.2 Profile (`UserProfileView.vue`)

| Location | Old text | New text |
|----------|----------|----------|
| Stats "Отправлено" | "Отправлено" | "Инициировано" |
| Stats "Получено" | "Получено" | "Принято" |
| Mutual "N из них тебе" | Keep as-is | Keep as-is |
| Mutual "N из них от тебя" | Keep as-is | Keep as-is |

### 6.3 HugButton toast

| Old | New |
|-----|-----|
| "Ты обнял(а) {username}!" | "Ты предложил(а) обнимашку {username}!" |

### 6.4 Utils (`lib/utils.ts`)

Add a new helper:
```typescript
export function suggestVerb(gender?: string | null): string {
  if (gender === 'male') return 'предложил'
  if (gender === 'female') return 'предложила'
  return 'предложил(а)'
}

export function acceptVerb(gender?: string | null): string {
  if (gender === 'male') return 'принял'
  if (gender === 'female') return 'приняла'
  return 'принял(а)'
}

export function initiateVerb(gender?: string | null): string {
  if (gender === 'male') return 'инициировал'
  if (gender === 'female') return 'инициировала'
  return 'инициировал(а)'
}
```

### 6.5 Feed (`FeedView.vue`)

Feed items currently show: "{giver} обнял(а) {receiver}"

Update to: "{giver} обнял(а) {receiver}" — **keep as-is** for the feed since these are completed hugs and the verb "обнял(а)" is still appropriate for completed hugs displayed in the public feed.

---

## Step 7: Animations & Styling

### 7.1 Inbox item animations

- Items enter with a slide-in from the right
- Items exit with a fade + slide-out
- New items from WebSocket get the `feed-highlight` yellow flash animation (reuse from feed)

### 7.2 Outgoing banner styling

- Yellow/amber border to match the app's yellow theme
- Subtle pulse animation on the heart icon
- Smooth appear/disappear transition

### 7.3 Accept animation

- Reuse `HugExplosion` component
- Position at the "Принять" button center (same `getBoundingClientRect()` technique as HugButton)
- Toast appears simultaneously

### 7.4 Badge styling

- Small red/yellow circle with white number
- Positioned at top-right of the navigation item
- Pulse animation when count increases

---

## Step 8: Edge Cases

1. **User opens dashboard, has no pending hugs** → Empty inbox: "Нет входящих объятий"
2. **User opens dashboard, has outgoing hug that expired** → `fetchOutgoing()` returns null (backend filters by 24h) → banner doesn't show
3. **User accepts a hug that was already cancelled by the sender** → Backend returns error → toast: "Это объятие было отменено"
4. **User accepts a hug that expired** → Backend returns 410 → toast: "Это объятие истекло"
5. **WebSocket disconnects** → Reconnect after 3s → on reconnect, re-fetch inbox count to sync state
6. **Multiple browser tabs** → Each tab has its own WebSocket → inbox state may be slightly out of sync → not a concern for MVP, could add `BroadcastChannel` later
7. **User clicks "Обнять" but already has a pending hug to someone else** → Button is disabled with "Ожидание ответа..." → tooltip could explain the global limit

---

## Step 9: Verify

After all changes:

1. `bun run build` — must succeed (type-check + vite build)
2. `bun lint` — must pass
3. `bun format` — format all files
4. Manual testing:
   - Suggest a hug → button shows "Ожидание...", outgoing banner appears on dashboard
   - Cancel the suggestion → banner disappears, button re-enables
   - Log in as user2 → inbox shows the suggestion (if not cancelled)
   - Accept → explosion + toast + both get coins + hug appears in feed
   - Decline → suggestion removed, toast shown
   - Verify text changes: "Инициировано" / "Принято" everywhere
   - Verify inbox count badge in sidebar/bottom nav
   - Verify real-time: suggest as user1 → user2 sees it immediately in inbox
   - Verify cooldown is shared: after accept, neither user can suggest
   - Verify global pending limit: can't suggest to anyone while one is pending
   - Verify expiry: wait 24h (or mock) → suggestion disappears

---

## Dependencies on Backend

This frontend plan assumes the backend implements these endpoints:

- `POST /api/v1/hugs/{userId}` — returns `{ id, giver_id, receiver_id, receiver_username, status: "pending", created_at }`
- `POST /api/v1/hugs/{hugId}/accept` — returns completed hug
- `POST /api/v1/hugs/{hugId}/decline` — returns 200
- `POST /api/v1/hugs/{hugId}/cancel` — returns 200
- `GET /api/v1/hugs/inbox` — returns `PendingHugInboxItem[]`
- `GET /api/v1/hugs/outgoing` — returns `OutgoingPendingHug | null`
- `GET /api/v1/hugs/inbox/count` — returns `{ count: number }`
- WebSocket at `/api/v1/ws?token=...` sends typed messages: `{ type, data }`

Error codes: `ALREADY_HAS_PENDING_HUG` (409), `PENDING_HUG_EXISTS` (409), `HUG_NOT_FOUND` (404), `HUG_NOT_PENDING` (409), `HUG_EXPIRED` (410), `DECLINE_COOLDOWN_ACTIVE` (429)
