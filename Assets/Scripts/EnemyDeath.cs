using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyDeath : MonoBehaviour
{
    public float health = 50f;
    public Animator animator;
    public void TakeDamage(float amount)
    {
        health -= amount;
        Debug.Log("died");
        if (health <= 0f)
        {
            Die();
        }
    }

    void Die()
    {
        Debug.Log("died");
        animator.SetBool("IsDead", true);
    }
}
