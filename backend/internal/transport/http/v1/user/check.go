package user

import (
	"context"
	"errors"

	"go-service-template/internal/errorz"
	v1 "go-service-template/internal/transport/http/v1"
)

func (h *UserHandler) CheckUsername(ctx context.Context, req v1.CheckUsernameRequestObject) (v1.CheckUsernameResponseObject, error) {
	_, err := h.svc.GetByUsername(ctx, req.Params.Username)
	if err != nil {
		if errors.Is(err, errorz.ErrUserNotFound) {
			return v1.CheckUsername200JSONResponse{Available: true}, nil
		}
		return nil, err
	}

	return v1.CheckUsername200JSONResponse{Available: false}, nil
}
