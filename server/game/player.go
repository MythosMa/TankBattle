package game

import (
	"log"

	"github.com/gorilla/websocket"
)

type Player struct {
	ID   string
	Conn *websocket.Conn
}

func NewPlayer(id string, conn *websocket.Conn) *Player {
	return &Player{
		ID:   id,
		Conn: conn,
	}
}

func (p *Player) SendMessage(message string) {
	err := p.Conn.WriteMessage(websocket.TextMessage, []byte(message))
	if err != nil {
		log.Println("Error sending message to player", p.ID, err)
		p.Conn.Close()
	}
}
