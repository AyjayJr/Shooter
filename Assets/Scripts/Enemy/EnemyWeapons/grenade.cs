using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class grenade : MonoBehaviour
{
    public float delay = 4.0f;
    float countDown;
    bool hasExploded;
    public GameObject explosionEffect;
    public float explosionDamage = 60f;
    public float explosionRadius = 10f;
    public float explosionForce = 400f;
    Target target;

    // Start is called before the first frame update
    void Start()
    {
        countDown = delay;
        target = GetComponent<Target>();
    }

    // Update is called once per frame
    void Update()
    {
        if (target.health <= 0)
        {
            Explode();
        }
        countDown -= Time.deltaTime;
        if (countDown < 0f && !hasExploded)
        {
            Explode();
            hasExploded = true;
        }
    }


    public void Explode()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, explosionRadius);
        foreach (Collider collider in colliders)
        {
            Rigidbody rb = collider.GetComponent<Rigidbody>();
            EnemyController enemy = collider.GetComponent<EnemyController>();
            PlayerTarget target = collider.GetComponent<PlayerTarget>();

            if (enemy != null)
            {
                enemy.TakeDamage(explosionDamage);
            }
            if (rb != null)
            {
                rb.AddExplosionForce(explosionForce, transform.position, explosionRadius);
            }
            if (target != null)
            {
                target.DamagePlayer(explosionDamage);
            }

        }
        GameObject explosion = Instantiate(explosionEffect, transform.position, transform.rotation);
        Destroy(gameObject);
        Destroy(explosion, 1.5f);
    }
    
}
