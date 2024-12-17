package game

import (
	"sync"

	"tank-tank-battle-server/config"
)

type Game struct {
	Players map[string]*Player
	mu      sync.Mutex
}

var gameInstance *Game
var once sync.Once

// GetGameInstance 返回单例的 Game 实例
func GetGameInstance() *Game {
	once.Do(func() {
		gameInstance = NewGame()
	})
	return gameInstance
}

// NewGame 创建一个新的 Game 实例
func NewGame() *Game {
	return &Game{
		Players: make(map[string]*Player),
	}
}

// AddPlayer 将玩家添加到游戏中
func (g *Game) AddPlayer(player *Player) {
	g.mu.Lock()
	defer g.mu.Unlock()
	g.Players[player.ID] = player
	g.Broadcast("Player " + player.ID + " joined the game")
}

// RemovePlayer 从游戏中移除玩家
func (g *Game) RemovePlayer(player *Player) {
	g.mu.Lock()
	defer g.mu.Unlock()
	if player.ID != "" {
		delete(g.Players, player.ID)
		g.Broadcast("Player " + player.ID + " left the game")
	}
}

// Broadcast 向所有玩家广播消息
func (g *Game) Broadcast(message string) {
	for _, player := range g.Players {
		player.SendDataMessage(map[string]interface{}{
			"Command": config.CommandNormalMessage,
			"Data":    message,
		})
	}
}

// CheckHasPlayer 检查玩家是否存在
func (g *Game) CheckHasPlayer(PlayerName string) bool {
	g.mu.Lock()
	defer g.mu.Unlock()
	_, ok := g.Players[PlayerName]
	return ok
}
