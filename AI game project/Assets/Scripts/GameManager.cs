using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public List<GameObject> enemies = new List<GameObject>();

    public GameObject gavin;
    public GameObject enemy;
    public bool isGavinMoving = false;
    private float soundTimer = 0.0f;
    private float soundFrequency = 2.0f;
    public bool soundExists = false;
    public List<List<object>> noisePositions = new List<List<object>>();
    public Vector3 lastSoundPosition;
    public GavinStats gavinStats;
    public EnemyStats enemyStats;

    private bool gavinWon = false;
    private bool enemiesWon = false;
    public Text gameOverText;  // text that will be displayer when the game is over

    public float noiseDuration = 1.0f;
    //private Vector3 gavinsPreviousPosition;
    
    void Start()
    {
        gavinStats = gavin.GetComponent<GavinStats>();
        enemyStats = enemy.GetComponent<EnemyStats>();

        gameOverText.gameObject.SetActive(false);
        //gavinsPreviousPosition = gavin.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        GavinMovingControl();
        HandleSoundPosition();
        DeathControl();
        GameOverCondition();
        if (gavinWon || enemiesWon)
        {
            DisplayGameOverScreen();
        }
    }

    // a function that turns isGavingMoving to true or false depending on if gavin is moving
    void GavinMovingControl()
    {
        //Vector3 gavinsCurrentPosition = gavin.transform.position;
        if (gavin.GetComponent<Gavin>().isMoving)
        {
            isGavinMoving = true;
            soundExists = true;
            lastSoundPosition = gavin.transform.position;
        }
        else
        {
            isGavinMoving = false;
            soundExists = false;
            soundTimer = 0;
        }
        //gavinsPreviousPosition = gavinsCurrentPosition;
    }

    /* a function that adds a noise and the time that it was created in the noisePositions list and it removes noises that were created more than 
     * noiseDuration ago */
    void HandleSoundPosition()
    {
        //List<List<object>> noisesToBeRemoved = new List<List<object>>();
        //timer += Time.deltaTime;
        //if (isGavinMoving)
        //{
        //    noisePositions.Add(new List<object> {gavin.transform.position, timer});
        //}
        //foreach (List<object> noiseInfo in  noisePositions)
        //{
        //    if ((timer - (float)noiseInfo[1]) > noiseDuration)
        //    {
        //        noisesToBeRemoved.Add(noiseInfo);
        //    }
        //}
        //foreach (List<object> noiseInfo in noisesToBeRemoved)
        //{
        //    noisePositions.Remove(noiseInfo);
        //}
        //noisesToBeRemoved.Clear();

        soundTimer += Time.deltaTime;
        if (isGavinMoving && (soundTimer > 1 / soundFrequency))
        {
            //soundExists = true;
            lastSoundPosition = gavin.transform.position;
            soundTimer = 0;
        }
        //else if (soundTimer > 1 / soundFrequency)
        //{
        //    soundExists = false;
        //    soundTimer = 0;
        //}
    }

    //public void NewNoise(float loudness){
    //    foreach (GameObject enemy in enemies)
    //    {
    //        enemy.GetComponent<EnemyStateMachineManager>().NewNoise(loudness, gavin.transform.position);   
    //    }

    //   // HandleNoisePositions();
    //}

    private void DeathControl()
    {
        if (gavinStats.health <= 0)
        {
            gavin.SetActive(false);
        }
        if (enemyStats.health <= 0)
        {
            enemy.SetActive(false);
        }
    }

    private void GameOverCondition()
    {
        if (!gavin.activeSelf)
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