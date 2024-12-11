package server

import (
	"log"
	"net/http"

	"github.com/gorilla/websocket"

	"tank-game-server/game"
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

	playerID := r.RemoteAddr
	gameInstance.AddPlayer(playerID, conn)

	for {
		messageType, message, err := conn.ReadMessage()
		if err != nil {
			log.Println("WebSocket read error:", err)
			break
		}

		log.Printf("Received WebSocket message: %s", message)

		err = conn.WriteMessage(messageType, message)
		if err != nil {
			log.Println("WebSocket write error:", err)
			gameInstance.RemovePlayer(playerID)
			break
		}

		gameInstance.Broadcast(string(message))
	}
}
