using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class PlayerScript : MonoBehaviour
{
    public GameObject npc1; // NPC对象的引用
    public GameObject npc2; // NPC对象的引用
    public GameObject npc3; // NPC对象的引用
    private GameObject nearestNpc; // 最近的NPC对象
    private bool isBoundToNearestNpc1 = false; // 是否与最近的NPC绑定
    private bool isBoundToNearestNpc2 = false; // 是否与最近的NPC绑定
    private bool isBoundToNearestNpc3 = false; // 是否与最近的NPC绑定
    private float nearestDistance = Mathf.Infinity; // 最近的NPC距离
    private Rigidbody playerRigidbody; // 玩家刚体组件

    void Start()
    {
        playerRigidbody = GetComponent<Rigidbody>(); // 获取玩家刚体组件
    }

    void Update()
    {
        nearestNpc = GetNearestNpc();

        if (Input.GetMouseButtonDown(0) || Input.GetButtonDown("RB"))
        {
            // 检测玩家与NPC的距离是否足够近以进行交互
            if (Vector3.Distance(transform.position, npc1.transform.position) < 5f && (nearestNpc == npc1 || nearestNpc == null) && isBoundToNearestNpc2 == false && isBoundToNearestNpc3 == false)
            {
                // 切换绑定状态
                isBoundToNearestNpc1 = !isBoundToNearestNpc1;
                if (isBoundToNearestNpc1)
                {
                    float distance = Vector3.Distance(transform.position, npc1.transform.position);
                    float playerSpeed = playerRigidbody.velocity.magnitude; // 获取玩家速度
                    Debug.Log("Bind Distance: " + distance + ", Player Speed: " + playerSpeed);
                    SaveBindData(distance, playerSpeed); // 保存绑定数据
                }
                // 通知NPC脚本绑定状态的改变
                npc1.GetComponent<walk1>().SetBoundState(isBoundToNearestNpc1);
                if (!isBoundToNearestNpc1)
                    return;
            }

            if (Vector3.Distance(transform.position, npc2.transform.position) < 5f && (nearestNpc == npc2 || nearestNpc == null) && isBoundToNearestNpc1 == false && isBoundToNearestNpc3 == false)
            {
                // 切换绑定状态
                isBoundToNearestNpc2 = !isBoundToNearestNpc2;
                if (isBoundToNearestNpc2)
                {
                    float distance = Vector3.Distance(transform.position, npc2.transform.position);
                    float playerSpeed = playerRigidbody.velocity.magnitude; // 获取玩家速度
                    Debug.Log("Bind Distance: " + distance + ", Player Speed: " + playerSpeed);
                    SaveBindData(distance, playerSpeed); // 保存绑定数据
                }
                // 通知NPC脚本绑定状态的改变
                npc2.GetComponent<walk2>().SetBoundState(isBoundToNearestNpc2);
                if (!isBoundToNearestNpc2)
                    return;
            }

            if (Vector3.Distance(transform.position, npc3.transform.position) < 5f && (nearestNpc == npc3 || nearestNpc == null) && isBoundToNearestNpc2 == false && isBoundToNearestNpc1 == false)
            {
                // 切换绑定状态
                isBoundToNearestNpc3 = !isBoundToNearestNpc3;
                if (isBoundToNearestNpc3)
                {
                    float distance = Vector3.Distance(transform.position, npc3.transform.position);
                    float playerSpeed = playerRigidbody.velocity.magnitude; // 获取玩家速度
                    Debug.Log("Bind Distance: " + distance + ", Player Speed: " + playerSpeed);
                    SaveBindData(distance, playerSpeed); // 保存绑定数据
                }
                // 通知NPC脚本绑定状态的改变
                npc3.GetComponent<walk3>().SetBoundState(isBoundToNearestNpc3);
                if (!isBoundToNearestNpc3)
                    return;
            }
        }
    }

    GameObject GetNearestNpc()
    {
        GameObject nearest = null;

        // 遍历所有NPC，找出最近的NPC
        foreach (GameObject npc in new GameObject[] { npc1, npc2, npc3 })
        {
            if (npc != null)
            {
                float distance = Vector3.Distance(transform.position, npc.transform.position);
                if (distance < nearestDistance)
                {
                    nearestDistance = distance;
                    nearest = npc;
                }
            }
        }

        return nearest;
    }

    void SaveBindData(float distance, float playerSpeed)
    {
        string filePath = Application.dataPath + "/bind_data.txt";

        // 打开文件流并设置为追加模式
        using (StreamWriter writer = new StreamWriter(filePath, true))
        {
            writer.Write(distance + " " + playerSpeed + " ");
        }
    }
}
