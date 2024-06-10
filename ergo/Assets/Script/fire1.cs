using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class fire1 : MonoBehaviour
{
    public float num;
    public GameObject obj;
    private float timer = 0f;
    private float interval = 5f;

    void Start()
    {
        // 获取球类对象
        obj = GameObject.Find("fire2");
        num = 1f;
    }

    void Update()
    {
        timer += Time.deltaTime; // 计时器增加每帧的时间

        if (timer >= interval) // 如果时间到了间隔
        {
            SpreadFire(); // 执行扩散
            timer = 0f; // 重置计时器
        }
    }

    void SpreadFire()
    {
        // 克隆八个方向的对象
        GameObject clone1 = Instantiate(obj, obj.transform.position + Vector3.forward * num, obj.transform.rotation);
        GameObject clone2 = Instantiate(obj, obj.transform.position + Vector3.back * num, obj.transform.rotation);
        GameObject clone3 = Instantiate(obj, obj.transform.position + Vector3.left * num, obj.transform.rotation);
        GameObject clone4 = Instantiate(obj, obj.transform.position + Vector3.right * num, obj.transform.rotation);
        GameObject clone5 = Instantiate(obj, obj.transform.position + (Vector3.left + Vector3.forward).normalized * num, obj.transform.rotation);
        GameObject clone6 = Instantiate(obj, obj.transform.position + (Vector3.right + Vector3.forward).normalized * num, obj.transform.rotation);
        GameObject clone7 = Instantiate(obj, obj.transform.position + (Vector3.left + Vector3.back).normalized * num, obj.transform.rotation);
        GameObject clone8 = Instantiate(obj, obj.transform.position + (Vector3.right + Vector3.back).normalized * num, obj.transform.rotation);

        num += 1f;
    }
}