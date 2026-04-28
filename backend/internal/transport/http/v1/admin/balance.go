package admin

import (
	"context"

	v1 "go-service-template/internal/transport/http/v1"
)

func (h *AdminHandler) AdminUpdateBalance(ctx context.Context, req v1.AdminUpdateBalanceRequestObject) (v1.AdminUpdateBalanceResponseObject, error) {
	bal, err := h.svc.AdminUpdateBalance(ctx, req.UserId, int32(req.Body.Amount))
	if err != nil {
		return nil, err
	}

	return v1.AdminUpdateBalance200JSONResponse{
		UserId: bal.UserID,
		Amount: int(bal.Amount),
	}, nil
}
