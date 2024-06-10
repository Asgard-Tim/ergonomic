using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class walk5 : MonoBehaviour
{
    public float moveSpeed; // 物体移动速度

    private bool isWalking = true; // 是否正在行走的标志
    private bool end = false;
    private float distanceTraveled = 0f; // 已经移动的距离
    private float steplong1 = 6f; // 每一步的距离

    public Animator animator;
    private NavMeshAgent agent;
    public Vector3 point = new Vector3(51f, 1.5f, 0f);

    void Start()
    {
        animator = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
    }

    void Update()
    {
        if (isWalking)
        {
            // 向前移动
            moveSpeed = animator.GetFloat("Speed");
            transform.Translate(Vector3.forward * moveSpeed * Time.deltaTime);
            distanceTraveled += moveSpeed * Time.deltaTime;
            if (distanceTraveled >= steplong1)
            {
                isWalking = false;
                end = true;
                distanceTraveled = 0f;
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
        if (other.gameObject.CompareTag("New VFX")) // 检查碰撞对象是否为 New VFX
        {
            Debug.Log("Collided with New VFX!");
            animator.SetBool("isRun", true);
            animator.SetBool("isWalk", false);
            isWalking = false; // 停止向前移动
            moveSpeed = 3f;
            agent.SetDestination(point); // 设置导航目标为 point
            agent.speed = moveSpeed; // 设置导航代理的速度为跑步速度
        }
    }
}
