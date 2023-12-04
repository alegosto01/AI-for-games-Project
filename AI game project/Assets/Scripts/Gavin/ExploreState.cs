
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.AI;


// This will need to be completelly changed
public class ExploreState : State
{
    public GameManager gameManager;
    public Gavin gavinScript;
    public GameObject gavin;
    //public GameObject enemy;
    public GavinAttackState attackState;
    //public float maxDistance = 1.0f;

    //// copied variables
    //public float fov;
    //public float radius;
    //public List<Vector3> squares = new List<Vector3>();
    //public List<Vector3> surroundingSquares = new List<Vector3>();
    //public List<Vector3> seenSquares = new List<Vector3>();
    //public List<Vector3> currentSeenSquares = new List<Vector3>();
    //public List<Vector3> walkedSquares = new List<Vector3>();
    //public List<Vector3> obstacleSquares = new List<Vector3>();
    //public List<Vector3> exploringSquares = new List<Vector3>();
    //public HashSet<Vector3> toRemove = new HashSet<Vector3>();

    //public List<Vector3> squaresInCircle = new List<Vector3>();


    //public List<float> inArchSquaresDistances = new List<float>();

    //public float leftDistancesSum = 0;
    //public float rightDistancesSum = 0;

    //public float agentSpeed = 3;
    //public UnityEngine.AI.NavMeshAgent agent;
    //public float timer = 0.5f;
    //public bool lockRotation = false;
    //public bool canSeeTheExit = false;
    //public Transform exitPosition;
    //public Vector3 exitSquare;
    //public bool turning = false;
    //public int turningSide = 1;
    //public Vector3 destination = new Vector3();
    //public float destination_distance = 10;
    //// public bool wallAvoidance;
    //// public int countWallCollision = 0;

    private DecisionMaking decisionMaking;

    //public GameObject walked_prefab;
    //public GameObject destination_prefab;

    public List<List<MazeCell>> all_paths = new List<List<MazeCell>>();
    public MazeCell[,] grid;
    public MazeGenerator mazeGenerator;


    public MazeCell startPosition;
    public MazeCell endPosition;
    public MazeCell currentCell;

    public MazeCell doorCell;
    public MazeCell otherSideDoor;
    public MazeCell keyCell;

    public UnityEngine.AI.NavMeshAgent agent;

    public int optionCount = 0;
    public List<MazeCell> exploreCells = new List<MazeCell>();
    public MazeCell[] gridCells;
    public Vector3 destination = new Vector3();

    public GameObject maze;
    public bool changedPath = false;

    public bool gotTheKey = false;
    public bool foundDoor = false;
    public bool openedDoor = false;

    public GameObject wallToRemove;


    public override State RunCurrentState()
    {

        // timer -= Time.deltaTime;
        // // i call the explore algorithm with the timer or if he is almost arrived to the destination but is not turning
        // // !turning avoid a bug where gavin will be stuck in a point
        // //if (timer < 0) {
        // if((Vector3.Distance(transform.position, destination) < destination_distance/3 && !turning) || (timer < 0 && turning)) {
        //     if(!canSeeTheExit) {
        //         AnalizePath();
        //     }
        //     GotoNextPoint();
        //     timer = 0.5f;
        // }


        // // when gavins need to rotate to find a way rotates depending on turning side
        // if(turning) {
        //     gavin.transform.Rotate(0, 90* Time.deltaTime * turningSide, 0, Space.Self) ;
        // }
        // float distance = Vector3.Distance(gavin.transform.position, enemy.transform.position);

        //Debug.Log("currentCell = " + currentCell.transform.position);
        //Debug.Log("destination = " + destination);
        //Debug.Log("agent.destination = " + agent.destination);
        //Debug.Log("Distance = " + Vector3.Distance(transform.position, agent.destination));
        // Debug.Log(agent.destination);
        //Debug.Log(agent.speed);
        if (Vector3.Distance(transform.position, agent.destination) < 0.5f)
        {
            GoToNextPoint();
            
            if (decisionMaking.runAway && !changedPath)
            {
                agent.speed = 3;
                ChangePath();
            }
            //Debug.Log("I go to next point");
        }

        //Debug.Log("decisionMaking.attack = " + decisionMaking.attack);
        //Debug.Log("decisionMaking.runAway = " + decisionMaking.runAway);
        if (decisionMaking.attack && !decisionMaking.runAway)
        {
            return attackState;
        }

        //if (agent.velocity.magnitude < 0.1f)
        //{
        //    timer += Time.deltaTime;
        //    if (timer > 0.5f)
        //    {
        //        if (timer > 0.6f)
        //        {
        //            timer = 0f;
        //            return attackState;
        //        }
        //        ChangePath();
        //    }
        //}
        return this;

        
    }

