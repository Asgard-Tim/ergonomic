using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class walk4 : MonoBehaviour
{
    public float moveSpeed; // 物体移动速度

    private bool isWalking = true; // 是否正在行走的标志
    private bool step1 = true; // 是否向前移动的标志
    private bool step2 = false; // 是否向左转的标志
    private bool step3 = false; // 是否向后移动的标志
    private bool end = false;
    private float distanceTraveled = 0f; // 已经移动的距离
    private float steplong1 = 5.5f; // 已经移动的距离
    private float steplong2 = 6f; // 已经移动的距离
    private float steplong3 = 20f; // 已经移动的距离

    public Animator animator;
    private NavMeshAgent agent;
    public Vector3 point4 = new Vector3(37.5f, 1.5f, 30f);

    void Start()
    {
        animator = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
    }

    void Update()
    {
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
                    transform.Rotate(Vector3.up, -90f);
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
            Debug.Log("Reached destination!");
            // 在这里可以执行到达目标点后的操作
        }
    }

    // 当该物体与其他碰撞器发生碰撞时被调用
    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("fire1") || other.gameObject.CompareTag("fire2")) // 检查碰撞对象是否为 New VFX
        {
            Debug.Log("Collided with New VFX!");
            animator.SetBool("isRun", true);
            animator.SetBool("isWalk", false);
            isWalking = false; // 停止向前移动
            moveSpeed = 3f;
            agent.SetDestination(point4); // 设置导航目标为 point
            agent.speed = moveSpeed; // 设置导航代理的速度为跑步速度
        }
    }
}
