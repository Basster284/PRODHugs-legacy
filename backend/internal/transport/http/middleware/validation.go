package middleware

import (
	"fmt"
	"go-service-template/internal/jwt"
	v1 "go-service-template/internal/transport/http/v1"
	"net/http"
	"strings"

	"github.com/getkin/kin-openapi/openapi3filter"
	"github.com/labstack/echo/v4"

	oapimiddleware "github.com/oapi-codegen/echo-middleware"
)

func OpenAPIValidationMiddleware(jwtManager *jwt.Manager) (echo.MiddlewareFunc, error) {
	swagger, err := v1.GetSwagger()
	if err != nil {
		return nil, fmt.Errorf("get swagger failed: %w", err)
	}

	validatorOptions := &oapimiddleware.Options{
		Options: openapi3filter.Options{
			AuthenticationFunc: NewAuthenticator(jwtManager),
		},
		ErrorHandler: func(c echo.Context, err *echo.HTTPError) error {
			finalCode := err.Code
			msg := fmt.Sprintf("%v", err.Message)
			// All security/auth failures come as 403 from the OAPI middleware,
			// but the frontend expects 401 for any token issue (missing, expired, invalid).
			if err.Code == http.StatusForbidden && (strings.Contains(msg, "Authorization") ||
				strings.Contains(msg, "token") ||
				strings.Contains(msg, "security")) {
				finalCode = http.StatusUnauthorized
			}
			return c.JSON(finalCode, map[string]interface{}{
				"type":   "validation_error",
				"title":  "Request validation failed",
				"status": finalCode,
				"detail": err.Message,
			})
		},
	}

	return oapimiddleware.OapiRequestValidatorWithOptions(swagger, validatorOptions), nil
}
