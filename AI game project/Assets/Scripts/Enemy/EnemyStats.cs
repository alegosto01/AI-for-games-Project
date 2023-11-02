using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyStats : MonoBehaviour
{
    public float health;
    public float maxHealth = 100f;  // only useful if we want to implement health regeneration
    public float attackDamage = 5f;
    public float attackSpeed = 0.8f;
    public float speed = 3f;

    private void Start()
    {
        health = maxHealth;
    }
}
