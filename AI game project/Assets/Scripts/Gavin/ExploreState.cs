
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;


// This will need to be completelly changed
public class ExploreState : State
{
    public Gavin gavinScript;
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

    public List<Vector3> squaresInCircle = new List<Vector3>();


    public List<float> inArchSquaresDistances = new List<float>();

    public float leftDistancesSum = 0;
    public float rightDistancesSum = 0;

    public float agentSpeed = 3;
    public UnityEngine.AI.NavMeshAgent agent;
    public float timer = 0.5f;
    public bool lockRotation = false;
    public bool canSeeTheExit = false;
    public Transform exitPosition;
    public Vector3 exitSquare;
    public bool turning = false;
    public int turningSide = 1;
    public Vector3 destination = new Vector3();
    public float destination_distance = 10;
    // public bool wallAvoidance;
    // public int countWallCollision = 0;

    private DecisionMaking decisionMaking;

    public GameObject walked_prefab;
    public GameObject destination_prefab;

    


    public override State RunCurrentState()
    {
      
        timer -= Time.deltaTime;
        // i call the explore algorithm with the timer or if he is almost arrived to the destination but is not turning
        // !turning avoid a bug where gavin will be stuck in a point
        //if (timer < 0) {
        if((Vector3.Distance(transform.position, destination) < destination_distance/3 && !turning) || (timer < 0 && turning)) {
            if(!canSeeTheExit) {
                AnalizePath();
            }
            GotoNextPoint();
            timer = 0.5f;
        }


        // when gavins need to rotate to find a way rotates depending on turning side
        if(turning) {
            gavin.transform.Rotate(0, 90* Time.deltaTime * turningSide, 0, Space.Self) ;
        }
        float distance = Vector3.Distance(gavin.transform.position, enemy.transform.position);
        
        if (decisionMaking.attack)
        {
            return attackState;
        }
        return this;

        
    }

    void Awake() {
        decisionMaking = GetComponentInParent<DecisionMaking>();
        gavinScript = GetComponentInParent<Gavin>();
        
    }


    // Start is called before the first frame update
    void Start()
    {
        //add all the squares of the map and the border to the obstacle squares
        for(int i = -5; i < 5; i+=1) {
            for(int j = -5; j < 5; j+= 1) {
                Vector3 newSquare = new Vector3(i+1f,0, j+1);
                squares.Add(newSquare);
                if(i == 4 || j == 4) {
                    obstacleSquares.Add(newSquare);
                    toRemove.Add(newSquare);
                }
            }
        }

        // find exit square
        exitSquare = FindCentreOfSquare(exitPosition.position);

        //start finding a path
        AnalizePath();
        GotoNextPoint();


        decisionMaking = GetComponentInParent<DecisionMaking>();

        
    }

