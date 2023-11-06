using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Navigation : MonoBehaviour
{
    public float fov;
    public float radius;
    public List<Vector3> squares = new List<Vector3>();
    public List<Vector3> seenSquares = new List<Vector3>();
    

    // Start is called before the first frame update
    void Start()
    {
        //add all the squares of the map
        for(int i = -5; i < 5; i++) {
            for(int j = -5; j < 5; j++) {
                squares.Add(new Vector3(i+0.5f,0, j+0.5f));
            }
        }
        // store in seenSquares all the squares in the arch
        foreach (Vector3 point in squares) {
            PointInCircle(point, this.transform.position, radius, fov);
        }

        //ray cat to spot the walls
        Vector3 direction = transform.forward;
        for (float a = -fov / 2; a <= fov / 2; a += 1.0f)
        {
            Vector3 rayDirection = Quaternion.Euler(0, a, 0) * direction;
            RaycastHit hit;
            if (Physics.Raycast(transform.position, rayDirection, out hit, radius))
            {
                //hit a wall
                Debug.Log(hit.point);
            }
            else
            {
                //raycast with no collision
            }
        }
    }
    

    // Update is called once per frame
    void Update()
    {
        
    }


    public void PointInCircle(Vector3 posPunto, Vector3 gavinPos, float raggio, float fov) {
        
        float distance = Vector3.Distance(posPunto, gavinPos);

        if(distance < raggio) {

            Vector3 direction = posPunto - gavinPos;
            float angle = Vector3.Angle(transform.forward, direction);

            if(angle < fov/2) {
                seenSquares.Add(posPunto);
            }
            // float theta = Mathf.Atan( (posPunto.z - gavinPos.z) / (posPunto.x - gavinPos.x) );
            // float degTheta = theta * Mathf.Rad2Deg;
            // // if (theta < 0){
            // //     theta += 2 * 90;
            // // }
            // Quaternion rotation = Quaternion.Euler(gavinPos);
            // Quaternion pointRot = Quaternion.Euler(0,degTheta, 0);
            // Quaternion fovRot = Quaternion.Euler(0,fov, 0);

            // if (pointRot.Euler >= rotation * fovRot && pointRot <= rotation * fovRot) {
                
            // }
        }
    }

}
