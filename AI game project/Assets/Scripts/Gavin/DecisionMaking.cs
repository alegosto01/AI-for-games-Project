using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DecisionMaking : MonoBehaviour
{
    private GavinVision gavinVision;
    public bool attack = false;  // should gavin attack or not
    public bool runAway = false;
    // public bool unexploredPaths = false;  // this variable will be true if there are paths that weren't explored yet
    //private float chasingTime = 2.0f;  // after how much time of not detecting enemies should attack go back to false
    //private float timer = 0f;  // counts how much time isnt detecting enemies

    [SerializeField] GameObject manager;
    private GameManager gameManager;

    GavinStats gavinStats;

    public float runAwayTimer = 0f;

    public GavinHearing gavinHearing;

    private void Start()
    {
        gavinVision = GetComponent<GavinVision>();
        gameManager = manager.GetComponent<GameManager>();
        gavinStats = GetComponent<GavinStats>();
        gavinHearing = GetComponent<GavinHearing>();   
    }

    private void Update()
    {
        if (runAway)
        {
            gameManager.gavinText.text = "Run awayyy!!!";
        }
        if (gavinVision.enemiesDetected)
        {
            float totalEnemiesHealth = 0;
            foreach(GameObject enemy in gavinVision.enemiesInSight)
            {
                totalEnemiesHealth += enemy.GetComponent<EnemyStats>().health;
            }

            if (totalEnemiesHealth < 2.5*gavinStats.health)
            {
                attack = true;
            }
            else
            {
                attack = false;
                runAway = true;
                //if(!astar.changedPath) {

                //    astar.ChangePath();
                //}
            }
        }
        //else if (attack && !gameManager.gavinUnderAttack)
        //{
        //    timer += Time.deltaTime;
        //    if (timer > chasingTime)
        //    {
        //        attack = false;
        //        timer = 0;
        //    }
        //}
        else
        {
            attack = false;
        }

        //if (gameManager.gavinUnderAttack && !runAway)
        //{
        //    attack = true;
        //}

        if (runAway)
        {
            runAwayTimer += Time.deltaTime;
            if (gavinHearing.enemiesInArc.Count > 0)
            {
                float totalEnemiesHealth = 0f;
                foreach (GameObject enemy in gavinHearing.enemiesInArc)
                {
                    totalEnemiesHealth += enemy.GetComponent<EnemyStats>().health;
                    //Vector3 direction = enemy.transform.position - transform.position;
                    //Physics.Raycast(transform.position, direction, out RaycastHit hit, 12f);
                    //if (hit.rigidbody)
                    //{
                    //    if (hit.rigidbody.tag == "Enemy")
                    //    {
                    //        totalEnemiesHealth += enemy.GetComponent<EnemyStats>().health;
                    //    }
                    //}
                }

                if (totalEnemiesHealth < 2.5 * gavinStats.health)
                {
                    runAway = false;
                }
            }
            
        }
    }
}
