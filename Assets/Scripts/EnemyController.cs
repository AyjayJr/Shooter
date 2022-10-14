using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour
{
    public float visionRadius = 10f;
    public float shootingRange = 6f;
    Transform target;
    NavMeshAgent agent;
    public Animator animator;
    public float health = 50f;
    bool isDead = false;


    // Start is called before the first frame update
    void Start()
    {
        target = PlayerManager.instance.player.transform;
        agent = GetComponent<NavMeshAgent>();
        setRigidbodyState(true);
        setColliderState(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (!isDead)
        {
            float distance = Vector3.Distance(target.position, transform.position);
            if (distance <= visionRadius)
            {
                animator.SetBool("isWalking", true);
                agent.SetDestination(target.position);
                FaceTarget();
            }
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
        Collider[] parentColliders = GetComponents<Collider>();

        GetComponent<Collider>().enabled = !state;
        foreach (Collider collider in parentColliders)
        {
            collider.enabled = !state;
        }
    }

    void FaceTarget()
    {
        Vector3 direction = (target.position - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);
    }

    public void Die()
    {
        animator.enabled = false;
        setRigidbodyState(false);
        setColliderState(true);
        isDead = true;
        Destroy(agent);

    }

    public void TakeDamage(float amount)
    {
        health -= amount;
        Debug.Log(health);
        if (health <= 0f)
        {
            Debug.Log("dead");
            Die();
        }
    }
}
