using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PatrolState : State
{
    // public bool patrolState;
    public Transform[] points = new Transform[2];
    private int destPoint = 0;
    public UnityEngine.AI.NavMeshAgent agent;
    public ChaseState chaseState;

    public override State RunCurrentState() {
        if (!agent.pathPending && agent.remainingDistance < 0.5f) {
            GotoNextPoint();
        }
        if (EnemySight.chase)
        {
            return chaseState;
        }
        return this;
    }


    void GotoNextPoint() {
        if (points.Length == 0)
            return;

        agent.destination = points[destPoint].position;

        destPoint = (destPoint + 1) % points.Length;
    }

    void Update () {
        // Choose the next destination point when the agent gets
        // close to the current one.
    }

    // void Start(){
    //     points[0].position = new Vector3(3.33999991f,0.360000014f,-2.58999991f);
    //     points[1].position = new Vector3(3.33999991f,0.360000014f,2.58999991f);
    // }
}
