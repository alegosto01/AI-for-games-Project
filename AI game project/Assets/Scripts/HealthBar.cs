using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    public GameObject character;
    private GavinStats gavinStats;
    private EnemyStats enemyStats;

    private Slider slider;

    [SerializeField] private Transform canvas;

    private void Start()
    {
        if (character.tag == "Player")
        {
            gavinStats = character.GetComponent<GavinStats>();
        }
        else
        {
            enemyStats = character.GetComponent<EnemyStats>();
        }

        slider = GetComponent<Slider>();
    }

    private void Update()
    {
        canvas.rotation = Quaternion.Euler(0, 0, 0);
        if (character.tag == "Player")
        {
            slider.value = gavinStats.health / 100f;
        }
        else
        {
            slider.value = enemyStats.health / 100f;
        }
    }
}
