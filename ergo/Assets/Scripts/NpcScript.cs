using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NpcScript : MonoBehaviour
{
    private bool isBound = false; // 当前的绑定状态
    private Transform playerTransform; // 玩家对象的Transform组件的引用

    // 设置绑定状态
    public void SetBoundState(bool state)
    {
        isBound = state;
        // 如果NPC被绑定，则获取玩家的Transform组件
        if (isBound)
        {
            playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        }
    }

    void Update()
    {
        // 如果NPC与玩家绑定，则跟随玩家移动
        if (isBound)
        {
            transform.position = playerTransform.position + Vector3.right * 2;//此处修改绑定位置
        }
    }
}
