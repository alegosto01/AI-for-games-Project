using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PatrolState : State
{
    public Transform[] points = new Transform[2]; // the 2 points between which the agent will patrol
    private int destPoint = 0; // the index of the current destination in the list above
    public UnityEngine.AI.NavMeshAgent agent;
    public ChaseState chaseState;
    public float agentSpeed = 3;
    EnemySight enemySight; // the EnemySight component of the enemy

    public bool random = false;
    public List<Vector3> squares = new List<Vector3>();


    private void Start()
    {
        for(int i = -5; i < 5; i++) {
            for(int j = -5; j < 5; j++) {
                squares.Add(new Vector3(i+0.5f,0, j+0.5f));
            }
        }
        enemySight = GetComponentInParent<EnemySight>();
    }

    public override State RunCurrentState() 
    {
        // if the remaining distance between the agent and its destination is less than 0.5 then he should move to the next target in the list
        if (agent.remainingDistance <= 0.5f) 
        {
            GotoNextPoint();
        }
        // if the agent is alerted by the player switch to the chase state
        if (enemySight.chase)
        {
            return chaseState;
        }
        return this;
    }


    void GotoNextPoint() 
    {
        if(random) {
            int randomIndex = UnityEngine.Random.Range(0, squares.Count);
            agent.destination = squares[randomIndex];
        }
        else {
            if (points.Length == 0)
                return;

            agent.speed = agentSpeed;

            // set the new destination point and update the destPoint index
            agent.destination = points[destPoint].position;
            destPoint = (destPoint + 1) % points.Length;
        }
    }
}
