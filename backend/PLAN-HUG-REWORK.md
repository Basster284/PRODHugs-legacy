# Backend Implementation Plan: Hug Suggest + Accept Rework

Read `../PLAN.md` for the full overview. This document covers backend-only implementation steps.

## Prerequisites

- Read `AGENTS.md` in this directory for codegen commands and architecture overview.
- Generated code lives in `internal/transport/http/v1/api.gen.go` and `internal/db/sqlc/storage/`. Do NOT hand-edit those.
- Codegen commands (run from `backend/`):
  - `oapi-codegen -config oapi-codegen.yml api/openapi.yaml`
  - `sqlc generate -f internal/db/sqlc/sqlc.yaml`

---

## Step 1: Database Migration

Create `internal/db/migrations/00005_hug_suggestions.sql`:

### Up migration:

```sql
-- Add status and accepted_at to hugs table
ALTER TABLE hugs ADD COLUMN status TEXT NOT NULL DEFAULT 'completed'
    CHECK (status IN ('pending', 'completed', 'declined', 'expired', 'cancelled'));
ALTER TABLE hugs ADD COLUMN accepted_at TIMESTAMPTZ;

-- Backfill: all existing hugs are completed, accepted_at = created_at
UPDATE hugs SET accepted_at = created_at WHERE status = 'completed';

-- Index for inbox queries (pending hugs for a receiver)
CREATE INDEX idx_hugs_receiver_status ON hugs(receiver_id, status) WHERE status = 'pending';

-- Index for outgoing pending hug check
CREATE INDEX idx_hugs_giver_status ON hugs(giver_id, status) WHERE status = 'pending';

-- Rework hug_cooldowns to be symmetric (shared per pair)
-- Drop old table and recreate with canonical pair ordering
DROP TABLE hug_cooldowns;

CREATE TABLE hug_cooldowns (
    user_a_id UUID NOT NULL REFERENCES users(id),
    user_b_id UUID NOT NULL REFERENCES users(id),
    last_hug_at TIMESTAMPTZ NOT NULL DEFAULT now(),
    cooldown_seconds INTEGER NOT NULL DEFAULT 3600,
    decline_cooldown_until TIMESTAMPTZ,
    PRIMARY KEY (user_a_id, user_b_id),
    CHECK (user_a_id < user_b_id)
);
```

### Down migration:

```sql
ALTER TABLE hugs DROP COLUMN status;
ALTER TABLE hugs DROP COLUMN accepted_at;
DROP TABLE hug_cooldowns;
CREATE TABLE hug_cooldowns (
    giver_id UUID NOT NULL REFERENCES users(id),
    receiver_id UUID NOT NULL REFERENCES users(id),
    last_hug_at TIMESTAMPTZ NOT NULL DEFAULT now(),
    cooldown_seconds INTEGER NOT NULL DEFAULT 3600,
    PRIMARY KEY (giver_id, receiver_id)
);
```

**Note**: Dropping `hug_cooldowns` loses all existing cooldown state. This is acceptable since it's transient data — all cooldowns will reset, which is fine for a rework.

---

## Step 2: SQL Queries

### 2.1 Update `internal/db/sqlc/queries/hug.sql`

Keep `GetHugByID`, but add `status` and `accepted_at` to the SELECT.

**Update `InsertHug`** — now takes a status parameter:
```sql
-- name: InsertHug :one
INSERT INTO hugs (giver_id, receiver_id, status)
VALUES ($1, $2, $3)
RETURNING *;
```

**Add new queries:**

