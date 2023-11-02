using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChaseState : State
{
    public UnityEngine.AI.NavMeshAgent agent;
    public GameObject target;
    public float agentSpeed = 3;
    public bool canSeeThePlayer; // true if the agent can see the player
    private Vector3 lastPosition; // the position where the player was last seen by the agent
    private float maxRayDistance = 12.72f; 
    public PatrolState patrolState;
    public AttackState attackState;
    public bool agentInDestination; // true if the agent has arrived to the last point where he saw the player
    private float timer = 0f;
    private float timeBetweenCalls = 0.5f;
    private float stopingDistance = 0.5f;  // the distance from the destination where the agent considers that he has arrived

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

        // increase the value of time by the time that passed since the last frame, and if this is more than half of a second the update the destination
        timer += Time.deltaTime;
        if (timer >= timeBetweenCalls)
        {
            agent.speed = agentSpeed;
            agent.stoppingDistance = stopingDistance;
            agent.destination = lastPosition;
        }

        /* if the agent arrived to the last position were the player was seen and he can't see the player then he goes back to patrol state. If he can
        see him then he keeps chasing him */
        if (agent.remainingDistance <= stopingDistance)
        {
            agentInDestination = true;
            if (canSeeThePlayer)
            {
                return attackState;
            }
            else
            {
                return patrolState;
            }
        }
        else
        {
            agentInDestination = false;
        }
        return this;
    }
}
