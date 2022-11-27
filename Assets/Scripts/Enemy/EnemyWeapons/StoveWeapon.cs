using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoveWeapon : MonoBehaviour
{
    public float explosionRadius = 6.0f;
    public float explosionForce = 2000.0f;
    public float explosionDamage = 30.0f;
    Target self;
    private void Start()
    {
        self = GetComponent<Target>();

    }

    // Update is called once per frame
    void Update()
    {
        if (self.health <= 0)
        {
            Explode();
        }
    }
    private void OnCollisionEnter(Collision collision)
    {
        bool hasExploded = false;
        Debug.Log("Collision");
        Collider[] colliders = Physics.OverlapSphere(transform.position, explosionRadius);
        foreach (Collider collider in colliders)
        {
            Rigidbody rb = collider.GetComponent<Rigidbody>();
            EnemyController enemy = collider.GetComponent<EnemyController>();
            PlayerTarget target = collider.GetComponent<PlayerTarget>();
            Platform targetPlatform = collider.GetComponent<Platform>();
            if (enemy != null)
            {
                hasExploded = true;
                enemy.TakeDamage(explosionDamage);
            }
            if (rb != null)
            {
                rb.AddExplosionForce(explosionForce, transform.position, explosionRadius);
            }
            if (target != null)
            {
                hasExploded = true;
                target.DamagePlayer(explosionDamage);
            }
            if (targetPlatform != null)
            {
                hasExploded = true;
                targetPlatform.Explode();
            }

        }
        if (hasExploded)
        {
            Instantiate(explosionEffect, transform.position, transform.rotation);
            Destroy(gameObject);
            Explode();
        }
        
    }
    public GameObject explosionEffect;

    public void Explode()
    {
        Instantiate(explosionEffect, transform.position, transform.rotation);
        Destroy(gameObject);
    }
}
