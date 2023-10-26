using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


public class Gavin : MonoBehaviour
{
    public GameManager gameManager;
    // moving variables
    public Vector3 lastPosition;

    public float noiseTimer = 0.25f;
    public bool isMoving = false;
    public bool stealth = false;

    //speed variables
    public float stealthSpeed = 2;
    public float runningSpeed = 4;
    public float gavinSpeed;

    //noise generation variables
    //public float makingNoisefrequency = 2;  //times per second
    public float noiseRadiusStealth = 1;
    public float noiseRadiusRunning = 2.5f;
    public float noiseFov;
    [Range(0, 360)] public float noiseFovAngle;
    private Vector3 noiseSource;

    //vision variables

    public float visionFov;
    [Range(0, 360)] public float visionFovAngle;
    private Vector3 visionSourceRightForward;
    private Vector3 visionSourceLeftForward;
    private Vector3 visionSourceForward;
    private Vector3 visionSourceRight;
    private Vector3 visionSourceLeft;

    public List<Vector3> previousPositions = new List<Vector3>();
    public EnemyStateMachineManager manager;
    public Vector3 startPosition;

    // public GameObject[] directions = new GameObject[5];
    // public ExploringDirections[] exploringDirections = new ExploringDirections[5];

    void Start()
    {
        gavinSpeed = runningSpeed;
        noiseFov = noiseRadiusRunning;
        previousPositions.Add(startPosition);  
        // exploringDirections[0] = directions[0].GetComponent<ExploringDirections>(); 
        // exploringDirections[1] = directions[1].GetComponent<ExploringDirections>(); 
        // exploringDirections[2] = directions[2].GetComponent<ExploringDirections>(); 
        // exploringDirections[3] = directions[3].GetComponent<ExploringDirections>(); 
        // exploringDirections[4] = directions[4].GetComponent<ExploringDirections>(); 
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

        if(transform.position.x > 5) {
            Debug.Log("You won!");
        }

        //Exploring();
        // if (playerInFov)
        // {
        //     playerInSight = ChekWithRayCasting(player);
        // }
        // UpdateAlertStage(playerInSight);

        // if (alertStage == AlertStage.Alerted)
        // {
        //     chase = true;
        // }
    }

    public void MakeNoise() {
        Debug.Log("make noise");
        manager.NewNoise(noiseFov);
    }

    public void Moving() {
        //Controls of gavin movement

        //if is not moving change the variables "isMoving"

        if(isMoving && GetComponent<Rigidbody>().velocity.magnitude == 0)
        {
            isMoving = false;
            gameManager.isGavinMoving = false;        
            noiseTimer = 0.25f;
        }        


        if (Input.GetKey(KeyCode.UpArrow))
        {
            isMoving = true;
            gameManager.isGavinMoving = true;
            transform.position += new Vector3(0,0,gavinSpeed) * Time.deltaTime;
            lastPosition = transform.position;
            noiseTimer -= 1 * Time.deltaTime;
            
            if(noiseTimer <= 0) {
                MakeNoise();
                noiseTimer = 0.25f;
            }
        }
        if (Input.GetKey(KeyCode.DownArrow))
        {
            isMoving = true;
            gameManager.isGavinMoving = true;
            transform.position += new Vector3(0,0,-gavinSpeed) * Time.deltaTime;
            lastPosition = transform.position;

            noiseTimer -= 1 * Time.deltaTime;
            
            if(noiseTimer <= 0) {
                MakeNoise();
                noiseTimer = 0.25f;
            }
        }
        if (Input.GetKey(KeyCode.RightArrow))
        {
            isMoving = true;
            gameManager.isGavinMoving = true;
            transform.position += new Vector3(gavinSpeed,0,0) * Time.deltaTime;
            lastPosition = transform.position;

            noiseTimer -= 1 * Time.deltaTime;
            
            if(noiseTimer <= 0) {
                MakeNoise();
                noiseTimer = 0.25f;
            }
        }
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            isMoving = true;
            gameManager.isGavinMoving = true;
            transform.position += new Vector3(-gavinSpeed,0,0) * Time.deltaTime;
            lastPosition = transform.position;

            noiseTimer -= 1 * Time.deltaTime;
            
            if(noiseTimer <= 0) {
                MakeNoise();
                noiseTimer = 0.25f;
            }
        }
        
    }

    // public void Exploring() {
    //     int directionCount = 0;
    //     directionCount += exploringDirections[0].isTriggered ? 0 : 1;
    //     directionCount += exploringDirections[1].isTriggered ? 0 : 1;
    //     directionCount -= exploringDirections[3].isTriggered ? 0 : 1;
    //     directionCount -= exploringDirections[4].isTriggered ? 0 : 1;

    //     Debug.Log(directionCount);
    //     Quaternion direction = Quaternion.Euler(0, 30.0f, 0);
    // }

    private void OnDrawGizmos()
    {
        visionSourceRightForward = Quaternion.Euler(0, 25.0f, 0) * transform.forward;
        visionSourceLeftForward = Quaternion.Euler(0, -35.0f, 0) * transform.forward;
        visionSourceLeft = Quaternion.Euler(0, -70.0f, 0) * transform.forward;
        visionSourceForward = Quaternion.Euler(0, -5, 0) * transform.forward;
        visionSourceRight = Quaternion.Euler(0, 60f, 0) * transform.forward;
        // visionSource = Quaternion.AngleAxis(-visionFovAngle / 2, transform.up) * new Vector3(30,0,0);
        // visionSource = Quaternion.AngleAxis(-visionFovAngle / 2, transform.up) * new Vector3(30,0,0);
        // visionSource = Quaternion.AngleAxis(-visionFovAngle / 2, transform.up) * new Vector3(30,0,0);
        Color c = Color.red;
        // if (alertStage == AlertStage.Intrigued)
        // {
        //     c = Color.Lerp(Color.green, Color.red, alertLevel / 100f);
        // }
        // else if (alertStage == AlertStage.Alerted)
        // {
        //     c = Color.red;
        // }
        Handles.color = c;
        Handles.DrawSolidArc(transform.position, transform.up, visionSourceRightForward, visionFovAngle, visionFov);
        Handles.DrawSolidArc(transform.position,transform.up, visionSourceLeftForward, visionFovAngle, visionFov);
        Handles.DrawSolidArc(transform.position,transform.up, visionSourceLeft, visionFovAngle, visionFov);
        Handles.DrawSolidArc(transform.position,transform.up, visionSourceForward, visionFovAngle, visionFov);
        Handles.DrawSolidArc(transform.position,transform.up, visionSourceRight, visionFovAngle, visionFov);

        noiseSource = Quaternion.AngleAxis(-noiseFovAngle / 2, transform.up) * transform.forward;
        Color soundColor = Color.blue;
        // if (alertStage == AlertStage.Intrigued)
        // {
        //     c = Color.Lerp(Color.green, Color.red, alertLevel / 100f);
        // }
        // else if (alertStage == AlertStage.Alerted)
        // {
        //     c = Color.red;
        // }
        Handles.color = soundColor;
        Handles.DrawSolidArc(transform.position, transform.up, noiseSource, noiseFovAngle, noiseFov);
    }
}
