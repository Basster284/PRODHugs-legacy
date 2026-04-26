# Hug System Rework: Suggest + Accept Model

## Overview

Rework the hug system from instant send/receive to a two-phase suggest/accept flow:

1. **User1 suggests** a hug to user2
2. **User2 accepts** (or declines) from their inbox on the dashboard
3. Only accepted hugs appear in the feed, earn coins, and count toward stats

## Core Rules

| Rule | Detail |
|------|--------|
| **Two-phase hugs** | User1 suggests → user2 accepts or declines |
| **Shared cooldown** | One cooldown per pair (A,B), direction doesn't matter. Starts on accept. |
| **Both get coins** | On accept: +1 coin to user1 (initiator) AND +1 coin to user2 (acceptor) |
| **Global pending limit** | User1 can only have ONE outgoing pending hug at a time (to anyone). Can cancel it. |
| **One pending per pair** | Can't send another suggestion to same person while one is pending |
| **24h expiry** | Pending suggestions expire after 24 hours (no cooldown penalty on expiry) |
| **Decline cooldown** | 5-minute cooldown on the pair after a decline |
| **Cancel** | User1 can cancel their outgoing pending hug (no cooldown penalty) |
| **Feed** | Only shows completed (accepted) hugs |
| **Text changes** | "отправлено" → "инициировано", "получено" → "принято" everywhere |
| **Inbox** | Section on dashboard showing incoming pending hug suggestions |
| **Outgoing banner** | Banner above inbox on dashboard showing user1's outgoing pending hug with cancel button |
| **Real-time** | WebSocket notifies user2 of new suggestions; notifies user1 of decline/cancel |
| **Cooldown upgrade** | Either user in a pair can pay to upgrade the shared cooldown |
| **Accept animation** | Full heart explosion + toast on accept, same as current hug send |

## Hug Lifecycle State Machine

```
                ┌──────────┐
   suggest      │          │  24h elapsed
  ──────────►   │ PENDING  │ ──────────► EXPIRED (no cooldown)
                │          │
                └────┬─────┘
                     │
          ┌──────────┼──────────┐
          │          │          │
          ▼          ▼          ▼
     COMPLETED    DECLINED   CANCELLED
     (accept)    (decline)   (cancel)
          │          │          │
          │    5-min cooldown   │
          │    on the pair    no penalty
          │
     +1 coin each
     shared cooldown starts
     appears in feed
```

## New API Endpoints

| Method | Path | Description |
|--------|------|-------------|
| `POST` | `/hugs/{userId}` | Suggest a hug (was: send instantly) — returns pending hug |
| `POST` | `/hugs/{hugId}/accept` | Accept a pending hug suggestion |
| `POST` | `/hugs/{hugId}/decline` | Decline a pending hug suggestion |
| `POST` | `/hugs/{hugId}/cancel` | Cancel your own outgoing pending hug |
| `GET` | `/hugs/inbox` | Get pending incoming hug suggestions |
| `GET` | `/hugs/outgoing` | Get current user's outgoing pending hug (if any) |
| `GET` | `/hugs/inbox/count` | Get count of pending incoming suggestions (for badge) |

## New Error Codes

| Code | HTTP | When |
|------|------|------|
| `ALREADY_HAS_PENDING_HUG` | 409 | User1 already has a pending outgoing hug |
| `HUG_NOT_FOUND` | 404 | Hug ID not found or doesn't belong to user |
| `HUG_NOT_PENDING` | 409 | Trying to accept/decline a non-pending hug |
| `HUG_EXPIRED` | 410 | Pending hug is older than 24 hours |
| `DECLINE_COOLDOWN_ACTIVE` | 429 | Can't suggest to this person yet (5-min decline cooldown) |
| `PENDING_HUG_EXISTS` | 409 | Already have a pending hug with this specific user |

## WebSocket Message Types

```json
{ "type": "hug_completed",   "data": { ...HugFeedItem } }          // broadcast to all
{ "type": "hug_suggestion",  "data": { ...PendingHug } }           // targeted to user2
{ "type": "hug_declined",    "data": { "hug_id": "..." } }         // targeted to user1
{ "type": "hug_cancelled",   "data": { "hug_id": "..." } }         // targeted to user2
{ "type": "hug_expired",     "data": { "hug_id": "..." } }         // targeted to both
{ "type": "inbox_count",     "data": { "count": N } }              // targeted to user
```

## Execution Order

1. Database migration (new columns + constraints)
2. SQL queries + `sqlc generate`
3. Backend models, errors, repository, service, OpenAPI, handlers, WebSocket
4. Frontend API client, WebSocket composable, store, components, text changes

Backend and frontend can be developed in parallel once the API contract (OpenAPI spec) is agreed upon.

## Files Affected

### Backend
- `internal/db/migrations/00005_hug_suggestions.sql` (new)
- `internal/db/sqlc/queries/hug.sql` (update)
- `internal/db/sqlc/queries/cooldown.sql` (update)
- `internal/db/sqlc/storage/` (regenerate)
- `internal/models/hug.go` (update)
- `internal/errorz/hug.go` (update)
- `internal/repository/hug/hug.go` (update)
- `internal/repository/hug/cooldown.go` (update)
- `internal/repository/hug/feed.go` (update)
- `internal/repository/hug/repo.go` (update)
- `internal/service/hug/hug.go` (major rewrite)
- `internal/service/hug/queries.go` (update)
- `internal/service/hug/service.go` (update)
- `api/openapi.yaml` (update)
- `internal/transport/http/v1/api.gen.go` (regenerate)
- `internal/transport/http/v1/hug/handler.go` (update)
- `internal/transport/http/v1/hug/hug.go` (major rewrite)
- `internal/transport/http/v1/hug/queries.go` (update)
- `internal/transport/http/v1/hug/dto.go` (update)
- `internal/ws/hub.go` (major update — auth + targeted delivery)
- `internal/app/app.go` (update wiring)

### Frontend
- `src/api/client.ts` (add new endpoints)
- `src/stores/hugs.ts` (add inbox state + actions)
- `src/composables/useWebSocket.ts` (new — extract from FeedView)
- `src/components/HugButton.vue` (update — pending state)
- `src/components/InboxSection.vue` (new — inbox on dashboard)
- `src/components/OutgoingHugBanner.vue` (new — outgoing pending hug)
- `src/views/DashboardView.vue` (add inbox section + outgoing banner + text changes)
- `src/views/FeedView.vue` (use composable, handle typed WS messages)
- `src/views/UserProfileView.vue` (text changes)
- `src/components/AppSidebar.vue` (inbox count badge)
- `src/components/AppBottomNav.vue` (inbox count badge)
- `src/components/AppHeader.vue` (possibly inbox count)
- `src/lib/utils.ts` (add suggestVerb helper)
