using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Target : MonoBehaviour
{
    public float health = 50f;
    public Action death;
 
    public void TakeDamage(float amount)
    {
        health -= amount;
        if (health <= 0f)
        {
            Die();
        }
    }


    void Die()
    {
          Destroy(gameObject, 0.05f);  
    }

    public void DamagePlayer(float damage)
    {
        PlayerManager.Instance.LoseHealth(damage);
    }
}
