using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// This will need to be completelly changed
public class ExploreState : State
{
    public GameObject gavin;
    public GameObject enemy;
    public GavinAttackState attackState;
    public float maxDistance = 1.0f;

    public override State RunCurrentState()
    {
        float distance = Vector3.Distance(gavin.transform.position, enemy.transform.position);
        if (distance <= maxDistance)
        {
            return attackState;
        }
        return this;
    }
}