    void AnalizePath() {

        inArchSquaresDistances.Clear();
        currentSeenSquares.Clear();

        // store in seenSquares all the squares in the arch
        foreach (Vector3 point in squares) {
            PointInCircle(point, this.transform.position, radius, fov);
        }

        //raycast to all the squares in the arch
        Vector3 direction = transform.forward;
        foreach (Vector3 square in currentSeenSquares)
        {
            Vector3 directionToSquare = square - transform.position;
            
            float angle = Vector3.Angle(directionToSquare, transform.forward);
            Vector3 rayDirection = Quaternion.Euler(0, angle, 0) * direction;

            //calculate the index to find then the corresponding distance in inArchSquaresDistances to use it as maxRayDistance
            int index = currentSeenSquares.IndexOf(square);
            
            // if i hit something it means that the square is hidden from gavin
            if (Physics.Raycast(transform.position, rayDirection, out RaycastHit hit, inArchSquaresDistances[index]))
            {
                //find the square where the hit point is
                Vector3 relatedSquare = FindCentreOfSquare(hit.point);

                // add that square to obstacle square and remove it from seenSquares
                toRemove.Add(relatedSquare);
                toRemove.Add(square);
                if(!obstacleSquares.Contains(relatedSquare) && relatedSquare.y > -1) {
                    obstacleSquares.Add(relatedSquare);
                }
            }
        }

        //initialize the distances sum
        leftDistancesSum = 0;
        rightDistancesSum = 0;

        //raycast in the whole arch 
        //countWallCollision = 0;
        for (float a = -fov / 2; a <= fov / 2; a += 1.0f)
        {
            Vector3 rayDirection = Quaternion.Euler(0, a, 0) * direction;
            RaycastHit hit;
            
            if (Physics.Raycast(transform.position, rayDirection, out hit, radius))
            {
                //countWallCollision += 1;
                //find the square where the hit point is
                Vector3 relatedSquare = FindCentreOfSquare(hit.point);

                // i calculate the distance to the wall and i add to left or right sum of distances
                // to be able to understand if gavin needs to turn right or left
                float distance = Vector3.Distance(relatedSquare, transform.position);
                if(a > 0) {
                    rightDistancesSum += distance;
                }
                else {
                    leftDistancesSum += distance;
                }

                // add that square to oblstacle square and remove it from seenSquares
                toRemove.Add(relatedSquare);
                if(!obstacleSquares.Contains(relatedSquare) && relatedSquare.y > -1) {
                    obstacleSquares.Add(relatedSquare);
                }
            }
        }

        // if(countWallCollision > 100) {
        //     Debug.Log("wall avoidance");
        //     wallAvoidance = true;
        // }
        // else {
        //     wallAvoidance = false;
        // }

        // calculate to which side gavin has tu turn
        // if(!turning) {
        //     if(rightDistancesSum >= leftDistancesSum) {
        //         turningSide = 1;
        //     }
        //     else {
        //         turningSide = -1;
        //     }
        // }

        // calculate all the squares around me 
        Vector3 currentSquare = FindCentreOfSquare(transform.position);

        Vector3 square1 = currentSquare + new Vector3(1,0,0);
        Vector3 square2 = currentSquare + new Vector3(1,0,1);
        Vector3 square3 = currentSquare + new Vector3(1,0,-1);
        Vector3 square4 = currentSquare + new Vector3(-1,0,0);
        Vector3 square5 = currentSquare + new Vector3(-1,0,1);
        Vector3 square6 = currentSquare + new Vector3(-1,0,-1);
        Vector3 square7 = currentSquare + new Vector3(0,0,-1);
        Vector3 square8 = currentSquare + new Vector3(0,0,1);


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

        

        // add squares seen for the first time to seenSquares
        foreach (Vector3 square in currentSeenSquares)
        {
            if(!seenSquares.Contains(square)) {
                seenSquares.Add(square);
            }
        }

        // remove squares no reachable
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

        // mark the surrounding squares as walked
        foreach(Vector3 square in surroundingSquares){
            if(!walkedSquares.Contains(square)) {
                walkedSquares.Add(square);
                Instantiate(walked_prefab, square + new Vector3(0,0.1f,0), Quaternion.identity);
            }
        }
        
        // if (currentSeenSquares.Contains(exitSquare)) {
        //     canSeeTheExit = true;
        // }
        
        toRemove.Clear();
    }

    // return the corresponsing square to a point
    public Vector3 FindCentreOfSquare(Vector3 hitPos) {
        Vector3 foundSquare = new Vector3(0,0,0);
        foreach (Vector3 square in squares) {
            float x_dif = hitPos.x - square.x;
            float z_dif = hitPos.z - square.z;
            if ( x_dif <= 0.5f && x_dif >= -0.5f && z_dif <= 0.5f && z_dif >= -0.5f) {
                foundSquare = square;
            }
        }
        return foundSquare;
    }

    // detect if a point is in the arch of vision
    public void PointInCircle(Vector3 posPunto, Vector3 gavinPos, float raggio, float fov) {
        
        float distance = Vector3.Distance(posPunto, gavinPos);

        if(distance < raggio) {

            Vector3 direction = posPunto - gavinPos;
            float angle = Vector3.Angle(transform.forward, direction);

            if(angle < fov/2) {
                currentSeenSquares.Add(posPunto);
                inArchSquaresDistances.Add(distance);
            }
        }
    }

