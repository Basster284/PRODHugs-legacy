package admin

import (
	"context"
	v1 "go-service-template/internal/transport/http/v1"
)

func (h *AdminHandler) GetAdminStats(ctx context.Context, req v1.GetAdminStatsRequestObject) (v1.GetAdminStatsResponseObject, error) {
	stats, err := h.svc.GetAdminStats(ctx)
	if err != nil {
		return nil, err
	}

	return v1.GetAdminStats200JSONResponse{
		TotalUsers:  int(stats.TotalUsers),
		BannedUsers: int(stats.BannedUsers),
	}, nil
}
