using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Platform : MonoBehaviour
{   
    public bool hasExploded = false;
    public const float PLATFORM_REGEN = 10f;
    float regenTimer = PLATFORM_REGEN;

    public GameObject explosionEffect;

    void Update()
    {
        if (hasExploded)
        {
            regenTimer -= Time.deltaTime;
            if (regenTimer < 0)
            {
                regenTimer = PLATFORM_REGEN;
                hasExploded = false;
                gameObject.SetActive(true);
            }
        }
    }

    public void Explode()
    {
        Instantiate(explosionEffect, transform.position, transform.rotation);
        gameObject.SetActive(false);
        hasExploded = true;
    }
}
