using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gavin : MonoBehaviour
{
    void Start()
    {
        
    }

    void Update()
    {
        if (Input.GetKey(KeyCode.UpArrow))
        {
            transform.position += new Vector3(0,0,4) * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.DownArrow))
        {
            transform.position += new Vector3(0,0,-4) * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.RightArrow))
        {
            transform.position += new Vector3(4,0,0) * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            transform.position += new Vector3(-4,0,0) * Time.deltaTime;
        }
        if(transform.position.x > 5) {
            Debug.Log("You won!");
        }
        
    }
}
