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

    void FaceTarget()
    {
        Vector3 direction = (target.position - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);
    }

    public void Die()
    {
        animator.SetBool("isWalking", false);
        animator.SetTrigger("dead");
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
