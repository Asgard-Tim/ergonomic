using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class demo : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("I from demo");
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.P))
        {
            Debug.Log("P Get");
        }
        if (Input.GetKey(KeyCode.F))
        {
            this.transform.Translate(new Vector3(0, 0, 1 * Time.deltaTime));
            // Debug.Log("Forward");
        }
        if (Input.GetKey(KeyCode.B))
        {
            this.transform.Translate(new Vector3(0, 0, -1 * Time.deltaTime));
            // Debug.Log("Forward");
        }
        if (Input.GetKeyDown(KeyCode.P))
        {
            Debug.Log("P Down");
        }
        if (Input.GetKeyUp(KeyCode.P))
        {
            Debug.Log("P Up");
        }
        if (Input.GetMouseButton(0))
        {
            Debug.Log("Left Get");
        }
        if (Input.GetMouseButtonDown(2))
        {
            Debug.Log("Middle Down");
        }
        if (Input.GetMouseButtonUp(1))
        {
            Debug.Log("Right Up");
        }
    }

    void OnCollisionEnter()
    {
        Debug.Log("Collision");
    }
}
