using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public static GameController Instance;

    public bool isGameRunning = false;

    private void Awake()
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
    }
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void StartGame()
    {
        this.isGameRunning = true;
        UIController.Instance.SetLoginUIVisible(false);
        PlayerController.Instance.CreatePlayerTank();

        GameModel.Instance.AddPlayer(PlayerModel.Instance.GetPlayer());
    }

    public void EndGame()
    {
        this.isGameRunning = false;
    }
}
