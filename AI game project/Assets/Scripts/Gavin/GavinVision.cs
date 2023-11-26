using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

public class GavinVision : MonoBehaviour
{
    public float fov;  // The radius of the arc 
    [Range(0, 360)] public float fovAngle; // the angle of the arc
    private Vector3 source; // a variable used only for drawing the arc. Not important
    private float maxRayDistance = 12;
    public bool enemiesDetected = false;
    public List<GameObject> enemiesInArc = new List<GameObject>();
    public List<GameObject> enemiesInSight = new List<GameObject>();
    public float timer = 0;
    public float gapTime = 0.5f;

    [SerializeField] GameObject manager;
    private GameManager gameManager;

    private void Start()
    {
        gameManager = manager.GetComponent<GameManager>();
    }

    private void Update()
    {
        timer += Time.deltaTime;
        if (timer > gapTime)
        {
            CheckVision();
            timer = 0;
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

    public void CheckVision()
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
                    //enemiesInFov = true;    
                    enemiesInArc.Add(c.gameObject);

                }
            }
        }

        // if the enemy is in the arc area check is he is also in sight. If so playerInSight will be set to true
        //Debug.Log(enemiesInArc.Count);
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
        else  // if the arc area is empty we still want the enemies previously detected to be considered in sight, unless if they are too far away or
        // behind a wall
        {
            List<GameObject> enemiesToDelete = new List<GameObject>() { };
            foreach (GameObject enemy in enemiesInSight)
            {
                if (!CheckWithRayCasting(enemy))
                {
                    enemiesToDelete.Add(enemy);
                }
            }
            foreach (GameObject enemy in enemiesToDelete)
            {
                enemiesInSight.Remove(enemy);
            }
            if (enemiesInSight.Count == 0)
            {
                enemiesDetected = false;
            }
        }

        // if gavin is under attack we consider that he sees the enemy closest to him (which will be the one attacking him)
        if (gameManager.gavinUnderAttack)
        {
            List<Tuple<float, GameObject>> distances = new List<Tuple<float, GameObject>>();
            foreach (GameObject enemy in gameManager.enemies)
            {
                distances.Add(new Tuple<float, GameObject>(Vector3.Distance(enemy.transform.position, transform.position), enemy));
            }

            if (distances.Count > 0)
            {
                (float minDistance, GameObject nearestEnemy) = distances[0];

                foreach ((float distance, GameObject enemy) in distances)
                {
                    if (distance < minDistance)
                    {
                        minDistance = distance;
                        nearestEnemy = enemy;
                    }
                }

                if (!enemiesInSight.Contains(nearestEnemy))
                {
                    enemiesInSight.Add(nearestEnemy);
                    enemiesDetected = true;
                }
            }


        }
    }
}