```sql
-- name: AcceptHug :one
UPDATE hugs SET status = 'completed', accepted_at = now()
WHERE id = $1 AND receiver_id = $2 AND status = 'pending'
RETURNING *;

-- name: DeclineHug :one
UPDATE hugs SET status = 'declined'
WHERE id = $1 AND receiver_id = $2 AND status = 'pending'
RETURNING *;

-- name: CancelHug :one
UPDATE hugs SET status = 'cancelled'
WHERE id = $1 AND giver_id = $2 AND status = 'pending'
RETURNING *;

-- name: GetPendingHugsForUser :many
SELECT h.id, h.giver_id, h.receiver_id, h.status, h.created_at, h.accepted_at,
       g.username AS giver_username, g.gender AS giver_gender
FROM hugs h
JOIN users g ON g.id = h.giver_id
WHERE h.receiver_id = $1
  AND h.status = 'pending'
  AND h.created_at > now() - INTERVAL '24 hours'
ORDER BY h.created_at DESC;

-- name: GetOutgoingPendingHug :one
SELECT h.id, h.giver_id, h.receiver_id, h.status, h.created_at, h.accepted_at,
       r.username AS receiver_username, r.gender AS receiver_gender
FROM hugs h
JOIN users r ON r.id = h.receiver_id
WHERE h.giver_id = $1
  AND h.status = 'pending'
  AND h.created_at > now() - INTERVAL '24 hours'
LIMIT 1;

-- name: CountPendingHugsForUser :one
SELECT COUNT(*) FROM hugs
WHERE receiver_id = $1
  AND status = 'pending'
  AND created_at > now() - INTERVAL '24 hours';

-- name: HasOutgoingPendingHug :one
SELECT EXISTS(
    SELECT 1 FROM hugs
    WHERE giver_id = $1
      AND status = 'pending'
      AND created_at > now() - INTERVAL '24 hours'
) AS has_pending;

-- name: ExpirePendingHugs :exec
UPDATE hugs SET status = 'expired'
WHERE status = 'pending'
  AND created_at <= now() - INTERVAL '24 hours';
```

**Update existing queries** to filter by `status = 'completed'`:

```sql
-- name: ListHugsByUser :many
-- Add: AND h.status = 'completed'

-- name: CountHugsReceived :one
-- Add: AND status = 'completed'

-- name: CountHugsGiven :one
-- Add: AND status = 'completed'

-- name: CountMutualHugs :one
-- Add: AND status = 'completed' (in both directions)
```

### 2.2 Update `internal/db/sqlc/queries/cooldown.sql`

Rewrite for symmetric pair. All queries must use `LEAST/GREATEST` pattern or expect canonical ordering:

```sql
-- name: GetCooldown :one
SELECT user_a_id, user_b_id, last_hug_at, cooldown_seconds, decline_cooldown_until
FROM hug_cooldowns
WHERE user_a_id = LEAST($1::UUID, $2::UUID)
  AND user_b_id = GREATEST($1::UUID, $2::UUID);

-- name: UpsertCooldown :one
INSERT INTO hug_cooldowns (user_a_id, user_b_id, cooldown_seconds)
VALUES (LEAST($1::UUID, $2::UUID), GREATEST($1::UUID, $2::UUID), $3)
ON CONFLICT (user_a_id, user_b_id)
DO UPDATE SET last_hug_at = now()
RETURNING *;

-- name: ReduceCooldown :one
UPDATE hug_cooldowns
SET cooldown_seconds = GREATEST(cooldown_seconds - @reduction::INTEGER, 300)
WHERE user_a_id = LEAST($1::UUID, $2::UUID)
  AND user_b_id = GREATEST($1::UUID, $2::UUID)
RETURNING *;

-- name: SetDeclineCooldown :exec
INSERT INTO hug_cooldowns (user_a_id, user_b_id, decline_cooldown_until, cooldown_seconds)
VALUES (LEAST($1::UUID, $2::UUID), GREATEST($1::UUID, $2::UUID), $3, 3600)
ON CONFLICT (user_a_id, user_b_id)
DO UPDATE SET decline_cooldown_until = $3;
```

**Delete** `GetOrCreateCooldown` (unused).

### 2.3 Update `internal/db/sqlc/queries/user.sql`

**Update `GetRecentHugsFeed`**: add `AND h.status = 'completed'` filter.

**Update `GetUserStats`**: add `AND status = 'completed'` to the count subqueries.

**Update `GetLeaderboard`**: add `AND status = 'completed'` to the count subqueries.

### 2.4 Run `sqlc generate -f internal/db/sqlc/sqlc.yaml`

---

## Step 3: Models

### 3.1 Update `internal/models/hug.go`

