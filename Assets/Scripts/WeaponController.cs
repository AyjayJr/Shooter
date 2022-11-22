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
    [SerializeField] private LayerMask shootableLayerMask;
    
    [Header("Weapon Bob")]
    [SerializeField] private Transform weapon;
    [System.Serializable]
    public struct BobOverride
    {
        public float minSpeed;
        public float maxSpeed;

        [Header("X Settings")]
        public float speedX;
        public float intenstityX;
        public AnimationCurve bobX;

        [Header("Y Settings")]
        public float speedY;
        public float intenstityY;
        public AnimationCurve bobY;
    }

    public BobOverride[] bobOverrides;
    private float currentTimeX;
    private float currentTimeY;
    private float xPos;
    private float yPos;
    private Vector3 smoothV;
    public float swayIntensityX;
    public float swayIntensityY;
    public float maxSway;
    public float minSway;
    public float currentSpeed;

    void FixedUpdate()
    {
        Vector3 target = new Vector3(xPos, yPos, 0);
        Vector3 desiredPos = Vector3.SmoothDamp(weapon.localPosition, target, ref smoothV, 0.1f);
        weapon.localPosition = desiredPos;
    }
    

    // Update is called once per frame
    void Update()
    {
        if (!PlayerManager.Instance.isAlive) return;

        Gunbob();

        // primary mouse button, maybe change this later
        if (Input.GetMouseButtonDown(0) && !GameManager.Instance.IsPaused)
        {
            Shoot();
        }

    }

    void Gunbob()
    {
        foreach (BobOverride bob in bobOverrides)
        {
            if (currentSpeed >= bob.minSpeed && currentSpeed <= bob.maxSpeed)
            {
                float bobMultiplier = (currentSpeed == 0) ? 1 : currentSpeed;

                currentTimeX += bob.speedX / 10 * Time.deltaTime * bobMultiplier;
                currentTimeY += bob.speedY / 10 * Time.deltaTime * bobMultiplier;

                xPos = bob.bobX.Evaluate(currentTimeX) * bob.intenstityX;
                yPos = bob.bobY.Evaluate(currentTimeY) * bob.intenstityY;
            }
        }

        float xSway = -Input.GetAxis("Mouse X") * swayIntensityX;
        float ySway = -Input.GetAxis("Mouse Y") * swayIntensityY;

        xSway = Mathf.Clamp(xSway, minSway, maxSway);
        ySway = Mathf.Clamp(ySway, minSway, maxSway);

        xPos += xSway;
        yPos += ySway;
    }

    void Shoot()
    {
        muzzleFlash.Play();
        SoundManager.Instance.PlaySFXOnce(SoundManager.GameSounds.PlayerPistolShoot);

        RaycastHit hit;
        if (Physics.Raycast(cam.transform.position, cam.transform.forward, out hit, range, shootableLayerMask))
        {
            Target target = hit.transform.GetComponent<Target>();
            EnemyController enemy = hit.transform.GetComponent<EnemyController>();
            ExploderController exploder = hit.transform.GetComponent<ExploderController>();
            ScoutDroidController flyEnemy = hit.transform.GetComponent<ScoutDroidController>();


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
            if (flyEnemy != null)
            {
                flyEnemy.TakeDamage(damage);
            }
            if (hit.rigidbody != null)
            {
                hit.rigidbody.AddForce(-hit.normal * impactForce);
            }

            GameObject impact = Instantiate(impactEffect, hit.point, Quaternion.LookRotation(hit.normal));
            Destroy(impact, 2f);
        }
    }

}