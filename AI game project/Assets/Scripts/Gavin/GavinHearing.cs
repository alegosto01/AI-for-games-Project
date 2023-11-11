
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
    public bool enemiesHeard = false;
    public bool enemiesAroundMe = false;
    public List<GameObject> enemiesInArc = new List<GameObject>();
    public List<GameObject> enemiesNextToMe = new List<GameObject>();
    public float timer = 0;
    public float gapTime = 0.5f;
    public Gavin gavinScript;

    public void Awake() {
        gavinScript = GetComponentInParent<Gavin>();
    }

    private void Update() {
        timer += Time.deltaTime;
        if(timer > gapTime) {
            CheckNoise();
            timer = 0;
        }
    }
    // a function that returns true or false depending on if the enemy is in sight, when he already is in the arc area
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

    // code to draw the arc on the scene in unity
    private void OnDrawGizmos() {
        source = Quaternion.AngleAxis(-fovAngle / 2, transform.up) * transform.forward;
        Color c = Color.blue;
        Handles.color = c;
        Handles.DrawSolidArc(transform.position, transform.up, source, fovAngle, fov);
    }

    public void CheckNoise() {
        enemiesInArc.Clear();

        Collider[] targetsInFov = Physics.OverlapSphere(transform.position, fov);
        foreach (Collider c in targetsInFov)
        {
            if (c.CompareTag("Enemy"))
            {
                enemiesInArc.Add(c.gameObject);
                enemiesHeard = true;
                if(!gavinScript.stealth) {

                    gavinScript.SwitchToStealh();
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
            if(gavinScript.stealth) {

                gavinScript.SwitchToRun();
            }

            enemiesNextToMe.Clear();
            enemiesHeard = false;
        }
    }
}
