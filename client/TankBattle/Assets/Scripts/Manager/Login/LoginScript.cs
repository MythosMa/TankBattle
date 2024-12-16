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
            var data = new { playerName = playerName };
            WebSocketManager.Instance.SendRequestAsync(WebSocketCommand.LOGIN, JsonUtility.ToJson(data), LoginCallback);
        }
        else
        {
            ToastNotification.Show("Please enter your name");
        }

    }

    private void LoginCallback(string resultData)
    {
        Debug.Log(resultData);
    }

    [Serializable]
    private class LoginResponseData
    {
        public string playerName;
        public string errMessage;
        public bool success;
    }

}
