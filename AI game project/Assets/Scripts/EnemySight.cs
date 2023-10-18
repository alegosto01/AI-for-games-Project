using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public enum AlertStage
{
    Peaceful,
    Intrigued,
    Alerted
}

public class EnemySight : MonoBehaviour
{
    public float fov;
    [Range(0, 360)] public float fovAngle;
    private Vector3 source;
    public AlertStage alertStage;
    [Range(0, 100)] public float alertLevel;
    public static bool chase;
    public Collider player;

    private void Awake()
    {
        alertStage = AlertStage.Peaceful;
        alertLevel = 0;
    }

    private void Update()
    {
        bool playerInFov = false;
        bool playerInSight = false;
        Collider[] targetsInFov = Physics.OverlapSphere(transform.position, fov);
        foreach (Collider c in targetsInFov)
        {
            if (c.CompareTag("Player"))
            {
                playerInFov = true;
                break;
            }
        }
        if (playerInFov)
        {
            playerInSight = ChekWithRayCasting(player);
        }
        UpdateAlertStage(playerInSight);

        if (alertStage == AlertStage.Alerted)
        {
            chase = true;
        }
    }

    private bool ChekWithRayCasting(Collider c)
    {
        Vector3 direction = c.transform.position - transform.position;
        Physics.Raycast(transform.position, direction, out RaycastHit hit);
        Debug.Log(hit.rigidbody);
        if (hit.rigidbody)
        {
            
            return true;
        }
        else
        {
            return false;
        }
    }

    private void UpdateAlertStage(bool playerInSight)
    {
        switch (alertStage)
        {
            case AlertStage.Peaceful:
                if (playerInSight)
                {
                    alertStage = AlertStage.Intrigued;
                }
                break;
            case AlertStage.Intrigued:
                if (playerInSight)
                {
                    alertLevel += 100*Time.deltaTime;
                    if (alertLevel >= 100)
                    {
                        alertStage = AlertStage.Alerted;
                    }
                }
                else
                {
                    alertLevel -= 100*Time.deltaTime;
                    if (alertLevel <= 0)
                    {
                        alertStage = AlertStage.Peaceful;
                    }
                }
                break;
        }
    }

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
