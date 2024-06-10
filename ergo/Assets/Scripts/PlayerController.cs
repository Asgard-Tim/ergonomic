using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class PlayerController : MonoBehaviour
{
    public float speed = 3.0f;
    public float jumpForce = 7.0f;
    public float mouseSensitivity = 100.0f;
    public float gamepadSensitivity = 100.0f;
    private Rigidbody rb;
    private bool isGrounded;
    private List<Vector3> trajectoryPoints = new List<Vector3>();
    private List<float> angles = new List<float>();
    private int collisionCount = 0;
    public Transform cameraTransform;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        // 锁定光标到屏幕中心
        Cursor.lockState = CursorLockMode.Locked;
        if (cameraTransform == null)
        {
            cameraTransform = GetComponentInChildren<Camera>().transform;
        }
    }

    void Update()
    {
        MovePlayer();
        RotatePlayer();

        RecordTrajectory();
        SaveTrajectoryToFile();
        RecordAngle();
        SaveAngle();
    }

    // InputSimulator 类模拟输入事件
    public static class InputSimulator
    {
        [System.Runtime.InteropServices.DllImport("user32.dll")]
        private static extern void mouse_event(int dwFlags, int dx, int dy, int cButtons, int dwExtraInfo);

        private const int MOUSEEVENTF_LEFTDOWN = 0x02; // 鼠标左键按下
        private const int MOUSEEVENTF_LEFTUP = 0x04; // 鼠标左键释放

        // 模拟鼠标左键按下事件
        public static void SimulateMouseButtonDown(int button)
        {
            if (button == 0) // 如果是鼠标左键
            {
                mouse_event(MOUSEEVENTF_LEFTDOWN, 0, 0, 0, 0);
            }
        }

        // 模拟鼠标左键释放事件
        public static void SimulateMouseButtonUp(int button)
        {
            if (button == 0) // 如果是鼠标左键
            {
                mouse_event(MOUSEEVENTF_LEFTUP, 0, 0, 0, 0);
            }
        }
    }

    void MovePlayer()
    {
        float moveHorizontal = Input.GetAxis("Horizontal");
        float moveVertical = Input.GetAxis("Vertical");

        // 尝试获取手柄输入，如果失败则默认为0
        float gamepadHorizontal = SafeGetAxis("GamepadHorizontal");
        float gamepadVertical = SafeGetAxis("GamepadVertical");

        // 调试日志以查看手柄输入值
        Debug.Log($"Gamepad Horizontal: {gamepadHorizontal}, Gamepad Vertical: {gamepadVertical}");

        // 反转手柄输入的垂直轴
        gamepadVertical *= -1f;

        // 将输入向量从世界空间转换为局部空间（相对于玩家的面朝方向）
        Vector3 moveInput = transform.right * (moveHorizontal + gamepadHorizontal) + transform.forward * (moveVertical + gamepadVertical);

        // 合并输入并归一化
        Vector3 movement = moveInput.normalized * speed;

        Debug.Log($"Movement Vector: {movement}");

        // 应用移动向量
        rb.velocity = new Vector3(movement.x, rb.velocity.y, movement.z);
    }

    void RotatePlayer()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        // 尝试获取手柄输入，如果失败则默认为0
        float gamepadX = SafeGetAxis("GamepadRightHorizontal") * gamepadSensitivity * Time.deltaTime;

        // 调试日志以查看手柄输入值
        Debug.Log($"Gamepad Right Horizontal: {gamepadX}");

        // 合并输入
        float rotationX = mouseX + gamepadX;

        // 只应用水平旋转到玩家对象
        transform.Rotate(Vector3.up * rotationX);

        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        // 只应用水平旋转到玩家对象
        cameraTransform.Rotate(Vector3.right * mouseY);
    }

    float SafeGetAxis(string axisName)
    {
        try
        {
            return Input.GetAxis(axisName);
        }
        catch (System.Exception)
        {
            Debug.LogWarning($"Input Axis {axisName} is not setup.");
            return 0f;
        }
    }

    void RecordTrajectory()
    {
        // 记录当前位置到轨迹列表
        trajectoryPoints.Add(transform.position);
    }

    void RecordAngle()
    {
        // 记录当前位置到轨迹列表
        Vector3 playerForward = transform.forward;
        float angleWithXAxis = Vector3.Angle(playerForward, Vector3.right);
        //Debug.Log("Angle between player forward direction and x-axis: " + angleWithXAxis + " degrees");
        angles.Add(angleWithXAxis);
    }

    void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
        }
        if (collision.gameObject.CompareTag("Walls"))
        {
            collisionCount++;
            Debug.Log("Collision count: " + collisionCount);
        }

        // 每次碰撞，增加碰撞计数器并打印
        
    }


    void SaveTrajectoryToFile()
    {
        // 检查是否有轨迹点需要保存
        if (trajectoryPoints.Count == 0)
            return;

        // 获取当前应用程序的数据路径
        string filePath = Application.dataPath + "/trajectory.txt";
        //print(filePath);

        // 打开文件流
        StreamWriter writer = new StreamWriter(filePath);

        // 将轨迹点写入文件
        foreach (Vector3 point in trajectoryPoints)
        {
            writer.WriteLine(point.x + "," + point.y + "," + point.z);
        }

        // 关闭文件流
        writer.Close();

        //Debug.Log("Trajectory points saved to: " + filePath);
    }

    void SaveAngle()
    {
        // 检查是否有轨迹点需要保存
        if (angles.Count == 0)
            return;

        // 获取当前应用程序的数据路径
        string filePath = Application.dataPath + "/angles.txt";
        //print(filePath);

        // 打开文件流
        StreamWriter writer = new StreamWriter(filePath);

        // 将轨迹点写入文件
        foreach (float point in angles)
        {
            writer.WriteLine(point);
        }

        // 关闭文件流
        writer.Close();

        //Debug.Log("Trajectory points saved to: " + filePath);
    }
}
