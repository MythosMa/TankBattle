package main

import (
	"log"

	"tank-tank-battle-server/game"
	"tank-tank-battle-server/server"
)

func main() {
	log.Println("Starting server...")

	game.NewGame()
	server.Start()
}
