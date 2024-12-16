package server

import (
	"encoding/json"
	"log"
	"net/http"

	"github.com/gorilla/websocket"

	"tank-tank-battle-server/game"
)

type Message struct {
	Command   string
	RequestId string
	Data      string
}

var gameInstance = game.NewGame()

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

	for {
		_, message, err := conn.ReadMessage()
		if err != nil {
			log.Println("WebSocket read error:", err)
			gameInstance.RemovePlayer(player)
			break
		}

		log.Println("WebSocket message:", string(message))

		var msg Message
		if err := json.Unmarshal(message, &msg); err != nil {
			log.Println("WebSocket message unmarshal error:", err)
			continue
		}

		log.Println("WebSocket message data:", msg)
		HandleRequestData(msg, player)

	}
}

func HandleRequestData(message Message, player *game.Player) {
	switch message.Command {
	case CommandLogin:
		HandleLogin(message, player)
	}
}

// login ===============
type LoginData struct {
	PlayerName string
}

func HandleLogin(message Message, player *game.Player) {
	var data LoginData
	if err := json.Unmarshal([]byte(message.Data), &data); err != nil {
		log.Println("WebSocket message unmarshal error:", err)
		return
	}

	log.Println("WebSocket message data:", data)

	success := false
	errMessage := ""
	ok := gameInstance.CheckHasPlayer(data.PlayerName)
	if ok {
		player.SetPlayerName(data.PlayerName)
		gameInstance.AddPlayer(player)
		success = true
	} else {
		errMessage = "Player name already exists"
	}
	player.SendDataMessage(
		map[string]interface{}{
			"command":  message.Command,
			"requesId": message.RequestId,
			"data": map[string]interface{}{
				"playerName": data.PlayerName,
				"success":    success,
				"errMessage": errMessage,
			},
		},
	)
}
