using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class walk : MonoBehaviour
{

    public float moveSpeed; // 物体移动速度

    private bool step1 = true; // 是否向前移动的标志
    private bool step2 = false; // 是否向左转的标志
    private bool step3 = false; // 是否向后移动的标志
    private bool step4 = false; // 是否向右转的标志
    private bool end = false;
    private float distanceTraveled = 0f; // 已经移动的距离
    private float steplong1 = 8f; // 已经移动的距离
    private float steplong2 = 10f; // 已经移动的距离
    private float steplong3 = 8f; // 已经移动的距离
    private float steplong4 = 12f; // 已经移动的距离

    public Animator animator;
    // Start is called before the first frame update
    void Start()
    {
        animator = this.GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
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
                transform.Rotate(Vector3.up, -90f);
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
                transform.Rotate(Vector3.up, 180f);
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
                step4 = true;
                distanceTraveled = 0f;
                transform.Rotate(Vector3.up, -90f);
            }
        }
        else if (step4)
        {
            // 向左转
            
            transform.Translate(Vector3.forward * moveSpeed * Time.deltaTime);
            distanceTraveled += moveSpeed * Time.deltaTime;
            if (distanceTraveled >= steplong4)
            {
                step4 = false;
                end = true;
                distanceTraveled = 0f;
            }
        }
        else if (end)
        {
            Destroy(animator);
        }
    }
}