```go
// Hug status constants
const (
    HugStatusPending   = "pending"
    HugStatusCompleted = "completed"
    HugStatusDeclined  = "declined"
    HugStatusExpired   = "expired"
    HugStatusCancelled = "cancelled"
)

// Add to Hug struct:
type Hug struct {
    ID         uuid.UUID
    GiverID    uuid.UUID
    ReceiverID uuid.UUID
    Status     string
    CreatedAt  time.Time
    AcceptedAt *time.Time
}

// HugFeedItem — add Status field (optional, only needed for inbox items)
// Keep existing fields, add Status and AcceptedAt

// Add new types:
type PendingHugInboxItem struct {
    ID            uuid.UUID
    GiverID       uuid.UUID
    ReceiverID    uuid.UUID
    GiverUsername string
    GiverGender   *string
    CreatedAt     time.Time
}

type OutgoingPendingHug struct {
    ID               uuid.UUID
    GiverID          uuid.UUID
    ReceiverID       uuid.UUID
    ReceiverUsername  string
    ReceiverGender   *string
    CreatedAt        time.Time
}
```

### 3.2 Update `internal/models/hug.go` — HugCooldown

```go
type HugCooldown struct {
    UserAID              uuid.UUID  // LEAST of the pair
    UserBID              uuid.UUID  // GREATEST of the pair
    LastHugAt            time.Time
    CooldownSeconds      int32
    DeclineCooldownUntil *time.Time
}
```

### 3.3 Update `internal/errorz/hug.go`

Add:
```go
var (
    ErrAlreadyHasPendingHug = errors.New("already has a pending hug")
    ErrPendingHugExists     = errors.New("pending hug already exists for this pair")
    ErrHugNotFound          = errors.New("hug not found")
    ErrHugNotPending        = errors.New("hug is not in pending state")
    ErrHugExpired           = errors.New("hug suggestion has expired")
    ErrDeclineCooldownActive = errors.New("decline cooldown is active")
)
```

---

## Step 4: Repository Layer

### 4.1 Update `internal/repository/hug/repo.go`

- Update all mapping functions for new fields (`status`, `accepted_at`)
- Update `toModelCooldown` for new fields (`user_a_id`, `user_b_id`, `decline_cooldown_until`)
- Add mapping functions for `PendingHugInboxItem` and `OutgoingPendingHug`

### 4.2 Update `internal/repository/hug/hug.go`

- `InsertHug(ctx, giverID, receiverID, status)` — pass status parameter
- Add `AcceptHug(ctx, hugID, receiverID)` → returns `*models.Hug`
- Add `DeclineHug(ctx, hugID, receiverID)` → returns `*models.Hug`
- Add `CancelHug(ctx, hugID, giverID)` → returns `*models.Hug`
- Add `GetPendingHugsForUser(ctx, userID)` → returns `[]*models.PendingHugInboxItem`
- Add `GetOutgoingPendingHug(ctx, userID)` → returns `*models.OutgoingPendingHug, error`
- Add `CountPendingHugsForUser(ctx, userID)` → returns `int64`
- Add `HasOutgoingPendingHug(ctx, userID)` → returns `bool`

### 4.3 Update `internal/repository/hug/cooldown.go`

- All methods now take two user IDs (the pair); SQL handles canonical ordering via LEAST/GREATEST
- `GetCooldown(ctx, userA, userB)` — same signature, symmetric lookup
- `UpsertCooldown(ctx, userA, userB, cooldownSeconds)` — symmetric
- `ReduceCooldown(ctx, userA, userB, reduction)` — symmetric
- Add `SetDeclineCooldown(ctx, userA, userB, until time.Time)` — sets the 5-min decline cooldown

### 4.4 Update `internal/repository/hug/feed.go`

- `GetRecentFeed` — no code change needed (SQL already filtered)
- Same for leaderboard, user stats, etc.

---

## Step 5: Service Layer

### 5.1 Update `internal/service/hug/service.go`

