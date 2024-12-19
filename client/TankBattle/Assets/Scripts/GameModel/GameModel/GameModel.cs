using System;
using System.Collections.Generic;
using UnityEngine;

public class GameModel : MonoBehaviour
{
    public static GameModel Instance;

    private List<Player> players;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            players = new List<Player>();
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Update()
    {
        players.ForEach(player => player.Update());
    }

    public void AddPlayer(Player player)
    {
        players.Add(player);
    }

    public void HandleGameModelMessage(string responseMessage)
    {
        Debug.Log("HandleGameModelMessage:" + responseMessage);
        GameModelResponseData data = JsonUtility.FromJson<GameModelResponseData>(responseMessage);
        data.PlayerDataModels.ForEach(playerData =>
        {
            Player player = players.Find(p => p.GetPlayerName() == playerData.PlayerName);
            player.SetPosition(playerData.PositionX, playerData.PositionZ);
            player.SetInputDirection(playerData.InputDirection);
        });

    }

    [Serializable]
    public class GameModelResponseData
    {
        public List<PlayerData> PlayerDataModels;
    }

    [Serializable]
    public class PlayerData
    {
        public string PlayerName;
        public string InputDirection;
        public int TankIndex;
        public float PositionX;
        public float PositionZ;
    }
}