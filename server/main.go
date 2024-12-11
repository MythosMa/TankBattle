package main

import (
	"log"

	"tank-game-server/game"
	"tank-game-server/server"
)

func main() {
	log.Println("Starting server...")

	game.NewGame()
	server.Start()
}
