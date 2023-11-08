using System.Threading;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// This will need to be completelly changed
public class ExploreState : State
{
    public GameObject gavin;
    public GameObject enemy;
    public GavinAttackState attackState;
    public float maxDistance = 1.0f;

    // copied variables
    public float fov;
    public float radius;
    public List<Vector3> squares = new List<Vector3>();
    public List<Vector3> surroundingSquares = new List<Vector3>();
    public List<Vector3> seenSquares = new List<Vector3>();
    public List<Vector3> currentSeenSquares = new List<Vector3>();
    public List<Vector3> walkedSquares = new List<Vector3>();
    public List<Vector3> obstacleSquares = new List<Vector3>();
    public List<Vector3> exploringSquares = new List<Vector3>();
    public HashSet<Vector3> toRemove = new HashSet<Vector3>();

    public List<float> inArchSquaresDistances = new List<float>();

    public float agentSpeed = 3;
    public UnityEngine.AI.NavMeshAgent agent;
    public float timer = 0.5f;

    public bool canSeeTheExit = false;
    public Transform exitPosition;
    public bool turning = false;

    public override State RunCurrentState()
    {
        float distance = Vector3.Distance(gavin.transform.position, enemy.transform.position);
        if (distance <= maxDistance)
        {
            return attackState;
        }
        return this;
    }


    // Start is called before the first frame update
    void Start()
    {
        //add all the squares of the map
        for(int i = -5; i < 5; i++) {
            for(int j = -5; j < 5; j++) {
                squares.Add(new Vector3(i+0.5f,0, j+0.5f));
            }
        }
        AnalizePath();
    }

     // Update is called once per frame
    void Update()
    {
        timer -= Time.deltaTime;
        if (timer < 0 ) {
            if(!canSeeTheExit) {
                AnalizePath();
            }
            GotoNextPoint();
            timer = 0.5f;
        }

        if(turning) {
            gavin.transform.Rotate(0, 180* Time.deltaTime, 0, Space.Self) ;
        }
    }

    void AnalizePath() {
        // store in seenSquares all the squares in the arch
        inArchSquaresDistances.Clear();
        currentSeenSquares.Clear();
        foreach (Vector3 point in squares) {
            PointInCircle(point, this.transform.position, radius, fov);
        }

        //raycast to spot the walls
        Vector3 direction = transform.forward;
        foreach (Vector3 square in currentSeenSquares)
        {
            Vector3 directionToSquare = square - transform.position;
            float distance = Vector3.Distance(square, transform.position);
            
            float angle = Vector3.Angle(directionToSquare, transform.forward);
            Vector3 rayDirection = Quaternion.Euler(0, angle, 0) * direction;
            //RaycastHit hit;
            int index = currentSeenSquares.IndexOf(square);
            //print(index);
            
            if (Physics.Raycast(transform.position, rayDirection, out RaycastHit hit, inArchSquaresDistances[index]))
            {
                //find the square where the hit point is
                Vector3 relatedSquare = FindCentreOfSquare(hit.point);
                //Debug.Log("related square = " + relatedSquare + " for this point = " + hit.point);

                // add that square to obstacle square and remove it from seenSquares
                toRemove.Add(relatedSquare);
                toRemove.Add(square);
                if(!obstacleSquares.Contains(relatedSquare) && relatedSquare.y > -1) {
                    obstacleSquares.Add(relatedSquare);
                }
            }
        }

        //raycast to spot the walls
        for (float a = -fov / 2; a <= fov / 2; a += 1.0f)
        {
            Vector3 rayDirection = Quaternion.Euler(0, a, 0) * direction;
            RaycastHit hit;
            if (Physics.Raycast(transform.position, rayDirection, out hit, radius))
            {
                //find the square where the hit point is
                Vector3 relatedSquare = FindCentreOfSquare(hit.point);
                //Debug.Log("related square = " + relatedSquare + " for this point = " + hit.point);

                // add that square to oblstacle square and remove it from seenSquares
                toRemove.Add(relatedSquare);
                if(!obstacleSquares.Contains(relatedSquare) && relatedSquare.y > -1) {
                    obstacleSquares.Add(relatedSquare);
                }

                if(hit.collider.CompareTag("Exit"))
                {
                    canSeeTheExit = true;
                }
            }
        }

        Vector3 currentSquare = FindCentreOfSquare(transform.position);

        Vector3 square1 = currentSquare + new Vector3(0.5f,0,0);
        Vector3 square2 = currentSquare + new Vector3(0.5f,0,0.5f);
        Vector3 square3 = currentSquare + new Vector3(0.5f,0,-0.5f);
        Vector3 square4 = currentSquare + new Vector3(-0.5f,0,0);
        Vector3 square5 = currentSquare + new Vector3(-0.5f,0,0.5f);
        Vector3 square6 = currentSquare + new Vector3(-0.5f,0,-0.5f);
        Vector3 square7 = currentSquare + new Vector3(0,0,-0.5f);
        Vector3 square8 = currentSquare + new Vector3(0,0,0.5f);


        surroundingSquares.Clear();
        surroundingSquares.Add(currentSquare);
        surroundingSquares.Add(square1);
        surroundingSquares.Add(square2);
        surroundingSquares.Add(square3);
        surroundingSquares.Add(square4);
        surroundingSquares.Add(square5);
        surroundingSquares.Add(square6);
        surroundingSquares.Add(square7);
        surroundingSquares.Add(square8);

       
        foreach (Vector3 square in currentSeenSquares)
        {
            if(!seenSquares.Contains(square)) {
                seenSquares.Add(square);
            }
        }
        foreach(Vector3 square in toRemove){
            if(currentSeenSquares.Count > 0) {
                int index = currentSeenSquares.IndexOf(square);
                if(index != -1) {
                    inArchSquaresDistances.RemoveAt(index);
                }
            }
            currentSeenSquares.Remove(square);
            seenSquares.Remove(square);
        }

        foreach(Vector3 square in surroundingSquares){
            if(!walkedSquares.Contains(square)) {
                walkedSquares.Add(square);
            }
            currentSeenSquares.Remove(square);
            seenSquares.Remove(square);
        }
        
        
        toRemove.Clear();
    }

