using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DecisionMaking : MonoBehaviour
{
    private GavinVision gavinVision;
    public bool attack = false;
    public bool unexploredPaths = false;  // this variable will be true if there are paths that weren't explored yet

    private void Start()
    {
        gavinVision = GetComponent<GavinVision>();
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

            if (totalEnemiesHealth < 200)
            {
                attack = true;
                Debug.Log("attack");
            }
            else
            {
                Debug.Log("run away");
            }
        }
    }
}
