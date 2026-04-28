-- +goose Up
ALTER TABLE users ADD COLUMN hug_slots int NOT NULL DEFAULT 1;

-- +goose Down
ALTER TABLE users DROP COLUMN hug_slots;