    // manage to move gavin to the next destination
    void GotoNextPoint() 
    {
        Vector3 chosenSquare = new Vector3();
        //agent.speed = agentSpeed;
        if(canSeeTheExit) {
            agent.destination = exitPosition.position;
            destination = agent.destination;
            turning = false;
            agent.updateRotation = true;
        }
        else {
            if(currentSeenSquares.Count > 0) {
                turning = false;
                agent.updateRotation = true;

                float distanceFromWalkedSquares = 0;
                int index = 0;
                // Vector3 chosenSquare = new Vector3();
                foreach (Vector3 square in currentSeenSquares)
                {
                    float sumDistances = 0;
                    foreach (Vector3 walkedSquare in walkedSquares)
                    {
                        sumDistances += Vector3.Distance(walkedSquare, square);
                    }
                    if(sumDistances > distanceFromWalkedSquares) {
                        distanceFromWalkedSquares = sumDistances;
                        chosenSquare = square;
                    }
                }
                
                // max distance to the wall 
                float maxDistance = inArchSquaresDistances.Max();

                // foreach (float distance in inArchSquaresDistances)
                // {
                //     if(distance > maxDistance) {
                //         maxDistance = distance;
                //         // int tryIndex = inArchSquaresDistances.IndexOf(distance);
                //         // if(tryIndex >= 0 && !walkedSquares.Contains(currentSeenSquares[tryIndex])) {
                            
                //         //     index = tryIndex;
                //         // }
                //     }
                // }

                // if(index == -1) {
                //     int randomIndex = UnityEngine.Random.Range(0, seenSquares.Count);
                //     agent.destination = seenSquares[randomIndex];
                //     destination = agent.destination;
                //     agent.speed = 2;
                //     Debug.Log("go random" + agent.destination);
                // }
                // else {

                // }

                // change speed depending on how near to the wall gavin is, if he is not in stealth mode
                if(!gavinScript.stealth) {
                    agent.speed = maxDistance / 2;
                }
                
                // if we are to close to the wall rotate to find better way
                if(maxDistance < 1.5f) {
                    if(!lockRotation) {
                        turningSide = CalculateSide();
                    }
                    lockRotation = true;
                    turning = true;
                    agent.updateRotation = false;
                }
                else {
                    turning = false;
                    agent.updateRotation = true;

                    if(index != -1){
                        agent.destination = chosenSquare;
                        Instantiate(destination_prefab, chosenSquare + new Vector3(0,0.11f,0), Quaternion.identity);
                        destination = agent.destination;
                        lockRotation = false;
                    }  
                }
            }
            else if(seenSquares.Count > 0) {
                turning = true;
            }
        }

        // int indexx = currentSeenSquares.IndexOf(chosenSquare);
        // print(inArchSquaresDistances[indexx]);

        // destination_distance = Vector3.Distance(transform.position, destination);
        // print(destination_distance);
        // Vector3 directionToSquare = chosenSquare - transform.position;
            
        // float angle = Vector3.Angle(directionToSquare, transform.forward);
        // Vector3 rayDirection = Quaternion.Euler(0, angle, 0) * direction;

        // //calculate the index to find then the corresponding distance in inArchSquaresDistances to use it as maxRayDistance
        // int index = currentSeenSquares.IndexOf(square);
        
        // // if i hit something it means that the square is hidden from gavin
        // if (Physics.Raycast(transform.position, rayDirection, out RaycastHit hit, inArchSquaresDistances[index]))
        // {
        //     //find the square where the hit point is
        //     Debug.Log("ostacolo");
        // }
    }
    public int CalculateSide() {
        squaresInCircle.Clear();
        foreach (Vector3 square in squares) {
            float distance = Vector3.Distance(transform.position, square);

            if(distance < 6) {

                Vector3 directionToSquare = square - transform.position;
            
                float angle = Vector3.Angle(directionToSquare, transform.forward);
                Vector3 rayDirection = Quaternion.Euler(0, angle, 0) * transform.forward;
                
                if (!Physics.Raycast(transform.position, rayDirection, out RaycastHit hit, 3))
                {
                    if(!walkedSquares.Contains(square)) {
                        squaresInCircle.Add(square);
                    }
                }
            }
        }
        float maxSum = 0;
        Vector3 goodSideSquare = new Vector3();

        foreach (Vector3 square in squaresInCircle) {
            float sumOfDistances = 0;
            foreach(Vector3 walkedSquare in walkedSquares) {
                sumOfDistances += Vector3.Distance(square, walkedSquare);
            }
            if(sumOfDistances > maxSum) {
                maxSum = sumOfDistances;
                goodSideSquare = square;
            }
        }

        Debug.Log("Chosen square = " + goodSideSquare);

        Vector3 direction = goodSideSquare - transform.position;
        float signedAngle = Vector3.SignedAngle(direction,transform.forward, Vector3.up);

        Debug.Log(signedAngle);

        if(signedAngle < 0) {
            return 1;
        }
        else {
            return -1;
        }

    }
}
