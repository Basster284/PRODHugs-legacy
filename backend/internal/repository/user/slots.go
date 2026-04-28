package user

import (
	"context"

	"go-service-template/internal/repository"

	"github.com/google/uuid"
	"github.com/jackc/pgx/v5"
)

func (r *repo) GetUserSlots(ctx context.Context, userID uuid.UUID) (int32, error) {
	q := repository.Queries(ctx, r.q)
	return q.GetUserSlots(ctx, userID)
}

func (r *repo) IncrementUserSlots(ctx context.Context, userID uuid.UUID) (int32, error) {
	q := repository.Queries(ctx, r.q)

	slots, err := q.IncrementUserSlots(ctx, userID)
	if err != nil {
		if err == pgx.ErrNoRows {
			// Already at max slots (5)
			return 0, nil
		}
		return 0, err
	}

	return slots, nil
}
