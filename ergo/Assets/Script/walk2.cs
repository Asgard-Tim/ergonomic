using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using System.IO;

public class walk2 : MonoBehaviour
{
    public float moveSpeed; // 物体移动速度

    private bool isWalking = true; // 是否正在行走的标志
    private bool step1 = true; // 是否向前移动的标志
    private bool step2 = false; // 是否向左转的标志
    private bool step3 = false; // 是否向后移动的标志
    private bool end = false;
    private bool trig = false;
    private float distanceTraveled = 0f; // 已经移动的距离
    private float steplong1 = 5f; // 已经移动的距离
    private float steplong2 = 3f; // 已经移动的距离
    private float steplong3 = 12f; // 已经移动的距离


    private bool isBound = false; // 当前的绑定状态
    private Transform playerTransform; // 玩家对象的Transform组件的引用

    public Animator animator;
    private NavMeshAgent agent;
    public Vector3 point2 = new Vector3(51f, 1.5f, 0f);

    public string fireTag = "Fire"; // 火源的标签

    private bool previousBoundState = false; // 上一个绑定状态

    // 设置绑定状态
    public void SetBoundState(bool state)
    {
        isBound = state;
        // 如果NPC被绑定，则获取玩家的Transform组件
        if (isBound)
        {
            playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        }

        // Check if the binding state has changed from unbound to bound
        if (isBound && !previousBoundState)
        {
            WriteSpeedToFile(moveSpeed); // Write movement speed to file
        }

        previousBoundState = isBound;
    }

    void Start()
    {
        animator = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
    }

    void Update()
    {
        // 如果NPC与玩家绑定，则跟随玩家移动
        if ((Vector3.Distance(transform.position, FindClosestFire().transform.position) < 5 || Vector3.Distance(transform.position, FindSecondClosestFire().transform.position) < 5) && animator.GetBool("isRun") == false)
        {
            //Debug.Log("Near fire!");
            animator.SetBool("isRun", true);
            animator.SetBool("isWalk", false);
            isWalking = false; // 停止向前移动
            moveSpeed = 2.8f;
            agent.SetDestination(point2); // 设置导航目标为 point
            agent.speed = moveSpeed; // 设置导航代理的速度为跑步速度
            trig = true;
        }
        if(trig==true)
        {
            animator.SetBool("isRun", true);
        }
        // 如果NPC与玩家绑定，则跟随玩家移动
        if (isBound)
        {
            transform.position = playerTransform.position + Vector3.right * 2;//此处修改绑定位置
            isWalking = false;
            animator.SetBool("isRun", false);
            animator.SetBool("isWalk", false);
            end = true;
        }
        if (isWalking)
        {
            moveSpeed = animator.GetFloat("Speed");
            if (step1)
            {
                // 向前移动
                transform.Translate(Vector3.forward * moveSpeed * Time.deltaTime);
                distanceTraveled += moveSpeed * Time.deltaTime;
                if (distanceTraveled >= steplong1)
                {
                    step1 = false;
                    step2 = true;
                    transform.Rotate(Vector3.up, 90f);
                    distanceTraveled = 0f;
                }
            }
            else if (step2)
            {
                // 向左转

                transform.Translate(Vector3.forward * moveSpeed * Time.deltaTime);
                distanceTraveled += moveSpeed * Time.deltaTime;
                if (distanceTraveled >= steplong2)
                {
                    step2 = false;
                    step3 = true;
                    distanceTraveled = 0f;
                    transform.Rotate(Vector3.up, 90f);
                }
            }
            else if (step3)
            {
                // 向后移动

                transform.Translate(Vector3.forward * moveSpeed * Time.deltaTime);
                distanceTraveled += moveSpeed * Time.deltaTime;
                if (distanceTraveled >= steplong3)
                {
                    step3 = false;
                    end = true;
                    distanceTraveled = 0f;
                }
            }
        }
        if (end)
        {
            animator.SetBool("isWalk", false);
        }
        if (agent.remainingDistance <= agent.stoppingDistance && animator.GetBool("isRun"))
        {
            // 当代理到达目标点时
            animator.SetBool("isRun", false);
            animator.SetBool("isWalk", false);
            //Debug.Log("Reached destination!");
            // 在这里可以执行到达目标点后的操作
        }
    }

    // 找到最近的火源
    GameObject FindClosestFire()
    {
        GameObject[] fires = GameObject.FindGameObjectsWithTag(fireTag);
        GameObject closestFire = null;
        float closestDistance = Mathf.Infinity;
        foreach (GameObject fire in fires)
        {
            float distance = Vector3.Distance(transform.position, fire.transform.position);
            if (distance < closestDistance)
            {
                closestFire = fire;
                closestDistance = distance;
            }
        }
        return closestFire;
    }

    // 找到第二近的火源
    GameObject FindSecondClosestFire()
    {
        GameObject[] fires = GameObject.FindGameObjectsWithTag(fireTag);
        GameObject closestFire = null;
        float closestDistance = Mathf.Infinity;
        GameObject secondClosestFire = null;
        float secondClosestDistance = Mathf.Infinity;
        foreach (GameObject fire in fires)
        {
            float distance = Vector3.Distance(transform.position, fire.transform.position);
            if (distance < closestDistance)
            {
                secondClosestFire = closestFire;
                secondClosestDistance = closestDistance;
                closestFire = fire;
                closestDistance = distance;
            }
            else if (distance < secondClosestDistance)
            {
                secondClosestFire = fire;
                secondClosestDistance = distance;
            }
        }
        return secondClosestFire;
    }

    // 当该物体与其他碰撞器发生碰撞时被调用
    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("fire1") || other.gameObject.CompareTag("fire2")) // 检查碰撞对象是否为 New VFX
        {
            if (isBound == false)
            {
                Debug.Log("Collided with New VFX!");
                animator.SetBool("isRun", true);
                animator.SetBool("isWalk", false);
                isWalking = false; // 停止向前移动
                moveSpeed = 3f;
                agent.SetDestination(point2); // 设置导航目标为 point
                agent.speed = moveSpeed; // 设置导航代理的速度为跑步速度
            }
        }
    }

    void WriteSpeedToFile(float speed)
    {
        // Specify the file path where you want to write the speed
        string filePath = Application.dataPath + "/bind_data.txt";

        using (StreamWriter writer = new StreamWriter(filePath, true))
        {
            writer.WriteLine(speed);
        }

        Debug.Log("Speed of NPC: " + speed);
    }
}
