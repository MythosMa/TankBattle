package main

import (
	"log"

	"tank-game-server/game"
	"tank-game-server/server"
)

func main() {
	log.Println("Starting server...")

	game := game.NewGame()

	server.Start()
}
