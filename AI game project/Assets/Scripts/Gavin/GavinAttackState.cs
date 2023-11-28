using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class GavinAttackState : State
{
    public GameObject gavin;
    private GameObject enemy;
    public List<GameObject> enemies;
    public NavMeshAgent gavinAgent;
    public GavinStats stats;
    private EnemyStats enemyStats;
    public float maxDistance = 1.0f;  // the max distance from which gavin can attack the enemy
    private float timer = 0;  // a timer that counts how much time passed since the last hit

    public ExploreState exploreState;

    public DecisionMaking decisionMaking;

    private bool attackingEnemy = false;  // does gavin currently have a target enemy to attack?
    private bool chaseEnemy = false;  // should gavin keep chasing the target enemy

    public GameObject manager;
    GameManager gameManager;
    private float visionTimer = 0f;  // a timer that counts how much time passed since the last time that the target enemy was seen
    private float maxChasingTime = 2f;  // gavin will stop chasing the target enemy if he cant see him for more than maxChasingTime


    private void Start()
    {
        gameManager = manager.GetComponent<GameManager>();
        stats = GetComponentInParent<GavinStats>();

        decisionMaking = gavin.GetComponent<DecisionMaking>();
    }

    public override State RunCurrentState()
    {
        Debug.Log("im attakcking");
        enemies = GetComponentInParent<GavinVision>().enemiesInSight;  // a list with all the enemies that gavin can currently see
        if ((!decisionMaking.attack && !chaseEnemy))
        {
            Debug.Log("Went back to explore from the first if");
            enemies.Clear();
            return exploreState;
        }
        if (decisionMaking.runAway)
        {
            if (!exploreState.changedPath)
            {
                exploreState.agent.destination = exploreState.destination;
                Debug.Log("i changed the destination back to the last one");
                enemies.Clear();
                return exploreState;
            }
        }

        // choose a random enemy from the list to attack
        if (!attackingEnemy && enemies.Count > 0)
        {
            System.Random random = new System.Random();
            int randomIndex = random.Next(enemies.Count);
            enemy = enemies[randomIndex];
            enemyStats = enemy.GetComponent<EnemyStats>();
            attackingEnemy = true;
        }
        else if (!attackingEnemy)
        {
            Debug.Log("Went back to explore from the second if");
            enemies.Clear();
            return exploreState;
        }
        else if (attackingEnemy && !chaseEnemy)
        {
            // choose a new target enemy (most probably the one closest)
        }


        // if the enemy that I was attacking is dead set attackingEnemy back to false
        if (!enemy.activeSelf && attackingEnemy)
        {
            attackingEnemy = false;
            return this;
        }

        float distance = Vector3.Distance(transform.position, enemy.transform.position);

        // if close enough to the target enemy start hitting him
        if (distance < maxDistance)
        {
            transform.LookAt(enemy.transform);
            if (timer >= (1 / stats.attackSpeed) || timer == 0)
            {
                enemyStats.health -= stats.attackDamage;
                timer = 0;
            }
            timer += Time.deltaTime;
        }
        else  // if not get close to him
        {
            timer = 0;
            gavinAgent.destination = enemy.transform.position;
        }

        chaseEnemy = ShouldChaseEnemy(enemy);
        // Debug.Log("visionTimer = " + visionTimer + ", chaseEnemy = " + chaseEnemy);
        return this;
    }

    // a function that returns true if its still worth it for gavin to chase the enemy and false otherwise
    private bool ShouldChaseEnemy(GameObject enemy)
    {
        int enemysIndex = gameManager.enemies.IndexOf(enemy);
        float maxRayDistance = 12f;
        Vector3 direction = enemy.transform.position - transform.position;
        Physics.Raycast(transform.position, direction, out RaycastHit hit, maxRayDistance);
        if (hit.rigidbody)
        {
            if (gameManager.enemies.IndexOf(hit.collider.gameObject) == enemysIndex)
            {
                return true;
            }
            else
            {
                visionTimer += Time.deltaTime;
            }
        }
        else
        {
            visionTimer += Time.deltaTime;
        }

        if (visionTimer >= maxChasingTime)
        {
            visionTimer = 0;
            return false;
        }
        else
        {
            return true;
        }
    }
}
