package server

import (
	"log"
	"net/http"
	"net/url"

	"github.com/gorilla/websocket"

	"tank-tank-battle-server/game"
)

var gameInstance = game.NewGame()

var upgrader = websocket.Upgrader{
	CheckOrigin: func(r *http.Request) bool { return true },
}

func handleWebSocket(w http.ResponseWriter, r *http.Request) {
	conn, err := upgrader.Upgrade(w, r, nil)

	if err != nil {
		log.Println("WebSocket upgrade error:", err)
		return
	}
	defer conn.Close()
	log.Println("WebSocket connection established")

	queryParams, err := url.ParseQuery(r.URL.RawQuery)

	if err != nil {
		log.Println("Error parsing query parameters:", err)
		return
	}

	playerID := queryParams.Get(("userid"))

	if playerID == "" {
		log.Println("Player ID not provided")
		return
	}
	gameInstance.AddPlayer(playerID, conn)

	for {
		_, message, err := conn.ReadMessage()
		if err != nil {
			log.Println("WebSocket read error:", err)
			gameInstance.RemovePlayer(playerID)
			break
		}

		log.Printf("Received WebSocket message: %s", message)

		gameInstance.Broadcast(playerID + ":" + string(message))
	}
}
