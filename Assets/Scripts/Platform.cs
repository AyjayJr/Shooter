using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Platform : MonoBehaviour
{
    public GameObject explosionEffect;

    private void OnCollisionEnter(Collision collision)
    {
        StoveWeapon stove = collision.collider.GetComponent<StoveWeapon>();
        if (stove != null)
        {
            Explode();
            stove.Explode();
        }

    }
    public void Explode()
    {
        Instantiate(explosionEffect, transform.position, transform.rotation);
        Destroy(gameObject);
    }
}
