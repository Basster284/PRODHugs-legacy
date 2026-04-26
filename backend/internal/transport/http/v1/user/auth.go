package user

import (
	"context"
	v1 "go-service-template/internal/transport/http/v1"
	"net/http"
	"time"
)

func makeRefreshCookie(token string, maxAge time.Duration) *http.Cookie {
	return &http.Cookie{
		Name:     "refresh_token",
		Value:    token,
		Path:     "/api/v1/auth/",
		HttpOnly: true,
		Secure:   false, // set true when behind HTTPS in production
		SameSite: http.SameSiteLaxMode,
		MaxAge:   int(maxAge.Seconds()),
	}
}

func expiredRefreshCookie() *http.Cookie {
	return &http.Cookie{
		Name:     "refresh_token",
		Value:    "",
		Path:     "/api/v1/auth/",
		HttpOnly: true,
		Secure:   false,
		SameSite: http.SameSiteLaxMode,
		MaxAge:   -1,
	}
}

func (h *UserHandler) RefreshToken(ctx context.Context, req v1.RefreshTokenRequestObject) (v1.RefreshTokenResponseObject, error) {
	tokenString := req.Params.RefreshToken

	userID, _, tokenType, err := h.jwtManager.ParseToken(tokenString)
	if err != nil {
		return v1.RefreshToken401JSONResponse{
			UnauthorizedJSONResponse: v1.UnauthorizedJSONResponse{
				Message: "invalid refresh token",
				Code:    v1.INVALIDCREDENTIALS,
			},
		}, nil
	}

	if tokenType != "refresh" {
		return v1.RefreshToken401JSONResponse{
			UnauthorizedJSONResponse: v1.UnauthorizedJSONResponse{
				Message: "invalid token type",
				Code:    v1.INVALIDCREDENTIALS,
			},
		}, nil
	}

	// Look up user to get current role
	u, err := h.svc.GetByID(ctx, userID)
	if err != nil {
		return v1.RefreshToken401JSONResponse{
			UnauthorizedJSONResponse: v1.UnauthorizedJSONResponse{
				Message: "user not found",
				Code:    v1.INVALIDCREDENTIALS,
			},
		}, nil
	}

	if u.BannedAt != nil {
		return v1.RefreshToken403JSONResponse{
			ForbiddenJSONResponse: v1.ForbiddenJSONResponse{
				Message: "Ваш аккаунт заблокирован",
				Code:    v1.USERBANNED,
			},
		}, nil
	}

	accessToken, _, err := h.jwtManager.GenerateAccessToken(u.ID, u.Role)
	if err != nil {
		return nil, err
	}

	// Rotate refresh token
	newRefreshToken, err := h.jwtManager.GenerateRefreshToken(u.ID)
	if err != nil {
		return nil, err
	}

	cookie := makeRefreshCookie(newRefreshToken, h.jwtManager.RefreshTokenDuration())

	return v1.RefreshToken200JSONResponse{
		Body: struct {
			Token string `json:"token"`
		}{Token: accessToken},
		Headers: v1.RefreshToken200ResponseHeaders{
			SetCookie: cookie.String(),
		},
	}, nil
}

func (h *UserHandler) Logout(_ context.Context, _ v1.LogoutRequestObject) (v1.LogoutResponseObject, error) {
	cookie := expiredRefreshCookie()

	return v1.Logout200JSONResponse{
		Body: struct {
			Message string `json:"message"`
		}{Message: "logged out"},
		Headers: v1.Logout200ResponseHeaders{
			SetCookie: cookie.String(),
		},
	}, nil
}
