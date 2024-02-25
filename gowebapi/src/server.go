package main

import (
	"github.com/brianvoe/gofakeit/v6"
	"github.com/labstack/echo/v4"
	"github.com/labstack/echo/v4/middleware"
	"net/http"
	"os"
)

func main() {

	e := echo.New()

	e.Use(middleware.Logger())
	e.Use(middleware.Recover())

	e.GET("/", func(c echo.Context) error {
		return c.HTML(http.StatusOK, "Hello, from gowebapi!")
	})

	e.GET("/health", func(c echo.Context) error {
		return c.JSON(http.StatusOK, struct{ Status string }{Status: "OK"})
	})

	e.GET("/podcast", func(c echo.Context) error {
		gofakeit.Seed(0)
		podcast := struct {
			Title    string
			HostedBy string
		}{
			Title:    gofakeit.Sentence(3),
			HostedBy: gofakeit.Name(),
		}

		return c.JSON(http.StatusOK, podcast)
	})

	httpPort := os.Getenv("PORT")
	if httpPort == "" {
		httpPort = "8082"
	}

	e.Logger.Fatal(e.Start(":" + httpPort))
}
