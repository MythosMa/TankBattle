package game

import (
	"encoding/json"
	"log"
	"math/rand"
	"sync"
	"time"

	"github.com/gorilla/websocket"

	"tank-tank-battle-server/constants"
)

type Player struct {
	PlayerName          string
	Conn                *websocket.Conn
	DataModel           *PlayerDataModel
	playerRunning       bool
	stopLoopMu          sync.Mutex
	IsPlayerModelUpdate bool
}

func NewPlayer(conn *websocket.Conn) *Player {
	return &Player{
		Conn: conn,
	}
}

func (p *Player) SetPlayerName(playerName string) {
	p.PlayerName = playerName
	p.DataModel = &PlayerDataModel{
		PlayerName: playerName,
		Direction:  constants.InputDirectionNone,
		TankIndex:  rand.Intn(4),
		PositionX:  0,
		PositionZ:  0,
	}
	p.StartGameLoop()
}

// server =============

type Message struct {
	Command   string
	RequestId string
	Data      string
}

func (p *Player) StartPlayer() {
	go p.ReceiveDataMessage()
}

func (p *Player) StartGameLoop() {
	p.playerRunning = true
	p.IsPlayerModelUpdate = true
	go p.GameLoop()
}

// 处理游戏逻辑==================
func (p *Player) GameLoop() {
	ticker := time.NewTicker(time.Millisecond * 33) // 每 33ms 执行一次，即每秒 30 帧
	defer ticker.Stop()

	for {
		<-ticker.C // 等待下一个周期到来
		p.Update()

		// 检查是否要停止循环
		p.stopLoopMu.Lock()
		if !p.playerRunning {
			p.stopLoopMu.Unlock()
			return // 退出循环
		}
		p.stopLoopMu.Unlock()
	}
}

func (p *Player) Update() {
	direction := p.DataModel.Direction
	switch direction {
	case constants.InputDirectionNone:
		return
	case constants.InputDirectionUp:
		p.DataModel.PositionZ += 0.1
	case constants.InputDirectionDown:
		p.DataModel.PositionZ -= 0.1
	case constants.InputDirectionLeft:
		p.DataModel.PositionX -= 0.1
	case constants.InputDirectionRight:
		p.DataModel.PositionX += 0.1
	}
	p.IsPlayerModelUpdate = true
}

func (p *Player) GetPlayerModel() *PlayerDataModel {
	if p.IsPlayerModelUpdate {
		// p.IsPlayerModelUpdate = false
		return p.DataModel
	}
	return nil
}

// 处理websocket信息=============
func (p *Player) ReceiveDataMessage() {
	for {
		_, message, err := p.Conn.ReadMessage()
		if err != nil {
			log.Println("WebSocket read error:", err)
			p.stopLoopMu.Lock()
			p.playerRunning = false
			p.stopLoopMu.Unlock()
			GetGameInstance().RemovePlayer(p)
			p.Conn.Close()
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
	log.Println("player receive data:", message)
	switch message.Command {
	case constants.CommandLogin:
		p.HandleLogin(message)
	case constants.CommandPlayerModel:
		p.HandlePlayerModel(message)
	}
}

func (p *Player) SendDataMessage(message interface{}) {
	msgBytes, err := json.Marshal(message)
	if err != nil {
		log.Println("Error marshalling data message to player", p.PlayerName, err)
		return
	}

	if err := p.Conn.WriteMessage(websocket.TextMessage, msgBytes); err != nil {
		log.Println("Error sending data message to player")
	}
}

// playerModel ===============
func (p *Player) HandlePlayerModel(message Message) {
	var data PlayerModel
	if err := json.Unmarshal([]byte(message.Data), &data); err != nil {
		log.Println("WebSocket player model unmarshal error:", err)
		return
	}

	p.DataModel.PlayerName = data.PlayerName
	p.DataModel.Direction = data.InputDirection
}

// login ===============
type LoginData struct {
	PlayerName string
}

func (p *Player) HandleLogin(message Message) {
	var data LoginData
	if err := json.Unmarshal([]byte(message.Data), &data); err != nil {
		log.Println("WebSocket login unmarshal error:", err)
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

	log.Println("SendDataMessage:", string(dataJson))

	p.SendDataMessage(
		map[string]interface{}{
			"Command":   message.Command,
			"RequestId": message.RequestId,
			"Data":      string(dataJson),
		},
	)
}

// 玩家数据
type PlayerModel struct {
	PlayerName     string
	InputDirection string
}
