package game

import "log"

type Player struct {
	ID string
}

func NewPlayer(id string) *Player {
	return &Player{
		ID: id,
	}
}

func (p *Player) SendMessage(message string) {
	log.Printf("Sending message to player %s: %s", p.ID, message)
}
