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


    private void Start()
    {
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
        if (points.Length == 0)
            return;

        agent.speed = agentSpeed;

        // set the new destination point and update the destPoint index
        agent.destination = points[destPoint].position;
        destPoint = (destPoint + 1) % points.Length;
    }
}
