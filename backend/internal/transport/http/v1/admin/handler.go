package admin

import (
	"context"
	"go-service-template/internal/models"
	v1 "go-service-template/internal/transport/http/v1"

	"github.com/google/uuid"
)

type service interface {
	GetAdminStats(ctx context.Context) (*models.AdminStats, error)
	ListUsersAdmin(ctx context.Context, limit, offset int32) ([]*models.AdminUser, error)
	BanUser(ctx context.Context, id uuid.UUID) (*models.User, error)
	UnbanUser(ctx context.Context, id uuid.UUID) (*models.User, error)
	AdminUpdateUsername(ctx context.Context, id uuid.UUID, username string) (*models.User, error)
	AdminUpdateGender(ctx context.Context, id uuid.UUID, gender *string) (*models.User, error)
	AdminUpdatePassword(ctx context.Context, id uuid.UUID, newPassword string) error
}

type AdminHandler struct {
	svc service
}

func New(svc service) *AdminHandler {
	return &AdminHandler{svc: svc}
}

func toV1AdminUser(u *models.User) v1.AdminUser {
	au := v1.AdminUser{
		Id:       u.ID,
		Username: u.Username,
		Role:     v1.AdminUserRole(u.Role),
	}
	if u.Gender != nil {
		g := v1.Gender(*u.Gender)
		au.Gender = &g
	}
	if u.BannedAt != nil {
		au.BannedAt = u.BannedAt
	}
	return au
}

func toV1AdminUserFromAdmin(u *models.AdminUser) v1.AdminUser {
	au := v1.AdminUser{
		Id:       u.ID,
		Username: u.Username,
		Role:     v1.AdminUserRole(u.Role),
	}
	if u.Gender != nil {
		g := v1.Gender(*u.Gender)
		au.Gender = &g
	}
	if u.BannedAt != nil {
		au.BannedAt = u.BannedAt
	}
	return au
}
