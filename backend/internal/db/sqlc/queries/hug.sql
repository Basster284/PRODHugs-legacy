-- name: InsertHug :one
INSERT INTO hugs (giver_id, receiver_id)
VALUES ($1, $2)
RETURNING id, giver_id, receiver_id, created_at;

-- name: GetHugByID :one
SELECT id, giver_id, receiver_id, created_at
FROM hugs
WHERE id = $1;

-- name: ListHugsByUser :many
SELECT
    h.id,
    h.giver_id,
    h.receiver_id,
    h.created_at,
    g.username AS giver_username,
    r.username AS receiver_username
FROM hugs h
JOIN users g ON g.id = h.giver_id
JOIN users r ON r.id = h.receiver_id
WHERE h.giver_id = $1 OR h.receiver_id = $1
ORDER BY h.created_at DESC;

-- name: CountHugsReceived :one
SELECT COUNT(*)
FROM hugs
WHERE receiver_id = $1;

-- name: CountHugsGiven :one
SELECT COUNT(*)
FROM hugs
WHERE giver_id = $1;
