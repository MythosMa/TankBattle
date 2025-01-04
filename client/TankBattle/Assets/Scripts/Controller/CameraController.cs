using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    // 单例模式
    public static CameraController Instance;
    // 主摄像机
    [SerializeField] Camera mainCamera;
    // 边缘阈值
    private float edgeThreshold = 0.1f;
    // 场景边界最小值
    public Vector2 sceneBoundsMin;
    // 场景边界最大值
    public Vector2 sceneBoundsMax;

    private void Awake()
    {
        // 如果实例为空，则创建实例，并设置 DontDestroyOnLoad
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            // 如果实例已存在，则销毁当前对象
            Destroy(gameObject);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        // 设置场景边界最小值和最大值
        sceneBoundsMin = new Vector2(-100, -100);
        sceneBoundsMax = new Vector2(100, 100);
    }

    // Update is called once per frame
    void LateUpdate()
    {


        // 如果游戏正在运行，则跟随玩家
        if (GameController.Instance.isGameRunning)
        {
            LookAtPlayer();
        }
        else
        {
            // 否则，跟随其他对象
            LookAtOthers();
        }



    }

    // 跟随玩家
    private void LookAtPlayer()
    {
        // 获取玩家位置
        Vector3 targetPosition = PlayerModel.Instance.GetPlayer().GetPosition();
        // 跟随目标
        FollowTarget(targetPosition);
    }

    // 跟随其他对象
    private void LookAtOthers()
    {

    }

    // 跟随目标
    private void FollowTarget(Vector3 lookAtPosition)
    {
        // 将目标位置转换为屏幕坐标
        Vector3 targetInScreenPos = mainCamera.WorldToViewportPoint(lookAtPosition);
        // 获取摄像机位置
        Vector3 cameraPos = mainCamera.transform.position;

        // 获取摄像机宽高比
        float aspectRation = mainCamera.aspect;
        // 获取摄像机视场角
        float fov = mainCamera.fieldOfView;
        // 获取摄像机与目标之间的距离
        float cameraDistance = Mathf.Abs(cameraPos.y - lookAtPosition.y);
        // 计算摄像机视场高度
        float viewHeight = 2 * cameraDistance * Mathf.Tan(fov * 0.5f * Mathf.Deg2Rad);
        float viewWidth = viewHeight * aspectRation;



        if (targetInScreenPos.x < edgeThreshold)
        {
            // 如果目标在屏幕左边缘，则摄像机向右移动
            float deltaX = (edgeThreshold - targetInScreenPos.x) * viewWidth;
            cameraPos.x -= deltaX;
            // Debug.Log($"deltaX: {deltaX}, edgeThreshold: {edgeThreshold}, targetInScreenPos.x: {targetInScreenPos.x}, viewWidth: {viewWidth}, aspectRation: {aspectRation}");
        }
        else if (targetInScreenPos.x > 1 - edgeThreshold)
        {
            // 如果目标在屏幕右边缘，则摄像机向左移动
            float deltaX = (targetInScreenPos.x - (1 - edgeThreshold)) * viewWidth;
            cameraPos.x += deltaX;
        }

        if (targetInScreenPos.y < edgeThreshold)
        {
            // 如果目标在屏幕下边缘，则摄像机向上移动
            float deltaZ = (edgeThreshold - targetInScreenPos.y) * viewHeight;
            cameraPos.z -= deltaZ;
        }
        else if (targetInScreenPos.y > 1 - edgeThreshold)
        {
            // 如果目标在屏幕上边缘，则摄像机向下移动
            float deltaZ = (targetInScreenPos.y - (1 - edgeThreshold)) * viewHeight;
            cameraPos.z += deltaZ;
        }

        // 限制摄像机位置在场景边界内
        cameraPos.x = Mathf.Clamp(cameraPos.x, sceneBoundsMin.x, sceneBoundsMax.x);
        cameraPos.z = Mathf.Clamp(cameraPos.z, sceneBoundsMin.y, sceneBoundsMax.y);

        Debug.Log($"Target In Screen: {targetInScreenPos}, Camera Position: {cameraPos}, Target Position: {lookAtPosition}");

        mainCamera.transform.position = cameraPos;
    }
}