using System.IO.Compression;
using UnityEngine;

public class Player
{
    private string playerName;
    private string inputDirection;

    private int tankIndex;
    private float positionX;
    private float positionZ;

    private GameObject tank;

    public Player()
    {
        positionX = 0;
        positionZ = 0;
    }

    public void SetPlayerName(string name)
    {
        playerName = name;
    }

    public string GetPlayerName()
    {
        return playerName;
    }

    public void SetInputDirection(string direction)
    {
        inputDirection = direction;
    }

    public string GetInputDirection()
    {
        return inputDirection;
    }

    public void SetTank(int index, GameObject tankObj)
    {
        tankIndex = index;
        tank = tankObj;
    }

    public GameObject GetTank()
    {
        return tank;
    }

    public void SetPosition(float positionX, float positionZ)
    {
        this.positionX = positionX;
        this.positionZ = positionZ;
    }

    public void Update()
    {
        UpdatePosition();
        UpdateRotation();

    }

    void UpdatePosition()
    {
        tank.transform.position = new Vector3(positionX, tank.transform.position.y, positionZ);
    }

    void UpdateRotation()
    {
        switch (inputDirection)
        {
            case InputDirection.Up:
                tank.transform.rotation = Quaternion.Euler(0, 0, 0);
                break;
            case InputDirection.Down:
                tank.transform.rotation = Quaternion.Euler(0, 180, 0);
                break;
            case InputDirection.Left:
                tank.transform.rotation = Quaternion.Euler(0, 270, 0);
                break;
            case InputDirection.Right:
                tank.transform.rotation = Quaternion.Euler(0, 90, 0);
                break;
        }
    }
}