    public Vector3 FindCentreOfSquare(Vector3 hitPos) {
        Vector3 foundSquare = new Vector3(0,0,0);
        foreach (Vector3 square in squares) {
            float x_dif = hitPos.x - square.x;
            float z_dif = hitPos.z - square.z;
            if ( x_dif <= 0.5f && x_dif >= -0.5f && z_dif <= 0.5f && z_dif >= -0.5f) {
                foundSquare = square;
            }
        }
        // if(foundSquare.y == 0 && foundSquare.x == 0 && foundSquare.z == 0) {
        //     Debug.Log(hitPos);
        // }
        return foundSquare;
    }
    public void PointInCircle(Vector3 posPunto, Vector3 gavinPos, float raggio, float fov) {
        
        float distance = Vector3.Distance(posPunto, gavinPos);

        if(distance < raggio) {

            Vector3 direction = posPunto - gavinPos;
            float angle = Vector3.Angle(transform.forward, direction);

            if(angle < fov/2) {
                currentSeenSquares.Add(posPunto);
                inArchSquaresDistances.Add(distance);
                //currentSquaresDistances.Add(distance);
            }
        }
    }

    void GotoNextPoint() 
    {
        agent.speed = agentSpeed;
        if(canSeeTheExit) {
            agent.destination = exitPosition.position;
            turning = false;

        }
        else {
            if(currentSeenSquares.Count > 0) {
                turning = false;
                float maxDistance = 0;
                int index = 0;
                foreach (float distance in inArchSquaresDistances)
                {
                    if(distance > maxDistance) {
                        maxDistance = distance;
                        index = inArchSquaresDistances.IndexOf(distance);
                    }
                }
                // int randomIndex = UnityEngine.Random.Range(0, currentSeenSquares.Count);
                if(index != -1){

                    agent.destination = currentSeenSquares[index];
                }       
            }
            else if(seenSquares.Count > 0) {

                // int randomIndex = UnityEngine.Random.Range(0, seenSquares.Count);       
                // agent.destination = seenSquares[randomIndex];
                turning = true;
                Debug.Log("I ll make a turn");
            }
            else {
                int randomIndex = UnityEngine.Random.Range(0, walkedSquares.Count);       
                agent.destination = walkedSquares[randomIndex];
                Debug.Log("I ll go to a walked square");
            }
        }
        //seenSquares.Clear();
    }


}
