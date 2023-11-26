using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

public enum AlertStage
{
    Peaceful,
    Intrigued,
    Alerted
}

public class EnemySight : MonoBehaviour
{
    public float fov;  // The radius of the arc 
    [Range(0, 360)] public float fovAngle; // the angle of the ark
    private Vector3 source; // a variable used only for drawing the ark. Not important
    public AlertStage alertStage; 
    [Range(0, 100)] public float alertLevel;
    public bool chase; // when the agent is in alert stage then this is true. We use it to go from patrol to chase
    private Collider player; 
    private float maxRayDistance = 7;
    ChaseState chaseState; // the ChaseStae component of the Chase State object

    public GameObject manager;
    private GameManager gameManager;
    public GameObject gavin;
    Gavin gavinScript;
    public bool noiseHeard;

    private void Awake()
    {
        alertStage = AlertStage.Peaceful;
        alertLevel = 0;
    }

    private void Start()
    {
        chaseState = gameObject.GetComponentInChildren<ChaseState>();
        gameManager = manager.GetComponent<GameManager>();
        gavinScript = gavin.GetComponent<Gavin>();
    }

    private void Update()
    {
        chase = false;
        bool playerInFov = false; // if the player is in the arc area
        bool playerInSight = false; // if at the same time the player is not blocked by a wall or something else

        // create a list with all the colliders that are present in a round area around the agent with radius fov
        Collider[] targetsInFov = Physics.OverlapSphere(transform.position, fov);
        // iterate through all of them and if one of them is the player set playerInFov to true
        foreach (Collider c in targetsInFov)
        {
            if (c.CompareTag("Player"))
            {
                float signedAngle = Vector3.Angle(transform.forward, c.transform.position - transform.position);
                // check if the player is also in the arc area
                if (Mathf.Abs(signedAngle) < fovAngle / 2)
                {
                    playerInFov = true;
                    player = c;
                }
                break;
            }
        }

        // if the player is in the arc area check is he is also in sight. If so playerInSight will be set to true
        if (playerInFov)
        {
            playerInSight = CheckWithRayCasting(player);
        }

        // check if the player was heard
        noiseHeard = HearingControl();

        // update the alert stage according to if the player is in sight or not
        UpdateAlertStage(playerInSight, noiseHeard);

        // if the agent is alerted then the chase will begin (chase=true) and the radius of the arc will be much bigger
        if (alertStage == AlertStage.Alerted)
        {
            chase = true;
        }
    }    
    // a function that returns true or false depending on if the player is in sight, when he already is in the arc area
    private bool CheckWithRayCasting(Collider c)
    {
        Vector3 direction = c.transform.position - transform.position;
        Physics.Raycast(transform.position, direction, out RaycastHit hit, maxRayDistance);
        if (hit.rigidbody)
        {
            if (hit.rigidbody.tag == "Player")
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

    bool HearingControl()
    {
        bool noiseHeard = false;
        if (gameManager.soundExists)
        {
            if (Vector3.Distance(gameManager.lastSoundPosition, transform.position) <= gavinScript.noiseFov)
            {
                noiseHeard = true;
            }
        }
        return noiseHeard;
    }

    // a function that updates the alert stage depending on if the player is in sight or not
    private void UpdateAlertStage(bool playerInSight, bool isThereNoise)
    {
        switch (alertStage)
        {
            case AlertStage.Peaceful:
                // if the alert stage is currently peaceful and the player is in sight (or heard) the alert stage should be updated to intrigued
                if (playerInSight || isThereNoise)
                {
                    alertStage = AlertStage.Intrigued;
                }
                break;
            case AlertStage.Intrigued:
                // if the alert stage is currently intrigued and the player is in sight (or heard) the alert level should start to increase
                if (playerInSight || isThereNoise)
                {
                    alertLevel += 200*Time.deltaTime;
                    // if the alert lever is equal to or more than 100 then the alert stage should be updated to alerted
                    if (alertLevel >= 100)
                    {
                        alertStage = AlertStage.Alerted;
                    }
                }
                // if the alert stage is currently intrigued and the player is not in sight (or heard) the alert level should start to decrease
                else
                {
                    alertLevel -= 200*Time.deltaTime;
                    // if the alert lever is equal to or smaller than 0 then the alert stage should be updated back to peacuful
                    if (alertLevel <= 0)
                    {
                        alertStage = AlertStage.Peaceful;
                    }
                }
                break;
            case AlertStage.Alerted:
                /* if the alert stage is currently alerted and the player is not in sight (or heard), and at the same time the agent is where he saw the player for
                the last time then the alert stage should be updated back to intrigued */
                if (!chaseState.canSeeThePlayer && chaseState.agentInDestination && !isThereNoise)
                {
                    alertStage = AlertStage.Intrigued;
                }
                break;
        }
    }

    // code to draw the arc on the scene in unity
    private void OnDrawGizmos()
    {
        source = Quaternion.AngleAxis(-fovAngle / 2, transform.up) * transform.forward;
        Color c = Color.green;
        if (alertStage == AlertStage.Intrigued)
        {
            c = Color.Lerp(Color.green, Color.red, alertLevel / 100f);
        }
        else if (alertStage == AlertStage.Alerted)
        {
            c = Color.red;
        }
        Handles.color = c;
        Handles.DrawSolidArc(transform.position, transform.up, source, fovAngle, fov);
    }
}
