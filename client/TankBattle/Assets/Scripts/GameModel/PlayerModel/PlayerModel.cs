using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerModel : MonoBehaviour
{
    public static PlayerModel Instance;
    // Start is called before the first frame update

    private Player player;

    private bool isPlayerModelUpdate = false;

    private PlayerModelRequestData playerModelRequestData;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            player = new Player();
            playerModelRequestData = new PlayerModelRequestData();
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Update()
    {
        if (GameController.Instance.isGameRunning && isPlayerModelUpdate)
        {
            isPlayerModelUpdate = false;
            var PlayerModelInfo = new PlayerModelRequestData
            {
                PlayerName = player.GetPlayerName(),
                InputDirection = player.GetInputDirection()
            };
            HandleSendPlayerModelMessage(PlayerModelInfo);
        }

    }

    public void SetPlayerName(string name)
    {
        player.SetPlayerName(name);
        isPlayerModelUpdate = true;
    }

    public void SetPlayerTank(int index, GameObject tank)
    {
        player.SetTank(index, tank);
    }

    public void SetInputDirection(string direction)
    {
        if (player.GetInputDirection() != direction)
        {
            player.SetInputDirection(direction);
            isPlayerModelUpdate = true;
        }
    }

    public Player GetPlayer()
    {
        return player;
    }

    // 处理数据上传
    private void HandleSendPlayerModelMessage(PlayerModelRequestData data)
    {
        WebSocketManager.Instance.SendNormalData(WebSocketCommand.PLAYER_MODEL, JsonUtility.ToJson(data));
    }

    [Serializable]
    public class PlayerModelRequestData
    {
        public string PlayerName;
        public string InputDirection;

        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }
            var other = (PlayerModelRequestData)obj;
            return PlayerName == other.PlayerName && InputDirection == other.InputDirection;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(PlayerName, InputDirection);
        }
    }

    public class PlayerModelResponseData
    {
        public string PlayerName;
        public string Direction;
        public int TankIndex;
        public float PositionX;
        public float PositionZ;
    }
}