Add new interface methods:
```go
type hugRepo interface {
    // ... existing ...
    AcceptHug(ctx context.Context, hugID, receiverID uuid.UUID) (*models.Hug, error)
    DeclineHug(ctx context.Context, hugID, receiverID uuid.UUID) (*models.Hug, error)
    CancelHug(ctx context.Context, hugID, giverID uuid.UUID) (*models.Hug, error)
    GetPendingHugsForUser(ctx context.Context, userID uuid.UUID) ([]*models.PendingHugInboxItem, error)
    GetOutgoingPendingHug(ctx context.Context, userID uuid.UUID) (*models.OutgoingPendingHug, error)
    CountPendingHugsForUser(ctx context.Context, userID uuid.UUID) (int64, error)
    HasOutgoingPendingHug(ctx context.Context, userID uuid.UUID) (bool, error)
    SetDeclineCooldown(ctx context.Context, userA, userB uuid.UUID, until time.Time) error
}
```

Add new callback types for WebSocket:
```go
type HugSuggestionCallback func(targetUserID uuid.UUID, item *PendingHugInboxItem)
type HugDeclinedCallback func(targetUserID uuid.UUID, hugID uuid.UUID)
type HugCancelledCallback func(targetUserID uuid.UUID, hugID uuid.UUID)
```

### 5.2 Rewrite `internal/service/hug/hug.go`

**Constants** — add:
```go
const (
    suggestionExpiryHours  = 24
    declineCooldownSeconds = 300 // 5 minutes
)
```

**`SuggestHug(ctx, giverID, receiverID)`** (replaces `SendHug`):
1. Self-hug check → `ErrCannotHugSelf`
2. Receiver exists → `userRepo.GetByID(receiverID)` → `ErrUserNotFound`
3. `HasOutgoingPendingHug(giverID)` → `ErrAlreadyHasPendingHug`
4. Check shared cooldown: `GetCooldown(giverID, receiverID)`
   - If cooldown active → `ErrHugCooldownActive`
   - If `decline_cooldown_until` is in the future → `ErrDeclineCooldownActive`
5. `InsertHug(giverID, receiverID, "pending")` — creates pending hug
6. Fire WebSocket `hug_suggestion` to user2 (targeted)
7. Return the pending hug

**`AcceptHug(ctx, hugID, receiverID)`** (new):
1. `hugRepo.AcceptHug(hugID, receiverID)` — returns nil if not found/not pending
   - If nil → check if hug exists but expired → `ErrHugExpired`
   - If nil → `ErrHugNotFound` or `ErrHugNotPending`
2. **Wrap in transaction** (`transactor.RunInTx`):
   a. Accept the hug (status → completed)
   b. `balanceRepo.AddBalance(hug.GiverID, 1)` — initiator gets +1
   c. `balanceRepo.AddBalance(hug.ReceiverID, 1)` — acceptor gets +1
   d. `hugRepo.UpsertCooldown(giverID, receiverID, cooldownSeconds)` — shared cooldown starts
3. Fire WebSocket `hug_completed` broadcast (feed item)
4. Return the completed hug

**`DeclineHug(ctx, hugID, receiverID)`** (new):
1. `hugRepo.DeclineHug(hugID, receiverID)` — returns nil if not found/not pending
2. Set decline cooldown: `SetDeclineCooldown(giverID, receiverID, now + 5min)`
3. Fire WebSocket `hug_declined` to user1 (targeted)
4. Return success

**`CancelHug(ctx, hugID, giverID)`** (new):
1. `hugRepo.CancelHug(hugID, giverID)` — returns nil if not found/not pending
2. Fire WebSocket `hug_cancelled` to user2 (targeted)
3. Return success

**`GetCooldownInfo(ctx, userA, userB)`** — update for symmetric pair and decline cooldown:
- Returns: `(cooldown, remainingSeconds, canHug, declineCooldownRemaining, error)`

**`UpgradeCooldown(ctx, userA, userB)`** — either user can pay. Same logic but symmetric cooldown.

### 5.3 Update `internal/service/hug/queries.go`

Add:
- `GetPendingInbox(ctx, userID)` → calls `hugRepo.GetPendingHugsForUser`
- `GetOutgoingPendingHug(ctx, userID)` → calls `hugRepo.GetOutgoingPendingHug`
- `GetInboxCount(ctx, userID)` → calls `hugRepo.CountPendingHugsForUser`