    void Awake() {
        decisionMaking = GetComponentInParent<DecisionMaking>();
        gavinScript = GetComponentInParent<Gavin>();
        
    }


    // Start is called before the first frame update
    void Start()
    {
        agent.speed = 1;

        // grid = mazeGenerator._mazeGrid;
        grid = new MazeCell[10, 10];
        gridCells = maze.GetComponentsInChildren<MazeCell>();
        foreach (MazeCell cell in gridCells)
        {
            if ((int)cell.transform.position.x <= 10 && (int)cell.transform.position.x >= 0 && (int)cell.transform.position.z <= 10 && (int)cell.transform.position.z >= 0)
            {
                if (cell.gameObject.name == "Exit") {
                    endPosition = cell;
                }
                else if (cell.gameObject.name == "Door") {
                    doorCell = cell;
                }
                else if (cell.gameObject.name == "OtherSideDoor") {
                    otherSideDoor = cell;
                }
                else if (cell.gameObject.name == "Key") {
                    keyCell = cell;
                }
                grid[(int)cell.transform.position.x, (int)cell.transform.position.z] = cell;
                if ((int)cell.transform.position.x == 0 && (int)cell.transform.position.z == 0)
                {
                    // currentCell = cell;
                    // Debug.Log("change cell at start");
                    // startPosition = cell;
                }
            }
        }
        startPosition = grid[0, 0];
        currentCell = grid[0, 0];
        
        GoToNextPoint();
    }


    public void GoToNextPoint()
    {
        if(!openedDoor) {
            if (currentCell == keyCell) {
                gotTheKey = true;
                if(foundDoor) {
                    Debug.Log("I found the key and i know where the door is");
                    gameManager.gavinText.text = "I have the key now, let's go to the door!";

                }
                else {
                    Debug.Log("I found the key but i don't know where the door is");
                    gameManager.gavinText.text = "I have the key now, let's find the door!";

                }
            }
                if (currentCell == doorCell || Vector3.Distance(doorCell.transform.position, transform.transform.position) < 0.4f) {
                foundDoor = true;
                if(gotTheKey) {
                    Debug.Log("I found the door and i have the key");
                    gameManager.gavinText.text = "I'll open the door!";
                    OpenDoor();
                    openedDoor = true;
                }
                else {
                    Debug.Log("I found the door and but i don't have the key");
                    gameManager.gavinText.text = "I found the door but i don't have the keys";

                }
            }

        }


        if (currentCell == endPosition)
        {
            Debug.Log("found exit!!");
            gameManager.gavinText.text = "I Found The Exit!!!";
        }
        else if (foundDoor && gotTheKey && !openedDoor) {
            agent.destination = doorCell.transform.position;
        }
        else
        {
            List<MazeCell> list_neighbours = GetNeigbours(currentCell);
            // Debug.Log("list neighbours count");
            if (list_neighbours.Count == 1)
            {
                agent.destination = list_neighbours[0].transform.position;
                currentCell = list_neighbours[0];

            }
            else if (list_neighbours.Count > 0)
            {
                agent.destination = list_neighbours[0].transform.position;
                currentCell = list_neighbours[0];


                list_neighbours.RemoveAt(0);
                foreach (MazeCell cell in list_neighbours)
                {
                    exploreCells.Add(cell);
                }
            }
            else
            {
                agent.destination = exploreCells[exploreCells.Count - 1].transform.position;
                currentCell = exploreCells[exploreCells.Count - 1];

                exploreCells.RemoveAt(exploreCells.Count - 1);
            }
            destination = agent.destination;
            currentCell.passed = true;
        }

    }

    public void ChangePath()
    {
        //List<MazeCell> list_neighbours = GetNeigbours(currentCell);

        changedPath = true;
        agent.destination = exploreCells[exploreCells.Count - 1].transform.position;
        destination = agent.destination;

        currentCell = exploreCells[exploreCells.Count - 1];

        exploreCells.RemoveAt(exploreCells.Count - 1);
        Debug.Log("changing the path");
        gameManager.gavinText.text = "There are enemies there, i will change path";

    }

