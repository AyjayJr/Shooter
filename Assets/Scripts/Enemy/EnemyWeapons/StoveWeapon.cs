using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoveWeapon : MonoBehaviour
{
    public GameObject explosionEffect;

    public void Explode()
    {
        Instantiate(explosionEffect, transform.position, transform.rotation);
        Destroy(gameObject);
    }
}