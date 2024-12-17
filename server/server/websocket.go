package server

import (
	"log"
	"net/http"

	"github.com/gorilla/websocket"

	"tank-tank-battle-server/game"
)

var upgrader = websocket.Upgrader{
	CheckOrigin: func(r *http.Request) bool { return true },
}

func HandleWebSocket(w http.ResponseWriter, r *http.Request) {
	conn, err := upgrader.Upgrade(w, r, nil)

	if err != nil {
		log.Println("WebSocket upgrade error:", err)
		return
	}
	defer conn.Close()
	log.Println("WebSocket connection established")

	player := game.NewPlayer(conn)
	player.ReceiveDataMessage()
}
