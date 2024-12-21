using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public static GameController Instance;

    public bool isGameRunning = false;

    [SerializeField] List<GameObject> tankPrefabs;
    [SerializeField] GameObject gameContainer;

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

    public void CreatePlayerTank(Player player)
    {
        int playerTankIndex = player.GetTankIndex();
        GameObject tank = Instantiate(tankPrefabs[playerTankIndex], new Vector3(0, 0.15f, 0), tankPrefabs[playerTankIndex].transform.rotation, gameContainer.transform);
        player.SetTank(playerTankIndex, tank);
    }

    public void StartGame()
    {
        this.isGameRunning = true;
        UIController.Instance.SetLoginUIVisible(false);
        GameModel.Instance.AddPlayer(PlayerModel.Instance.GetPlayer());
    }

    public void EndGame()
    {
        this.isGameRunning = false;
    }
}
