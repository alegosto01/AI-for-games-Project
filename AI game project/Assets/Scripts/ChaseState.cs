using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChaseState : State
{
    public UnityEngine.AI.NavMeshAgent agent;
    public GameObject target;
    public float agentSpeed = 3;
    private bool canSeeThePlayer; // true if the agent can see the player
    private Vector3 lastPosition; // the position where the player was last seen by the agent
    private float maxRayDistance = 7; 
    public PatrolState patrolState;
    public bool agentInDestination; // true if the agent has arrived to the last point where he saw the player

    public override State RunCurrentState()
    {
        Vector3 direction = target.transform.position - agent.transform.position;
        Physics.Raycast(agent.transform.position, direction, out RaycastHit hit, maxRayDistance);
        if (hit.rigidbody)
        {
            if (hit.rigidbody.tag == "Player")
            {
                canSeeThePlayer = true;
            }
            else
            {
                canSeeThePlayer = false;
            }
        }
        else
        {
            canSeeThePlayer = false;
        }

        // if the agent can see the player then update the last position were the player was seen
        if (canSeeThePlayer)
        {
            lastPosition = target.transform.position;
        }

        agent.speed = agentSpeed;
        agent.destination = lastPosition;

        /* if the agent arrived to the last position were the player was seen and he can't see the player then he goes back to patrol state. If he can
        see him then he keeps chasing him */
        if (agent.remainingDistance < 0.1f)
        {
            agentInDestination = true;
            if (canSeeThePlayer)
            {
                return this;
            }
            else
            {
                return patrolState;
            }
        }

        return this;
    }
}
