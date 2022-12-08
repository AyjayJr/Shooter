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
    public float damage = 7.0f;
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
        accumulatedTime = 0.05f;
    }

    void FireBullet()
    {
        tracerEffect.Clear();
        tracerEffect.emitting = true;
        var tracer = Instantiate(tracerEffect, ray.origin, Quaternion.identity);
        muzzleFlash.Emit(1);
        GetComponent<AudioSource>().Play();
        tracer.AddPosition(rayCastOrigin.position);

        ray.origin = rayCastOrigin.position;
        ray.direction = rayCastOrigin.forward;
        Vector3 randomVar = Random.insideUnitSphere * inaccuracy;
        ray.direction += randomVar;

        if (Physics.Raycast(ray, out hit))
        {
            tracer.transform.position = hit.point;

            PlayerTarget target = hit.transform.GetComponent<PlayerTarget>();
            if (target != null)
            {
                target.DamagePlayer(this.damage);
            }
        }
        tracerEffect.emitting = false;
        Destroy(tracer.gameObject, 0.6f);

    }

    public void StopFiring()
    {
        isFiring = false;
    }
}
