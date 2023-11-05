using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Navigation : MonoBehaviour
{
    public List<Vector3> squares = new List<Vector3>();

    // Start is called before the first frame update
    void Start()
    {
        for(int i = -5; i < 5; i++) {
            for(int j = -5; j < 5; j++) {
                squares.Add(new Vector3(i+0.5f,0, j+0.5f));
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
