package game

import (
	"encoding/json"
	"log"
	"sync"
	"time"

	"tank-tank-battle-server/constants"
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
	game := &Game{
		Players: make(map[string]*Player),
	}
	go game.HandleGameLoop()
	return game
}

func (g *Game) HandleGameLoop() {
	ticker := time.NewTicker(time.Millisecond * 33) // 每 33ms 执行一次，即每秒 30 帧
	defer ticker.Stop()
	for {
		<-ticker.C // 等待下一个周期到来
		g.BroadGameData()
	}
}

func (g *Game) BroadGameData() {
	var playerModels []PlayerDataModel = make([]PlayerDataModel, 0)
	for _, player := range g.Players {
		playerModel := player.GetPlayerModel()
		if playerModel != nil {
			playerModels = append(playerModels, *playerModel)
		}
	}

	if len(playerModels) == 0 {
		return
	}

	var gameData = GameDataModel{
		PlayerDataModels: playerModels,
	}

	jsonData, err := json.Marshal(gameData)
	if err != nil {
		log.Println("Error marshaling player data:")
		return
	}

	for _, player := range g.Players {
		player.SendDataMessage(map[string]interface{}{
			"Command": constants.CommandGameData,
			"Data":    string(jsonData),
		})
	}
}

// AddPlayer 将玩家添加到游戏中
func (g *Game) AddPlayer(player *Player) {
	g.mu.Lock()
	defer g.mu.Unlock()
	g.Players[player.PlayerName] = player
	g.Broadcast("Player " + player.PlayerName + " joined the game")
}

// RemovePlayer 从游戏中移除玩家
func (g *Game) RemovePlayer(player *Player) {
	g.mu.Lock()
	defer g.mu.Unlock()
	if player.PlayerName != "" {
		delete(g.Players, player.PlayerName)
		g.Broadcast("Player " + player.PlayerName + " left the game")
	}
}

// Broadcast 向所有玩家广播消息
func (g *Game) Broadcast(message string) {
	for _, player := range g.Players {
		player.SendDataMessage(map[string]interface{}{
			"Command": constants.CommandNormalMessage,
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

type GameDataModel struct {
	PlayerDataModels []PlayerDataModel
}
