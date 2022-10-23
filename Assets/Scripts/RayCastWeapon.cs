using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RayCastWeapon : MonoBehaviour
{
    public bool isFiring;
    public ParticleSystem muzzleFlash;
    public Transform rayCastOrigin;
    public int fireRate = 1;
    Ray ray;
    RaycastHit hit;
    public TrailRenderer tracerEffect;
    float accumulatedTime;

    // Start is called before the first frame update
    public void UpdateFiring(float deltaTime)
    {
        
        float fireInterval =  1.0f / fireRate;
        accumulatedTime -= deltaTime;

        if (accumulatedTime <= 0.0f)
        {
            FireBullet();            
            accumulatedTime += fireInterval;
        }
    }

    public void StartFiring()
    {
        isFiring = true;
        accumulatedTime = 0f;
        FireBullet();
    }

    void FireBullet()
    {
        muzzleFlash.Emit(1);
        var tracer = Instantiate(tracerEffect, ray.origin, Quaternion.identity);
        tracer.AddPosition(ray.origin);

        ray.origin = rayCastOrigin.position;
        ray.direction = rayCastOrigin.forward;
        if (Physics.Raycast(ray, out hit))
        {
            tracer.transform.position = hit.point;
        }

    }


    // Update is called once per frame
    public void StopFiring()
    {
        isFiring = false;
    }
}
