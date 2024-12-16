package game

type Game struct {
	Players map[string]*Player
}

func NewGame() *Game {
	return &Game{
		Players: make(map[string]*Player),
	}
}

func (g *Game) AddPlayer(player *Player) {
	g.Players[player.ID] = player
	g.Broadcast("Player " + player.ID + " joined the game")
}

func (g *Game) RemovePlayer(player *Player) {
	if player.ID != "" {
		delete(g.Players, player.ID)
		g.Broadcast("Player " + player.ID + " left the game")
	}
}

func (g *Game) Broadcast(message string) {
	for _, player := range g.Players {
		player.SendMessage(message)
	}
}

func (g *Game) CheckHasPlayer(PlayerName string) bool {
	_, ok := g.Players[PlayerName]
	return ok
}
