using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.WSA;

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
            UIManager.Instance.BroadcastMessage("Please enter your name");
        }

    }

    private void LoginCallback(string resultData)
    {
        LoginResponseData response = JsonUtility.FromJson<LoginResponseData>(resultData);
        if (response.Success)
        {
            Debug.Log(response);
        }
        else
        {
            UIManager.Instance.ShowToastMessage(response.ErrMessage);
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
