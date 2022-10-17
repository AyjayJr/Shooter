using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Target : MonoBehaviour
{
    public float health = 50f;
    public Animator animator;
    public UnityEngine.AI.NavMeshAgent agent;
    EnemyController enemyController;

    void Start()
    { 
        enemyController = GetComponent<EnemyController>();
    }

    public void TakeDamage(float amount)
    {
        health -= amount;
        Debug.Log(health);
        if (health <= 0f)
        {
            Die();
        }
    }


    void Die()
    {
        if (enemyController != null)
        {
            enemyController.Die();
            
        }
        else
        {
            Destroy(gameObject);
            Destroy(agent);
        }
    }
}
