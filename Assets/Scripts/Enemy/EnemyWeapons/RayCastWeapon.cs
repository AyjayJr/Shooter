using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RayCastWeapon : MonoBehaviour
{
    public bool isFiring;
    public ParticleSystem muzzleFlash;
    public Transform rayCastOrigin;
    Ray ray;
    RaycastHit hit;
    public TrailRenderer tracerEffect;
    float accumulatedTime;
    float damage = 7.0f;
    public float inaccuracy = 0.0f;
    public float fireRate = 1.4f;

    public void UpdateFiring(float deltaTime)
    {
        
        float fireInterval =  1.0f / fireRate;
        accumulatedTime -= deltaTime;

        if (accumulatedTime <= 0.0f && isFiring && PlayerManager.Instance.isAlive)
        {

            accumulatedTime = fireInterval;
            FireBullet();
        }
    }

    public void StartFiring()
    {
        isFiring = true;
        accumulatedTime = 0.5f;
    }

    void FireBullet()
    {
        muzzleFlash.Emit(1);
        GetComponent<AudioSource>().Play();
        var tracer = Instantiate(tracerEffect, ray.origin, Quaternion.identity);
        tracer.AddPosition(ray.origin);

        ray.origin = rayCastOrigin.position;
        ray.direction = rayCastOrigin.forward;
        Vector3 randomVar = Random.insideUnitSphere * inaccuracy;
        ray.direction += randomVar;


        if (Physics.Raycast(ray, out hit))
        {
            tracer.transform.position = hit.point;
            Debug.Log(tracer.material.ToString());
            Debug.Log(tracer.name.ToString());

            PlayerTarget target = hit.transform.GetComponent<PlayerTarget>();
            if (target != null)
            {
                target.DamagePlayer(this.damage);
            }
        }
        Destroy(tracer.gameObject, 0.5f);

    }

    public void StopFiring()
    {
        isFiring = false;
    }
}
