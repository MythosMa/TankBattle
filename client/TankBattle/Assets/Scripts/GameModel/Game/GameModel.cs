using System;
using System.Collections.Generic;
using System.Linq;
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
        GameModelResponseData data = JsonUtility.FromJson<GameModelResponseData>(responseMessage);
        HashSet<string> playerDataNames = new HashSet<string>(data.PlayerDataModels.Select(playerData => playerData.PlayerName));
        HashSet<string> playerNames = new HashSet<string>(players.Select(player => player.GetPlayerName()));

        List<PlayerData> newPlayers = data.PlayerDataModels.Where(data => !playerNames.Contains(data.PlayerName)).ToList();
        List<Player> playersToRemove = players.Where(player => !playerDataNames.Contains(player.GetPlayerName())).ToList();
        List<PlayerData> existingPlayers = data.PlayerDataModels.Where(data => playerNames.Contains(data.PlayerName)).ToList();

        newPlayers.ForEach(playerData =>
        {
            Player newPlayer = new Player();

            GameController.Instance.CreatePlayerTank(newPlayer);
            newPlayer.SetPlayerName(playerData.PlayerName);
            newPlayer.SetPosition(playerData.PositionX, playerData.PositionZ);
            newPlayer.SetInputDirection(playerData.Direction);
            newPlayer.CreatePlayerObject();
            players.Add(newPlayer);
        });

        playersToRemove.ForEach(player =>
        {
            player.DestroyPlayerObject();
            Destroy(player.GetTank());
        });

        players.RemoveAll(player => playersToRemove.Exists(playerToRemove => playerToRemove == player));

        existingPlayers.ForEach(playerData =>
        {
            Player player = players.Find(p => p.GetPlayerName() == playerData.PlayerName);
            if (player != null)
            {
                if (player.IsPlayerObjectCreated())
                {
                    player.SetPosition(playerData.PositionX, playerData.PositionZ);
                    player.SetInputDirection(playerData.Direction);
                }
                else
                {
                    GameController.Instance.CreatePlayerTank(player);
                    player.SetPosition(playerData.PositionX, playerData.PositionZ);
                    player.SetInputDirection(playerData.Direction);
                    player.CreatePlayerObject();
                }
            }
        });


        // data.PlayerDataModels.ForEach(playerData =>
        // {
        //     Player player = players.Find(p => p.GetPlayerName() == playerData.PlayerName);
        //     player.SetPosition(playerData.PositionX, playerData.PositionZ);
        //     player.SetInputDirection(playerData.InputDirection);
        // });
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
        public string Direction;
        public int TankIndex;
        public float PositionX;
        public float PositionZ;
    }
}