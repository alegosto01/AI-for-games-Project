using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GavinAttackState : State
{
    public GameObject gavin;
    public GameObject enemy;
    public EnemyStats enemyStats;
    public GavinStats stats;
    public float maxDistance = 1.0f;
    private float timer = 0;  // a timer that counts how much time passed since the last hit

    public ExploreState exploreState;


    private void Start()
    {
        stats = GetComponentInParent<GavinStats>();
        enemyStats = enemy.GetComponent<EnemyStats>();
    }

    public override State RunCurrentState()
    {
        float distance = Vector3.Distance(gavin.transform.position, enemy.transform.position);
        if (distance > maxDistance)
        {
            return exploreState;
        }

        timer += Time.deltaTime;
        if (timer >= (1/stats.attackSpeed))
        {
            enemyStats.health -= stats.attackDamage;
            timer = 0;
        }
        return this;
    }
}
