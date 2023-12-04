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
    public EnemySight enemySight;
    public Image[] imageComponents;
    public Image imageComponent;
    public Color c = Color.green;
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

        enemySight = character.GetComponent<EnemySight>();
        imageComponents = GetComponentsInChildren<Image>();
        foreach(Image image in imageComponents)
        {
            if(image.gameObject.name == "Fill")
            {
                imageComponent = image;
            }
        }
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
            c = Color.Lerp(Color.green, Color.red, enemySight.alertLevel / 100f);
            imageComponent.color = c;
        }

        

    }
}
