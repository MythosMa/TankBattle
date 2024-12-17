package main

import (
	"log"

	"tank-tank-battle-server/game"
	"tank-tank-battle-server/server"
)

func main() {
	log.Println("Starting server...")

	game.GetGameInstance()
	server.Start()
}
