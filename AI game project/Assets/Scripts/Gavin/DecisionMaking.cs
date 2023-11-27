using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DecisionMaking : MonoBehaviour
{
    private GavinVision gavinVision;
    public AStar astar;
    public bool attack = false;  // should gavin attack or not
    // public bool unexploredPaths = false;  // this variable will be true if there are paths that weren't explored yet
    private float chasingTime = 2.0f;  // after how much time of not detecting enemies should attack go back to false
    private float timer = 0f;  // counts how much time isnt detecting enemies

    [SerializeField] GameObject manager;
    private GameManager gameManager;

    private void Start()
    {
        gavinVision = GetComponent<GavinVision>();
        gameManager = manager.GetComponent<GameManager>();
    }

    private void Update()
    {
        if (gavinVision.enemiesDetected)
        {
            float totalEnemiesHealth = 0;
            foreach(GameObject enemy in gavinVision.enemiesInSight)
            {
                totalEnemiesHealth += enemy.GetComponent<EnemyStats>().health;
            }

            if (totalEnemiesHealth < 250)
            {
                attack = true;
            }
            else {
                if(!astar.changedPath) {

                    astar.ChangePath();
                }
            }
        }
        else if (attack)
        {
            timer += Time.deltaTime;
            if (timer > chasingTime)
            {
                attack = false;
                timer = 0;
            }
        }

        if (gameManager.gavinUnderAttack)
        {
            attack = true;
        }
    }
}