---

## Step 6: OpenAPI Spec

### 6.1 Update `api/openapi.yaml`

**Update `POST /hugs/{userId}`** (`sendHug` → `suggestHug`):
- operationId: `suggestHug`
- Description: Suggest a hug to another user
- 201 response now returns hug with `status: "pending"`
- Add 409 for `ALREADY_HAS_PENDING_HUG` and `PENDING_HUG_EXISTS`
- Add 429 for `DECLINE_COOLDOWN_ACTIVE`

**Add new endpoints:**

```yaml
/hugs/{hugId}/accept:
  post:
    operationId: acceptHug
    security: [bearerAuth: [user, admin]]
    parameters:
      - name: hugId
        in: path
        required: true
        schema: { type: string, format: uuid }
    responses:
      200: { description: Hug accepted, content: ... }
      404: { description: HUG_NOT_FOUND }
      409: { description: HUG_NOT_PENDING }
      410: { description: HUG_EXPIRED }

/hugs/{hugId}/decline:
  post:
    operationId: declineHug
    # same pattern as accept, returns 200 on success

/hugs/{hugId}/cancel:
  post:
    operationId: cancelHug
    # same pattern, uses giverID from JWT

/hugs/inbox:
  get:
    operationId: getHugInbox
    security: [bearerAuth: [user, admin]]
    responses:
      200:
        content:
          application/json:
            schema:
              type: array
              items: { $ref: '#/components/schemas/PendingHugInboxItem' }

/hugs/outgoing:
  get:
    operationId: getOutgoingHug
    security: [bearerAuth: [user, admin]]
    responses:
      200:
        content:
          application/json:
            schema:
              oneOf:
                - $ref: '#/components/schemas/OutgoingPendingHug'
                - type: "null"

/hugs/inbox/count:
  get:
    operationId: getHugInboxCount
    security: [bearerAuth: [user, admin]]
    responses:
      200:
        content:
          application/json:
            schema:
              type: object
              properties:
                count: { type: integer }
```

**Add new schemas:**

```yaml
PendingHugInboxItem:
  type: object
  required: [id, giver_id, receiver_id, giver_username, created_at]
  properties:
    id: { type: string, format: uuid }
    giver_id: { type: string, format: uuid }
    receiver_id: { type: string, format: uuid }
    giver_username: { type: string }
    giver_gender: { type: string, nullable: true }
    created_at: { type: string, format: date-time }

OutgoingPendingHug:
  type: object
  required: [id, giver_id, receiver_id, receiver_username, created_at]
  properties:
    id: { type: string, format: uuid }
    giver_id: { type: string, format: uuid }
    receiver_id: { type: string, format: uuid }
    receiver_username: { type: string }
    receiver_gender: { type: string, nullable: true }
    created_at: { type: string, format: date-time }
```

**Update Hug schema** to include `status` and `accepted_at`.

### 6.2 Run `oapi-codegen -config oapi-codegen.yml api/openapi.yaml`

---

## Step 7: HTTP Handlers

### 7.1 Update `internal/transport/http/v1/hug/handler.go`

Add interface methods for new service calls.

### 7.2 Rewrite `internal/transport/http/v1/hug/hug.go`

**`SuggestHug`** (was `SendHug`):
- Same request shape (POST with userId path param)
- Returns 201 with pending hug
- Error mapping:
  - `ErrAlreadyHasPendingHug` → 409 `ALREADY_HAS_PENDING_HUG`
  - `ErrPendingHugExists` → 409 `PENDING_HUG_EXISTS`
  - `ErrDeclineCooldownActive` → 429 `DECLINE_COOLDOWN_ACTIVE`
  - `ErrHugCooldownActive` → 429 `COOLDOWN_ACTIVE`

**`AcceptHug`** (new):
- POST /hugs/{hugId}/accept
- Extracts receiverID from JWT, hugID from path
- Error mapping: 404/409/410

**`DeclineHug`** (new):
- POST /hugs/{hugId}/decline
- Same pattern

