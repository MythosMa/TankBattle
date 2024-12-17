package game

import (
	"encoding/json"
	"log"

	"github.com/gorilla/websocket"

	"tank-tank-battle-server/config"
)

type Player struct {
	ID        string
	Conn      *websocket.Conn
	DataModel *PlayerGameDataModel
}

func NewPlayer(conn *websocket.Conn) *Player {
	return &Player{
		Conn: conn,
	}
}

func (p *Player) SetPlayerName(id string) {
	p.ID = id
}

// server =============

type Message struct {
	Command   string
	RequestId string
	Data      string
}

func (p *Player) ReceiveDataMessage() {
	for {
		_, message, err := p.Conn.ReadMessage()
		if err != nil {
			log.Println("WebSocket read error:", err)
			GetGameInstance().RemovePlayer(p)
			break
		}

		var msg Message
		if err := json.Unmarshal(message, &msg); err != nil {
			log.Println("WebSocket message unmarshal error:", err)
			continue
		}

		p.HandleRequestData(msg)
	}
}

func (p *Player) HandleRequestData(message Message) {
	switch message.Command {
	case config.CommandLogin:
		p.HandleLogin(message)
	}
}

func (p *Player) SendDataMessage(message interface{}) {
	msgBytes, err := json.Marshal(message)
	if err != nil {
		log.Println("Error marshalling data message to player", p.ID, err)
		return
	}

	log.Println("Sending data message to player", p.ID, string(msgBytes))

	if err := p.Conn.WriteMessage(websocket.TextMessage, msgBytes); err != nil {
		log.Println("Error sending data message to player")
	}
}

// login ===============
type LoginData struct {
	PlayerName string
}

func (p *Player) HandleLogin(message Message) {
	var data LoginData
	if err := json.Unmarshal([]byte(message.Data), &data); err != nil {
		log.Println("WebSocket message unmarshal error:", err)
		return
	}

	success := false
	errMessage := ""
	ok := GetGameInstance().CheckHasPlayer(data.PlayerName)

	if ok {
		errMessage = "Player name already exists"
	} else {
		p.SetPlayerName(data.PlayerName)
		GetGameInstance().AddPlayer(p)
		success = true
	}

	dataJson, err := json.Marshal(map[string]interface{}{
		"PlayerName": data.PlayerName,
		"Success":    success,
		"ErrMessage": errMessage,
	})

	if err != nil {
		log.Println("WebSocket message marshal error:", err)
		return
	}

	p.SendDataMessage(
		map[string]interface{}{
			"Command":   message.Command,
			"RequestId": message.RequestId,
			"Data":      string(dataJson),
		},
	)
}
