using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class GavinVision : MonoBehaviour
{
    public float fov;  // The radius of the arc 
    [Range(0, 360)] public float fovAngle; // the angle of the arc
    private Vector3 source; // a variable used only for drawing the arc. Not important
    private float maxRayDistance = 12;
    public bool enemiesDetected = false;
    public List<GameObject> enemiesInArc = new List<GameObject>();
    public List<GameObject> enemiesInSight = new List<GameObject>();

    private void Update()
    {
        enemiesInArc.Clear();
        // create a list with all the colliders that are present in a round area around the player with radius fov
        Collider[] targetsInFov = Physics.OverlapSphere(transform.position, fov);
        // iterate through all of them and see which one of them are enemies
        foreach (Collider c in targetsInFov)
        {
            if (c.CompareTag("Enemy"))
            {
                float signedAngle = Vector3.Angle(transform.forward, c.transform.position - transform.position);
                // check if the enemy is also in the arc area
                if (Mathf.Abs(signedAngle) < fovAngle / 2)
                {
                    enemiesInFov = true;    
                    enemiesInArc.Add(c.gameObject);
                }
            }
        }

        // if the enemy is in the arc area check is he is also in sight. If so playerInSight will be set to true
        Debug.Log(enemiesInArc.Count);
        if (enemiesInArc.Count > 0)
        {
            bool enemyInSight;
            foreach (GameObject enemy in enemiesInArc)
            {
                enemyInSight = CheckWithRayCasting(enemy);
                if (enemyInSight)
                {
                    enemiesDetected = true;
                    if (!enemiesInSight.Contains(enemy))
                    {
                        enemiesInSight.Add(enemy);
                    }
                }

            }
        }
        else
        {
            enemiesInSight.Clear();
        }
    }
    // a function that returns true or false depending on if the enemy is in sight, when he already is in the arc area
    private bool CheckWithRayCasting(GameObject c)
    {
        Vector3 direction = c.transform.position - transform.position;
        Physics.Raycast(transform.position, direction, out RaycastHit hit, maxRayDistance);
        if (hit.rigidbody)
        {
            if (hit.rigidbody.tag == "Enemy")
            {
                return true;
            }
            return false;
        }
        else
        {
            return false;
        }
    }

    // code to draw the arc on the scene in unity
    private void OnDrawGizmos()
    {
        source = Quaternion.AngleAxis(-fovAngle / 2, transform.up) * transform.forward;
        Color c = Color.green;
        Handles.color = c;
        Handles.DrawSolidArc(transform.position, transform.up, source, fovAngle, fov);
    }
}