**`CancelHug`** (new):
- POST /hugs/{hugId}/cancel
- Uses giverID from JWT

### 7.3 Update `internal/transport/http/v1/hug/queries.go`

Add handlers:
- `GetHugInbox` — returns inbox items
- `GetOutgoingHug` — returns outgoing pending hug or null
- `GetHugInboxCount` — returns `{ count: N }`

### 7.4 Update `internal/transport/http/v1/hug/dto.go`

Add DTOs for:
- `PendingHugInboxItemDTO`
- `OutgoingPendingHugDTO`

Update `HugFeedItemDTO` to include `status` if needed.

---

## Step 8: WebSocket — Auth + Targeted Delivery

### 8.1 Update `internal/ws/hub.go`

**Add user tracking:**
```go
type client struct {
    userID uuid.UUID  // NEW: associated user
    send   chan []byte
    conn   *websocket.Conn
}

type Hub struct {
    clients   map[*client]struct{}
    userIndex map[uuid.UUID]map[*client]struct{} // NEW: user -> clients
    // ... existing fields
}
```

**Add `SendToUser(userID uuid.UUID, msg any)`:**
- Looks up clients by userID in the index
- JSON-marshals and sends to those clients only

**Add authentication:**
- On WebSocket connect, expect a JWT token as query parameter: `/api/v1/ws?token=...`
- Validate the token using the same JWT secret/middleware
- Extract userID and associate with the client
- Reject connection if token is invalid

**Typed messages:**
- All outgoing messages are now wrapped: `{ "type": "...", "data": ... }`
- `Broadcast(msgType string, data any)` — sends to all clients
- `SendToUser(userID uuid.UUID, msgType string, data any)` — sends to specific user

### 8.2 Update `internal/app/app.go`

Wire up new callbacks:
```go
hugService.SetHugCompletedCallback(func(item *models.HugFeedItem) {
    a.hub.Broadcast("hug_completed", hughandler.ToFeedItemDTO(item))
})
hugService.SetHugSuggestionCallback(func(targetUserID uuid.UUID, item *models.PendingHugInboxItem) {
    a.hub.SendToUser(targetUserID, "hug_suggestion", item)
})
hugService.SetHugDeclinedCallback(func(targetUserID uuid.UUID, hugID uuid.UUID) {
    a.hub.SendToUser(targetUserID, "hug_declined", map[string]string{"hug_id": hugID.String()})
})
hugService.SetHugCancelledCallback(func(targetUserID uuid.UUID, hugID uuid.UUID) {
    a.hub.SendToUser(targetUserID, "hug_cancelled", map[string]string{"hug_id": hugID.String()})
})
```

---

## Step 9: Verify

After all changes:

1. Run `sqlc generate -f internal/db/sqlc/sqlc.yaml` — must succeed
2. Run `oapi-codegen -config oapi-codegen.yml api/openapi.yaml` — must succeed
3. Run `go build ./cmd` — must compile
4. Run `docker compose -f compose-dev.yml up` from repo root — migration must apply, app must start
5. Test manually:
   - Suggest a hug → should return pending
   - Check inbox for receiver → should see the suggestion
   - Accept → both get coins, hug appears in feed
   - Decline → 5-min cooldown
   - Cancel → no cooldown
   - Expiry → after 24h, suggestion gone
   - Cooldown shared → after accept, neither user can suggest to the other
   - Global pending limit → with pending hug, can't suggest to anyone else
   - WebSocket → real-time notifications for all events

---

## Key Decisions

1. **No background job for expiry**: Pending hugs are filtered by `created_at > now() - 24h` in all queries. The `ExpirePendingHugs` SQL exists for optional cleanup but is not strictly required.
2. **Transaction for AcceptHug**: Use `transactor.RunInTx` to wrap status update + both balance additions + cooldown upsert.
3. **Cooldown table recreation**: The migration drops and recreates `hug_cooldowns` with the new symmetric schema. Existing cooldown data is lost (acceptable for a rework).
4. **WebSocket auth via query param**: JWT token passed as `?token=` on the WebSocket URL. This is standard practice for WebSocket auth since custom headers aren't supported by the browser WebSocket API.
