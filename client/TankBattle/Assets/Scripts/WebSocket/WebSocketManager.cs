using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using NativeWebSocket;
using System.Threading.Tasks;
using System;
using UnityEditor;

public class WebSocketManager : MonoBehaviour
{

    public static WebSocketManager Instance;
    private WebSocket ws;
    // Start is called before the first frame update
    private string wsUrl = "ws://localhost:3000/ws";

    private Dictionary<string, WebSocketPromise> pendingPromises = new Dictionary<string, WebSocketPromise>();

    async void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        await InitWebSocket();
    }

    async Task InitWebSocket()
    {
        ws = new WebSocket(wsUrl);

        ws.OnOpen += () =>
        {
            Debug.Log("Connection open!");
        };

        ws.OnError += (e) =>
        {
            Debug.Log("Error! " + e);
        };

        ws.OnClose += (e) =>
        {
            Debug.Log("Connection closed!");
        };

        ws.OnMessage += (bytes) =>
        {
            string message = System.Text.Encoding.UTF8.GetString(bytes);
            HandleServerMessage(message);
        };

        await ws.Connect();
    }

    // Update is called once per frame
    void Update()
    {
#if !UNITY_WEBGL || !UNITY_EDITOR
        ws.DispatchMessageQueue();
#endif
    }

    public void SendNormalData(string command, string data)
    {
        var message = new ServerMessage
        {
            Command = command,
            Data = data,
        };
        string jsonMessage = JsonUtility.ToJson(message);

        SendingDataToServer(jsonMessage);
    }

    public void SendRequestAsync(string command, string data, Action<string> callback)
    {
        string requestId = Tools.GenerateId();

        var message = new ServerMessage
        {
            Command = command,
            RequestId = requestId,
            Data = data
        };

        WebSocketPromise promise = new WebSocketPromise(callback);
        pendingPromises.Add(requestId, promise);

        string jsonMessage = JsonUtility.ToJson(message);

        SendingDataToServer(jsonMessage);
    }

    private async void SendingDataToServer(string dataJson)
    {
        if (ws.State == WebSocketState.Open)
        {
            await ws.SendText(dataJson);
        }
        else
        {
            ToastNotification.Show("Network Error");
        }
    }

    private void HandleServerMessage(string message)
    {
        Debug.Log("Received message from server: " + message);
        var serverMessage = JsonUtility.FromJson<ServerMessage>(message);
        if (serverMessage.RequestId != null && pendingPromises.ContainsKey(serverMessage.RequestId))
        {
            pendingPromises[serverMessage.RequestId].SetResponse(serverMessage.Data);
            pendingPromises.Remove(serverMessage.RequestId);
        }
        else
        {
            DealDataWithCommand(serverMessage.Command, serverMessage.Data);
        }

    }

    private void DealDataWithCommand(string command, string data)
    {
        switch (command)
        {
            // TODO: handle commands
            case WebSocketCommand.NORMAL:
                UIManager.Instance.ShowToastMessage(data);
                break;
            default:
                break;
        }
    }

    async void OnApplicationQuit()
    {
        await ws.Close();
    }
}

[Serializable]
public class ServerMessage
{
    public string Command;
    public string Data;
    public string RequestId;
}


public class WebSocketPromise
{
    private TaskCompletionSource<string> tcs;
    private Action<string> callback;

    public WebSocketPromise(Action<string> cb)
    {
        tcs = new TaskCompletionSource<string>();
        callback = cb;
    }

    public Task<string> GetResponseTask()
    {
        return tcs.Task;
    }

    public void SetResponse(string response)
    {
        tcs.SetResult(response);
        if (callback != null)
        {
            callback(response);
        }
    }
}