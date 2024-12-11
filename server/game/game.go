package game

import (
	"log"
)

type Game struct {
	Players map[string]*Player
}

func NewGame() *Game {
	return &Game{
		Players: make(map[string]*Player),
	}
}

func (g *Game) AddPlayer(id string) {
	g.Players[id] = NewPlayer(id)
	log.Printf("Player %s joined the game", id)
}

func (g *Game) RemovePlayer(id string) {
	delete(g.Players, id)
	log.Printf("Player %s left the game", id)
}

func (g *Game) Broadcast(message string) {
	for _, player := range g.Players {
		player.SendMessage(message)
	}
}
