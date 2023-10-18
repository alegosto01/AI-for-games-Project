using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChaseState : State
{
    public UnityEngine.AI.NavMeshAgent agent;
    public Transform target;
    public override State RunCurrentState()
    {
        agent.destination = target.position;
        return this;
    }
}
