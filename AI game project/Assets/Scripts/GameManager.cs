using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public GameObject gavin;
    public GameObject enemy;
    public bool isGavinMoving = false;
    public List<Vector3> noisePositions = new List<Vector3>();
    public GavinStats gavinStats;
    public EnemyStats enemyStats;

    private bool gavinWon = false;
    private bool enemiesWon = false;
    public Text gameOverText;  // text that will be displayer when the game is over
    
    void Start()
    {
        gavinStats = gavin.GetComponent<GavinStats>();
        enemyStats = enemy.GetComponent<EnemyStats>();

        gameOverText.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        DeathControl();
        GameOverCondition();
        if (gavinWon || enemiesWon)
        {
            DisplayGameOverScreen();
        }
    }

    public void newNoise(){
    	/* for tutti i nemici, call newnoise with gavin position
    	questa funzione viene chiamata da gavin quando fa rumore*/
    }

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