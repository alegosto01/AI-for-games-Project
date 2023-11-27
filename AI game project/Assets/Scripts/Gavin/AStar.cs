using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AStar : MonoBehaviour
{
    public List<List<MazeCell>> all_paths = new List<List<MazeCell>>();
    public MazeCell[,] grid;
    public MazeGenerator mazeGenerator;


    public MazeCell startPosition;
    public MazeCell endPosition;
    public MazeCell currentCell;

    public UnityEngine.AI.NavMeshAgent agent;

    public int optionCount = 0;
    public List<MazeCell> exploreCells = new List<MazeCell>();
    public MazeCell[] gridCells;
    public Vector3 destination = new Vector3();

    public GameObject maze;


    // Start is called before the first frame update
    void Start()
    {
        agent.speed = 1;
        
        // grid = mazeGenerator._mazeGrid;
         grid = new MazeCell[10, 10];
        gridCells = maze.GetComponentsInChildren<MazeCell>();
        foreach (MazeCell cell in gridCells) {
            if((int)cell.transform.position.x <=10 && (int)cell.transform.position.x >= 0 && (int)cell.transform.position.z <=10 && (int)cell.transform.position.z >= 0) {
                grid[(int)cell.transform.position.x, (int)cell.transform.position.z] = cell;
                if((int)cell.transform.position.x == 0 && (int)cell.transform.position.z == 0) {
                    currentCell = cell;
                    startPosition = cell;
                }
            }
        }
        startPosition = grid[0,0];
        currentCell = grid[0,0];

        GoToNextPoint();

        //GeneratePaths();
    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log(Vector3.Distance(transform.position, destination));
        if(Vector3.Distance(transform.position, destination) < 0.4f) {
            Debug.Log("ciao");
            GoToNextPoint();
        }
    }

    // public void GeneratePaths() {
    //     //int count = 100;
    //     MazeCell currentCell = startPosition;
    //     bool stop = false;
    //     List<List<MazeCell>> to_add = new List<List<MazeCell>>();

    //     do {
    //         to_add.Clear();
    //         List<MazeCell> list_neighbours = GetNeigbours(currentCell);
    //         //count -= list_neighbours.Count;
    //         if(list_neighbours.Count == 0) {
    //             currentCell.deadEnd = true;
    //         }
    //         else {
    //             foreach (List<MazeCell> path in all_paths) {
    //                 if(path[path.Count - 1] == currentCell) {
    //                     if(list_neighbours.Count == 1) {
    //                         path.Add(list_neighbours[0]);
    //                     }
    //                     else {
    //                         for (int i = 0; i < list_neighbours.Count - 1; i++) {
    //                             List<MazeCell> new_path = new List<MazeCell>(path);
    //                             new_path.Add(list_neighbours[i]);
    //                             to_add.Add(new_path);
    //                         }
    //                     }
    //                 }
    //             }
    //             foreach (List<MazeCell> path in to_add) {
    //                 all_paths.Add(path);
    //             }
    //         }

    //         foreach(List<MazeCell> path in all_paths) {
    //             if(path[path.Count - 1].deadEnd == false) {
    //                 currentCell = path[path.Count - 1];
    //                 break;
    //             }
    //             stop = true;
    //         }
            

    //     } while (!stop);
        
    // }

    public void GoToNextPoint() {
        List<MazeCell> list_neighbours = GetNeigbours(currentCell);
        //Debug.Log(list_neighbours[0].transform.position);
        if(list_neighbours.Count == 1) {
            agent.destination = list_neighbours[0].transform.position;
            currentCell = list_neighbours[0];
        }
        else if(list_neighbours.Count > 0) {
            agent.destination = list_neighbours[0].transform.position;
            currentCell = list_neighbours[0];

            list_neighbours.RemoveAt(0);
            foreach (MazeCell cell in list_neighbours) {
                exploreCells.Add(cell);
            }
        }
        else {
            agent.destination = exploreCells[exploreCells.Count - 1].transform.position;
            currentCell = exploreCells[exploreCells.Count - 1];
            exploreCells.RemoveAt(exploreCells.Count - 1);
        }
        destination = agent.destination;

    }

    public List<MazeCell> GetNeigbours(MazeCell currentCell) {
        MazeCell leftCell = null;
        MazeCell rightCell = null;
        MazeCell frontCell = null;
        MazeCell backCell = null;
        List<MazeCell> list_neighbours = new List<MazeCell>();

        if(currentCell.transform.position.x > 0) {
            leftCell = grid[(int)currentCell.transform.position.x - 1, (int)currentCell.transform.position.z];
        }
        if(currentCell.transform.position.x < 9) {
            rightCell = grid[(int)currentCell.transform.position.x +1, (int)currentCell.transform.position.z];
        }
        if(currentCell.transform.position.z < 9) {
            frontCell = grid[(int)currentCell.transform.position.x, (int)currentCell.transform.position.z + 1];
        }
        if(currentCell.transform.position.z > 0) {
            backCell = grid[(int)currentCell.transform.position.x, (int)currentCell.transform.position.z - 1];
        }

        if(frontCell) {
            if(!currentCell._frontWall.activeSelf && !frontCell._backWall.activeSelf && !frontCell.passed) {
                list_neighbours.Add(frontCell);
                frontCell.passed = true;
            }
        }
        if(leftCell) {
            if(!currentCell._leftWall.activeSelf && !leftCell._rightWall.activeSelf && !leftCell.passed) {
                list_neighbours.Add(leftCell);
                leftCell.passed = true;
            }
        }
        if(rightCell) {
            if(!currentCell._rightWall.activeSelf && !rightCell._leftWall.activeSelf && !rightCell.passed) {
                list_neighbours.Add(rightCell);
                rightCell.passed = true;
            }
        }
        if(backCell) {
            if(!currentCell._backWall.activeSelf && !backCell._frontWall.activeSelf && !backCell.passed) {
                list_neighbours.Add(backCell);
                backCell.passed = true;

            }
        }

        return list_neighbours;
    }
}
