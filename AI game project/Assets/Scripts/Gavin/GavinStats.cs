using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GavinStats : MonoBehaviour
{
    public float health;
    public float maxHealth = 100f;  // only useful if we want to implement health regeneration
    public float attackDamage = 10f;
    public float attackSpeed = 1.2f;
    public float speed = 4f;

    private void Start()
    {
        health = maxHealth;
    }
}
