using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


public class Gavin : MonoBehaviour
{
    public GameManager gameManager;
    // moving variables
    public Vector3 lastPosition;

    public float noiseTimer = 0.5f;
    public float noiseFrequency = 0.5f;
    public bool isMoving = false;
    public bool stealth = false;

    //speed variables
    public float stealthSpeed = 2;
    public float runningSpeed = 4;
    public float gavinSpeed;

    //noise generation variables
    //public float makingNoisefrequency = 2;  //times per second
    public float noiseRadiusStealth = 2f;
    public float noiseRadiusRunning = 4f;
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

    void Start()
    {
        gavinSpeed = runningSpeed;
        noiseFov = noiseRadiusRunning;
        previousPositions.Add(startPosition);  
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

    // public void MakeNoise() {
    //    Debug.Log("make noise");
    //    gameManager.NewNoise(noiseFov);
    //}

    public void Moving() {
        //Controls of gavin movement

        //if is not moving change the variables "isMoving"

        if(isMoving && GetComponent<Rigidbody>().velocity.magnitude == 0)
        {
            isMoving = false;
            gameManager.isGavinMoving = false;        
            //noiseTimer = noiseFrequency;
        }        


        if (Input.GetKey(KeyCode.UpArrow))
        {
            isMoving = true;
            //gameManager.isGavinMoving = true;
            transform.position += new Vector3(0,0,gavinSpeed) * Time.deltaTime;
            //lastPosition = transform.position;
            //noiseTimer -= 1 * Time.deltaTime;
            
            //if(noiseTimer <= 0) {
            //    MakeNoise();
            //    noiseTimer = noiseFrequency;
            //}
        }
        if (Input.GetKey(KeyCode.DownArrow))
        {
            isMoving = true;
            //gameManager.isGavinMoving = true;
            transform.position += new Vector3(0,0,-gavinSpeed) * Time.deltaTime;
            //lastPosition = transform.position;

            //noiseTimer -= 1 * Time.deltaTime;
            
            //if(noiseTimer <= 0) {
            //    MakeNoise();
            //    noiseTimer = noiseFrequency;
            //}
        }
        if (Input.GetKey(KeyCode.RightArrow))
        {
            isMoving = true;
            //gameManager.isGavinMoving = true;
            transform.position += new Vector3(gavinSpeed,0,0) * Time.deltaTime;
            //lastPosition = transform.position;

            //noiseTimer -= 1 * Time.deltaTime;
            
            //if(noiseTimer <= 0) {
            //    MakeNoise();
            //    noiseTimer = noiseFrequency;
            //}
        }
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            isMoving = true;
            //gameManager.isGavinMoving = true;
            transform.position += new Vector3(-gavinSpeed,0,0) * Time.deltaTime;
            //lastPosition = transform.position;

            //noiseTimer -= 1 * Time.deltaTime;
            
            //if(noiseTimer <= 0) {
            //    MakeNoise();
            //    noiseTimer = noiseFrequency;
            //}
        }
        
    }

   

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
       
        Handles.color = c;
        Handles.DrawSolidArc(transform.position, transform.up, visionSourceRightForward, visionFovAngle, visionFov);
        Handles.DrawSolidArc(transform.position,transform.up, visionSourceLeftForward, visionFovAngle, visionFov);
        Handles.DrawSolidArc(transform.position,transform.up, visionSourceLeft, visionFovAngle, visionFov);
        Handles.DrawSolidArc(transform.position,transform.up, visionSourceForward, visionFovAngle, visionFov);
        Handles.DrawSolidArc(transform.position,transform.up, visionSourceRight, visionFovAngle, visionFov);

        noiseSource = Quaternion.AngleAxis(-noiseFovAngle / 2, transform.up) * transform.forward;
        Color soundColor = Color.blue;
      
        Handles.color = soundColor;
        Handles.DrawSolidArc(transform.position, transform.up, noiseSource, noiseFovAngle, noiseFov);
    }
}
