using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform playerTransform;
    public Vector3 camerachange = new Vector3(0, 2, 0); // 后上方的偏移量
    public float mouseSensitivity = 100.0f;
    public float minYAngle = -30.0f; // 最小垂直旋转角度
    public float maxYAngle = 30.0f; // 最大垂直旋转角度

    private float verticalAngle = 0.0f; // 记录当前垂直旋转角度

    void Start()
    {
        playerTransform = GameObject.Find("Player").transform;
    }

    void LateUpdate()
    {
        // 相机跟随玩家的位置
        transform.position = playerTransform.position + camerachange;

        // 获取鼠标Y轴输入并反转方向
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        // 更新垂直旋转角度
        verticalAngle -= mouseY; // 反转方向
        verticalAngle = Mathf.Clamp(verticalAngle, minYAngle, maxYAngle);

        // 应用垂直旋转
        transform.localEulerAngles = new Vector3(verticalAngle, 0.0f, 0.0f);
    }
}
