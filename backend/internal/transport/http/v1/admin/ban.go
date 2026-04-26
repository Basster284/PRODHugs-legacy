package admin

import (
	"context"
	"errors"
	"go-service-template/internal/errorz"
	v1 "go-service-template/internal/transport/http/v1"
)

func (h *AdminHandler) BanUser(ctx context.Context, req v1.BanUserRequestObject) (v1.BanUserResponseObject, error) {
	u, err := h.svc.BanUser(ctx, req.UserId)
	if err != nil {
		if errors.Is(err, errorz.ErrCannotBanAdmin) {
			return v1.BanUser400JSONResponse{
				BadRequestJSONResponse: v1.BadRequestJSONResponse{
					Message: "cannot ban admin user",
					Code:    v1.CANNOTBANADMIN,
				},
			}, nil
		}
		return nil, err
	}

	return v1.BanUser200JSONResponse(toV1AdminUser(u)), nil
}

func (h *AdminHandler) UnbanUser(ctx context.Context, req v1.UnbanUserRequestObject) (v1.UnbanUserResponseObject, error) {
	u, err := h.svc.UnbanUser(ctx, req.UserId)
	if err != nil {
		if errors.Is(err, errorz.ErrUserNotFound) {
			return v1.UnbanUser404JSONResponse{
				NotFoundJSONResponse: v1.NotFoundJSONResponse{
					Message: "user not found",
					Code:    v1.USERNOTFOUND,
				},
			}, nil
		}
		return nil, err
	}

	return v1.UnbanUser200JSONResponse(toV1AdminUser(u)), nil
}
