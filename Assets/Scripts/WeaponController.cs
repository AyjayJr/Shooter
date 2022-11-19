using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponController : MonoBehaviour
{
    [Header("Shooting")]
    [SerializeField] private Camera cam; 
    [SerializeField] private float damage = 10f;
    [SerializeField] private float range = 100f;
    [SerializeField] private float impactForce = 3f;
    [SerializeField] private ParticleSystem muzzleFlash;
    [SerializeField] private GameObject impactEffect;
    

    [Header("Weapon Sway")]
    [SerializeField] private float smooth;
    [SerializeField] private float multiplier;

    [Header("Weapon Bob")]
    [SerializeField] private float gunBobAmtX;
    [SerializeField] private float gunBobAmtY;
    private float currentBobX;
    private float currentBobY;

    [Header("Idle Sway")]
    [SerializeField] private Transform weapon;

    [SerializeField] private float idleA = 1;
    [SerializeField] private float idleB = 2;
    [SerializeField] private float swayScale = 600;
    [SerializeField] private float swayLerpSpeed = 14;

    [SerializeField] private float swayTime;
    [SerializeField] private Vector3 swayPosition;
    

    // Update is called once per frame
    void Update()
    {
        if (!PlayerManager.Instance.isAlive) return;

        WeaponSway();
        CalculateIdleSway();
        WalkMovement();

        // primary mouse button, maybe change this later
        if (Input.GetMouseButtonDown(0) && !GameManager.Instance.IsPaused)
        {
            Shoot();
        }

    }

    void WalkMovement()
    {
    }

    void Shoot()
    {
        muzzleFlash.Play();
        SoundManager.Instance.PlaySFXOnce(SoundManager.GameSounds.PlayerPistolShoot);

        RaycastHit hit;
        if (Physics.Raycast(cam.transform.position, cam.transform.forward, out hit, range))
        {
            Debug.Log(hit.transform.name);

            Target target = hit.transform.GetComponent<Target>();
            EnemyController enemy = hit.transform.GetComponent<EnemyController>();
            ExploderController exploder = hit.transform.GetComponent<ExploderController>();


            if (target != null)
            {
                target.TakeDamage(damage);
            }
            if (enemy != null)
            {
                enemy.TakeDamage(damage);
            }
            if (exploder != null)
            {
                exploder.TakeDamage(damage);
            }
            if (hit.rigidbody != null)
            {
                hit.rigidbody.AddForce(-hit.normal * impactForce);
            }

            GameObject impact = Instantiate(impactEffect, hit.point, Quaternion.LookRotation(hit.normal));
            Destroy(impact, 2f);
        }
    }

    void WeaponSway()
    {
       float mouseX = Input.GetAxisRaw("Mouse X") * multiplier; 
       float mouseY = Input.GetAxisRaw("Mouse Y") * multiplier; 

       Quaternion rotationX = Quaternion.AngleAxis(-mouseY, Vector3.right);
       Quaternion rotationY = Quaternion.AngleAxis(mouseX, Vector3.up);
       Quaternion targetRotation = rotationX * rotationY;

       transform.localRotation = Quaternion.Slerp(transform.localRotation, targetRotation, smooth * Time.deltaTime);
    }

    void CalculateIdleSway()
    {
        var targetPosition = LissajousCurve(swayTime, idleA, idleB) / swayScale;
        swayPosition = Vector3.Lerp(swayPosition, targetPosition, Time.smoothDeltaTime * swayLerpSpeed);
        swayTime += Time.deltaTime;

        if (swayTime > 6.3f)
        {
            swayTime = 0;
        }

        weapon.localPosition = swayPosition;
    }

    Vector3 LissajousCurve(float Time, float A, float B)
    {
        return new Vector3(Mathf.Sin(Time), A * Mathf.Sin(B * Time + Mathf.PI));
    }
}
