using System;
using TMPro;
using UnityEngine;

public class LoginScript : MonoBehaviour
{
    public TMP_InputField playerNameInput;

    public void StartButtonClick()
    {
        string playerName = playerNameInput.text;
        if (playerName != "")
        {
            var data = new LoginRequestData { PlayerName = playerName };
            WebSocketManager.Instance.SendRequestAsync(WebSocketCommand.LOGIN, JsonUtility.ToJson(data), LoginCallback);
        }
        else
        {
            UIController.Instance.BroadcastMessage("Please enter your name");
        }

    }

    private void LoginCallback(string resultData)
    {
        LoginResponseData response = JsonUtility.FromJson<LoginResponseData>(resultData);
        if (response.Success)
        {
            PlayerModel.Instance.InitPlayerName(response.PlayerName);
            PlayerController.Instance.StartController();
            GameController.Instance.StartGame();
        }
        else
        {
            UIController.Instance.ShowToastMessage(response.ErrMessage);
        }

    }

    [Serializable]
    private class LoginRequestData
    {
        public string PlayerName;
    }

    [Serializable]
    private class LoginResponseData
    {
        public string PlayerName;
        public string ErrMessage;
        public bool Success;
    }

}
