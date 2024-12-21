using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public static PlayerController Instance;
    private bool _isPlayerControllerStart = false;

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
        HandlePlayerInput();
    }

    void HandlePlayerInput()
    {
        if (GameController.Instance.isGameRunning && _isPlayerControllerStart)
        {
            float horizontalInput = Input.GetAxisRaw("Horizontal");
            float verticalInput = Input.GetAxisRaw("Vertical");
            string inputDirection = InputDirection.None;
            if (horizontalInput != 0 && verticalInput == 0)
            {
                inputDirection = horizontalInput > 0 ? InputDirection.Right : InputDirection.Left;
            }
            else if (verticalInput != 0 && horizontalInput == 0)
            {
                inputDirection = verticalInput > 0 ? InputDirection.Up : InputDirection.Down;
            }
            PlayerModel.Instance.SetInputDirection(inputDirection);
        }
    }

    public void StartController()
    {
        _isPlayerControllerStart = true;
    }
}
