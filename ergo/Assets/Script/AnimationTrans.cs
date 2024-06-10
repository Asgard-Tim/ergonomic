using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationTrans : MonoBehaviour
{
    private Animator animator;
    private Rigidbody rigid_body;
    private bool walk_key = false;
    private int walk_key_count = 0;
    // Start is called before the first frame update
    void Start()
    {
        animator = this.GetComponent<Animator>();
        rigid_body = this.GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKey(KeyCode.P))
        {
            walk_key_count = walk_key_count + 1;
        }
        else if(walk_key_count>5)
        {
            walk_key_count = 0;
            if (walk_key == true)
            {
                walk_key = false;
            }
            else
            {
                walk_key = true;
            }
        }
        else { walk_key_count = 0; }

        if (walk_key)
        {
            if (animator.GetBool("isWalk"))
            {
                animator.SetBool("isWalk", false);
                walk_key = false;
            }
            else
            {
                animator.SetBool("isWalk", true);
                walk_key = false;
            }
        }

        if (animator.GetBool("isWalk"))
        {
            //this.transform.Translate(new Vector3(0, 0, animator.GetFloat("Speed") * Time.deltaTime));
            this.rigid_body.MovePosition((new Vector3(0, 0, animator.GetFloat("Speed") * Time.deltaTime)+this.rigid_body.position));
        }
    }
}