    public List<MazeCell> GetNeigbours(MazeCell currentCell)
    {
        MazeCell leftCell = null;
        MazeCell rightCell = null;
        MazeCell frontCell = null;
        MazeCell backCell = null;
        List<MazeCell> list_neighbours = new List<MazeCell>();
        // Debug.Log("CUrrent cell" + currentCell.transform.position);
        if (currentCell.transform.position.x > 0)
        {
            leftCell = grid[(int)currentCell.transform.position.x - 1, (int)currentCell.transform.position.z];
        }
        if (currentCell.transform.position.x < 9)
        {
            rightCell = grid[(int)currentCell.transform.position.x + 1, (int)currentCell.transform.position.z];
        }
        if (currentCell.transform.position.z < 9)
        {
            frontCell = grid[(int)currentCell.transform.position.x, (int)currentCell.transform.position.z + 1];
        }
        if (currentCell.transform.position.z > 0)
        {
            backCell = grid[(int)currentCell.transform.position.x, (int)currentCell.transform.position.z - 1];
        }

        string neighbors = "";
        if (frontCell)
        {
            if (!currentCell._frontWall.activeSelf && !frontCell._backWall.activeSelf && !frontCell.passed)
            {
                list_neighbours.Add(frontCell);
                neighbors += "front";
                //frontCell.passed = true;
            }
        }
        if (leftCell)
        {
            if (!currentCell._leftWall.activeSelf && !leftCell._rightWall.activeSelf && !leftCell.passed)
            {
                list_neighbours.Add(leftCell);
                neighbors += " left";
                //leftCell.passed = true;
            }
        }
        if (rightCell)
        {
            if (!currentCell._rightWall.activeSelf && !rightCell._leftWall.activeSelf && !rightCell.passed)
            {
                list_neighbours.Add(rightCell);
                neighbors += " right";
                //rightCell.passed = true;
            }
        }
        if (backCell)
        {
            if (!currentCell._backWall.activeSelf && !backCell._frontWall.activeSelf && !backCell.passed)
            {
                list_neighbours.Add(backCell);
                neighbors += " back";
                //backCell.passed = true;

            }
        }
        //Debug.Log(neighbors);

        return list_neighbours;
    }

    public void OpenDoor() {
        agent.Warp(otherSideDoor.transform.position);
        agent.ResetPath();
        currentCell = otherSideDoor;
    }
}   
    //e
    //         }
    //         else if(seenSquares.Count > 0) {
    //             turning = true;
    //         }
    //     }

    //     // int indexx = currentSeenSquares.IndexOf(chosenSquare);
    //     // print(inArchSquaresDistances[indexx]);

    //     // destination_distance = Vector3.Distance(transform.position, destination);
    //     // print(destination_distance);
    //     // Vector3 directionToSquare = chosenSquare - transform.position;

    //     // float angle = Vector3.Angle(directionToSquare, transform.forward);
    //     // Vector3 rayDirection = Quaternion.Euler(0, angle, 0) * direction;

    //     // //calculate the index to find then the corresponding distance in inArchSquaresDistances to use it as maxRayDistance
    //     // int index = currentSeenSquares.IndexOf(square);

    //     // // if i hit something it means that the square is hidden from gavin
    //     // if (Physics.Raycast(transform.position, rayDirection, out RaycastHit hit, inArchSquaresDistances[index]))
    //     // {
    //     //     //find the square where the hit point is
    //     //     Debug.Log("ostacolo");
    //     // }
    // }
    // public int CalculateSide() {
    //     squaresInCircle.Clear();
    //     foreach (Vector3 square in squares) {
    //         float distance = Vector3.Distance(transform.position, square);

    //         if(distance < 6) {

    //             Vector3 directionToSquare = square - transform.position;

    //             float angle = Vector3.Angle(directionToSquare, transform.forward);
    //             Vector3 rayDirection = Quaternion.Euler(0, angle, 0) * transform.forward;

    //             if (!Physics.Raycast(transform.position, rayDirection, out RaycastHit hit, 3))
    //             {
    //                 if(!walkedSquares.Contains(square)) {
    //                     squaresInCircle.Add(square);
    //                 }
    //             }
    //         }
    //     }
    //     float maxSum = 0;
    //     Vector3 goodSideSquare = new Vector3();

    //     foreach (Vector3 square in squaresInCircle) {
    //         float sumOfDistances = 0;
    //         foreach(Vector3 walkedSquare in walkedSquares) {
    //             sumOfDistances += Vector3.Distance(square, walkedSquare);
    //         }
    //         if(sumOfDistances > maxSum) {
    //             maxSum = sumOfDistances;
    //             goodSideSquare = square;
    //         }
    //     }

    //     Debug.Log("Chosen square = " + goodSideSquare);

    //     Vector3 direction = goodSideSquare - transform.position;
    //     float signedAngle = Vector3.SignedAngle(direction,transform.forward, Vector3.up);

    //     Debug.Log(signedAngle);

    //     if(signedAngle < 0) {
    //         return 1;
    //     }
    //     else {
    //         return -1;
    //     }

    // }
