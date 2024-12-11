package game

import (
	"log"

	"github.com/gorilla/websocket"
)

type Player struct {
	ID        string
	Conn      *websocket.Conn
	DataModel *PlayerGameDataModel
}

func NewPlayer(id string, conn *websocket.Conn) *Player {
	return &Player{
		ID:   id,
		Conn: conn,
		DataModel: &PlayerGameDataModel{
			X: 0,
			Y: 0,
		},
	}
}

func (p *Player) SendMessage(message string) {
	err := p.Conn.WriteMessage(websocket.TextMessage, []byte(message))
	if err != nil {
		log.Println("Error sending message to player", p.ID, err)
		p.Conn.Close()
	}
}
