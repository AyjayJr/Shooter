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
        setRigidbodyState(true);
        setColliderState(false);    
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

    void setRigidbodyState(bool state)
    {

        Rigidbody[] rigidbodies = GetComponentsInChildren<Rigidbody>();

        foreach (Rigidbody rigidbody in rigidbodies)
        {
            rigidbody.isKinematic = state;
        }

        GetComponent<Rigidbody>().isKinematic = !state;

    }


    void setColliderState(bool state)
    {

        Collider[] colliders = GetComponentsInChildren<Collider>();

        foreach (Collider collider in colliders)
        {
            collider.enabled = state;
        }

        GetComponent<Collider>().enabled = !state;

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
