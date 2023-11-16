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

    // Start is called before the first frame update
    void Start()
    {
        grid = mazeGenerator._mazeGrid;

        GeneratePaths();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void GeneratePaths() {
        //int count = 100;
        MazeCell currentCell = startPosition;
        bool stop = false;
        List<List<MazeCell>> to_add = new List<List<MazeCell>>();

        do {
            to_add.Clear();
            List<MazeCell> list_neighbours = GetNeigbours(currentCell);
            //count -= list_neighbours.Count;
            if(list_neighbours.Count == 0) {
                currentCell.deadEnd = true;
            }
            else {
                foreach (List<MazeCell> path in all_paths) {
                    if(path[path.Count - 1] == currentCell) {
                        if(list_neighbours.Count == 1) {
                            path.Add(list_neighbours[0]);
                        }
                        else {
                            for (int i = 0; i < list_neighbours.Count - 1; i++) {
                                List<MazeCell> new_path = new List<MazeCell>(path);
                                new_path.Add(list_neighbours[i]);
                                to_add.Add(new_path);
                            }
                        }
                    }
                }
                foreach (List<MazeCell> path in to_add) {
                    all_paths.Add(path);
                }
            }

            foreach(List<MazeCell> path in all_paths) {
                if(path[path.Count - 1].deadEnd == false) {
                    currentCell = path[path.Count - 1];
                    break;
                }
                stop = true;
            }
            

        } while (!stop);
        
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
        if(currentCell.transform.position.x > 0) {
            rightCell = grid[(int)currentCell.transform.position.x, (int)currentCell.transform.position.z - 1];
        }

        if(leftCell) {
            if(currentCell._leftWall.activeSelf && leftCell._rightWall.activeSelf && !leftCell.passed) {
                list_neighbours.Add(leftCell);
                leftCell.passed = true;
            }
        }
        if(rightCell) {
            if(currentCell._rightWall.activeSelf && rightCell._leftWall.activeSelf && !rightCell.passed) {
                list_neighbours.Add(rightCell);
                rightCell.passed = true;
            }
        }
        if(frontCell) {
            if(currentCell._frontWall.activeSelf && frontCell._backWall.activeSelf && !frontCell.passed) {
                list_neighbours.Add(frontCell);
                frontCell.passed = true;

            }
        }
        if(backCell) {
            if(currentCell._backWall.activeSelf && backCell._frontWall.activeSelf && !backCell.passed) {
                list_neighbours.Add(backCell);
                backCell.passed = true;

            }
        }

        return list_neighbours;
    }
}
