package game

import (
	"encoding/json"
	"log"

	"github.com/gorilla/websocket"
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

func (p *Player) SendMessage(message string) {
	err := p.Conn.WriteMessage(websocket.TextMessage, []byte(message))
	if err != nil {
		log.Println("Error sending message to player", p.ID, err)
		p.Conn.Close()
	}
}

func (p *Player) SendDataMessage(message interface{}) {
	msgBytes, err := json.Marshal(message)
	if err != nil {
		log.Println("Error marshalling data message to player", p.ID, err)
		return
	}

	if err := p.Conn.WriteMessage(websocket.TextMessage, msgBytes); err != nil {
		log.Println("Error sending data message to player")
	}
}
