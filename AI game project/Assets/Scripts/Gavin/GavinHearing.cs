
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class GavinHearing : MonoBehaviour
{
    public float fov;  // The radius of the arc 
    [Range(0, 360)] public float fovAngle; // the angle of the arc
    private Vector3 source; // a variable used only for drawing the arc. Not important
    private float maxRayDistance = 12;
    public bool enemiesHeard = false; // if enemies are in the circle but behind a wall
    public bool enemiesAroundMe = false; // if enemies are in the circle and there is no obstacle between gavin and them
    public List<GameObject> enemiesInArc = new List<GameObject>(); // enemies in the circle of hearing
    public List<GameObject> enemiesNextToMe = new List<GameObject>(); // enemies in the circle of hearing without any obstacle in between
    public float timer = 0;
    public float gapTime = 0.5f;
    public Gavin gavinScript;
    public DecisionMaking decisionMaking;

    public bool onDrawGizmos = false;

    public void Awake() {
        gavinScript = GetComponentInParent<Gavin>();
        decisionMaking = GetComponent<DecisionMaking>();
    }

    private void Update() {
        timer += Time.deltaTime;
        if(timer > gapTime) {
            CheckNoise();
            timer = 0;
        }
    }
    // a function that returns true or false depending on if the enemy is covered by a wall or not, when he already is in the arc area
    private bool CheckWithRayCasting(GameObject c) {
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

    //code to draw the arc on the scene in unity
    private void OnDrawGizmos()
    {
        if (onDrawGizmos)
        {
            source = Quaternion.AngleAxis(-fovAngle / 2, transform.up) * transform.forward;
            Color c = Color.blue;
            Handles.color = c;
            Handles.DrawSolidArc(transform.position, transform.up, source, fovAngle, fov);
        }
        
    }


    public void CheckNoise() {
        enemiesInArc.Clear();

        // find all the enemies in the circle
        Collider[] targetsInFov = Physics.OverlapSphere(transform.position, fov);
        foreach (Collider c in targetsInFov)
        {
            if (c.CompareTag("Enemy"))
            {
                enemiesInArc.Add(c.gameObject);
                enemiesHeard = true;
                // if gavin hear enemies will go to stealth mode
                if(!gavinScript.stealth && !decisionMaking.runAway) {

                    gavinScript.SwitchToStealth();
                }
            }
        }


        if (enemiesInArc.Count > 0)
        {
            bool enemyNearMe;
            foreach (GameObject enemy in enemiesInArc)
            {
                enemyNearMe = CheckWithRayCasting(enemy);
                if (enemyNearMe)
                {
                    enemiesAroundMe = true;

                    if (!enemiesNextToMe.Contains(enemy))
                    {
                        enemiesNextToMe.Add(enemy);
                    }
                }
            }
            if(enemiesNextToMe.Count == 0) {
                enemiesAroundMe = false;
            }
        }
        else
        {
            //if there are no more enemies in the circle gavin goes back to run mode
            if(gavinScript.stealth) {

                gavinScript.SwitchToRun();
            }

            

            enemiesNextToMe.Clear();
            enemiesHeard = false;
        }

        if (decisionMaking.runAway)
        {
            gavinScript.SwitchToRunAway();
        }
    }
}
