using System.Net.Cache;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;


public class Gavin : MonoBehaviour
{
    
    public GameManager gameManager;
    // moving variables
    public Vector3 lastPosition;

    public float noiseTimer = 0.5f;
    public float noiseFrequency = 0.5f;
    public bool stealth = false;

    //speed variables
    public float stealthSpeed = 2;
    public float runningSpeed = 3;
    public float gavinSpeed;

    //noise generation variables
    //public float makingNoisefrequency = 2;  //times per second
    public float noiseRadiusStealth = 2f;
    public float noiseRadiusRunning = 4f;
    public float noiseFov;
    private Vector3 noiseSource;
    public UnityEngine.AI.NavMeshAgent agent;

    public EnemyStateMachineManager manager;

    void Awake() {
        agent = GetComponent<UnityEngine.AI.NavMeshAgent>();
    }

    void Start()
    {
        agent.speed = runningSpeed;
        noiseFov = noiseRadiusRunning;
    }

    void Update()
    {
        //changing modality of moving either running or walking
        if (Input.GetKeyDown(KeyCode.S)) {
            stealth = true;
            gavinSpeed = stealthSpeed;
            noiseFov = noiseRadiusStealth;
        }
        if (Input.GetKeyDown(KeyCode.R)) {
            stealth = false;
            gavinSpeed = runningSpeed;
            noiseFov = noiseRadiusRunning;
        }

        Moving();

    
    }

    public void Moving() {
        //Controls of gavin movement

        //if is not moving change the variables "isMoving"   


        if (Input.GetKey(KeyCode.UpArrow))
        {
            transform.position += new Vector3(0,0,gavinSpeed) * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.DownArrow))
        {
            transform.position += new Vector3(0,0,-gavinSpeed) * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.RightArrow))
        {
            transform.position += new Vector3(gavinSpeed,0,0) * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            transform.position += new Vector3(-gavinSpeed,0,0) * Time.deltaTime;
        }
        
    }

    
    public void SwitchToStealth() {
        stealth = true;
        agent.speed = stealthSpeed;
        noiseFov = noiseRadiusStealth;

    }
    public void SwitchToRun() {
        stealth = false;
        agent.speed = runningSpeed;
        noiseFov = noiseRadiusRunning;
    }

    public void SwitchToRunAway()
    {
        stealth = false;
        agent.speed = 4f;
        noiseFov = noiseRadiusRunning;
    }



    // private void OnDrawGizmos()
    // {
    //     visionSourceRightForward = Quaternion.Euler(0, 25.0f, 0) * transform.forward;
    //     visionSourceLeftForward = Quaternion.Euler(0, -35.0f, 0) * transform.forward;
    //     visionSourceLeft = Quaternion.Euler(0, -70.0f, 0) * transform.forward;
    //     visionSourceForward = Quaternion.Euler(0, -5, 0) * transform.forward;
    //     visionSourceRight = Quaternion.Euler(0, 60f, 0) * transform.forward;
    //     // visionSource = Quaternion.AngleAxis(-visionFovAngle / 2, transform.up) * new Vector3(30,0,0);
    //     // visionSource = Quaternion.AngleAxis(-visionFovAngle / 2, transform.up) * new Vector3(30,0,0);
    //     // visionSource = Quaternion.AngleAxis(-visionFovAngle / 2, transform.up) * new Vector3(30,0,0);
    //     Color c = Color.red;

    //     Handles.color = c;
    //     Handles.DrawSolidArc(transform.position, transform.up, visionSourceRightForward, visionFovAngle, visionFov);
    //     Handles.DrawSolidArc(transform.position,transform.up, visionSourceLeftForward, visionFovAngle, visionFov);
    //     Handles.DrawSolidArc(transform.position,transform.up, visionSourceLeft, visionFovAngle, visionFov);
    //     Handles.DrawSolidArc(transform.position,transform.up, visionSourceForward, visionFovAngle, visionFov);
    //     Handles.DrawSolidArc(transform.position,transform.up, visionSourceRight, visionFovAngle, visionFov);

    //     noiseSource = Quaternion.AngleAxis(-noiseFovAngle / 2, transform.up) * transform.forward;
    //     Color soundColor = Color.blue;

    //     Handles.color = soundColor;
    //     Handles.DrawSolidArc(transform.position, transform.up, noiseSource, noiseFovAngle, noiseFov);
    // }
}
