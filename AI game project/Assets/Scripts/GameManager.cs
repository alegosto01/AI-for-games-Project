using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using TMPro;
using System;

public class GameManager : MonoBehaviour
{

    public Text enemiesText;
    public Text gavinText;
    public Text gavinStateText;
    [Range(0.2f, 5)] public float timeScale;

    public GavinStateMachineManager stateMachineManager;

    public List<GameObject> enemies = new List<GameObject>();

    public NavMeshAgent gavin;
    public NavMeshAgent enemy;
    public bool soundExists = false;
    public List<List<object>> noisePositions = new List<List<object>>();
    public Vector3 lastSoundPosition;
    public GavinStats gavinStats;
    public EnemyStats enemyStats;

    private bool gavinWon = false;
    private bool enemiesWon = false;
    public Text gameOverText;  // text that will be displayer when the game is over

    // variables to check if gavin is moving
    public bool gavinIsMoving= false;
    public bool gavinJustStartedMoving = false;

    // variables to control sounds
    private float soundTimer = 0.0f;
    private float soundFrequency = 2.0f;

    public bool gavinUnderAttack = false;
    float prevHealth;
    bool haveBeenAttacked = false;
    float lastAttackedMoment;



    void Start()
    {
        timeScale = 1;
        gavinStats = gavin.gameObject.GetComponent<GavinStats>();
        enemyStats = enemy.gameObject.GetComponent<EnemyStats>();

        gameOverText.gameObject.SetActive(false);

        prevHealth = gavinStats.health;
    }

    // Update is called once per frame
    void Update()
    {
        gavinStateText.text = stateMachineManager.currentState.gameObject.transform.name;
        Debug.Log("speed = " + gavin.speed);
        GavinMovingControl();
        HandleSoundPosition();
        gavinUnderAttack = AttackControl();
        prevHealth = gavinStats.health;
        DeathControl();
        GameOverCondition();
        if (gavinWon || enemiesWon)
        {
            //DisplayGameOverScreen();
        }

        Time.timeScale = timeScale;
    }

    // a function that turns isGavingMoving to true or false depending on if gavin is moving
    void GavinMovingControl()
    {
        gavinJustStartedMoving = false;
        if (gavin.velocity.magnitude != 0)
        {
            if (!gavinIsMoving)
            {
                gavinJustStartedMoving = true;
            }
            gavinIsMoving = true;
        }
        else
        {
            gavinIsMoving = false;
        }
    }

    /* a function that creates a noise every time that gavin just started moving or every 1/soundFrequency seconds if gavin is moving continuously.
     * If gavin is not moving for more then 1/soundFrequency seconds then soundExists = false */
    void HandleSoundPosition()
    {
        soundTimer += Time.deltaTime;
        if (gavinJustStartedMoving)
        {
            soundExists = true;
            lastSoundPosition = gavin.transform.position;
            soundTimer = 0;
        }
        else if (gavinIsMoving && soundTimer >= (1 / soundFrequency))
        {
            soundExists = true;
            lastSoundPosition = gavin.transform.position;
            soundTimer = 0;
        }
        else if (soundTimer >= (1 / soundFrequency))
        {
            soundExists = false;
        }
    }

    bool AttackControl()
    {
        if (gavinStats.health < prevHealth)
        {
            haveBeenAttacked = true;
            lastAttackedMoment = Time.time;
            return true;
        }
        else
        {
            if (haveBeenAttacked)
            {
                if (Time.time - lastAttackedMoment > 1 / enemyStats.attackSpeed)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
            else
            {
                return false;
            }
        }
    }

    private void DeathControl()
    {
        if (gavinStats.health <= 0)
        {
            gavin.gameObject.SetActive(false);
        }
        foreach (GameObject enemy in enemies)
        {
            if (enemy.GetComponent<EnemyStats>().health <= 0)
            {
                enemy.gameObject.SetActive(false);
            }
        }
    }

    private void GameOverCondition()
    {
        if (!gavin.gameObject.activeSelf)
        {
            enemiesWon = true;
        }
        if (gavin.transform.position.x > 5)
        {
            gavinWon = true;
        }
    }

    private void DisplayGameOverScreen()
    {   
        if (gavinWon)
        {
            gameOverText.text = "Gavin Won!!!";
        }
        else if (enemiesWon)
        {
            gameOverText.text = "Enemies Won!!!";
        }

        gameOverText.gameObject.SetActive(true);
    }
}