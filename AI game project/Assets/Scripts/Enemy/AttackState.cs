using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;

public class AttackState : State
{
    public UnityEngine.AI.NavMeshAgent agent;
    public GameObject target;
    private float distance;  // the distance between the enemy and the player
    public float maxDistance = 1.0f;  // the maximum distance from which the enemy will keep attacking before he switches to the chase state
    public ChaseState chaseState;
    private float timer = 0;  // a timer that counts how much time passed since the last hit
    public EnemyStats myStats;
    public GavinStats gavinStats;

    private void Start()
    {
        myStats = GetComponentInParent<EnemyStats>();
        gavinStats = target.GetComponent<GavinStats>();
    }

    public override State RunCurrentState()
    {
        distance = Vector3.Distance(agent.transform.position, target.transform.position);
        if (distance > maxDistance || !target.activeSelf)
        {
            return chaseState;
        }

        timer += Time.deltaTime;
        if (timer >= (1/myStats.attackSpeed))
        {
            gavinStats.health -= myStats.attackDamage;
            timer = 0;
        }

        return this;
    }
}
