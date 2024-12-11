package game

import (
	"log"

	"github.com/gorilla/websocket"
)

type Game struct {
	Players map[string]*Player
}

func NewGame() *Game {
	return &Game{
		Players: make(map[string]*Player),
	}
}

func (g *Game) AddPlayer(id string, conn *websocket.Conn) {
	g.Players[id] = NewPlayer(id, conn)
	log.Printf("Player %s joined the game", id)
	g.Broadcast("Player " + id + " joined the game")
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
