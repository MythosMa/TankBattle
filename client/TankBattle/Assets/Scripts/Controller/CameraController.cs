using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] Camera mainCamera;
    private float cameraOffset = 10f;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (GameController.Instance.isGameRunning)
        {
            LookAtPlayer();
        }
        else
        {
            LookAtOthers();
        }



    }

    private void LookAtPlayer()
    {
        Vector3 cameraPosition = mainCamera.transform.position;
        Vector3 playerPosition = PlayerModel.Instance.GetPlayer().GetPosition();
        string playerDirection = PlayerModel.Instance.GetPlayer().GetCurrentDirection();

        if (playerDirection == InputDirection.Up)
        {

        }
    }

    private void LookAtOthers()
    {

    }
}
