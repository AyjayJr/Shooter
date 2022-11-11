using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GaussCannon : MonoBehaviour
{
    [Header("Shooting")]
    [SerializeField] private Camera cam; 
    [SerializeField] private float rateOfFire;
    private float lastShot;
    private float charge;
    private bool charging;
    [SerializeField] private float damage = 50f;
    [SerializeField] private float range = 100f;
    [SerializeField] private float impactForce = 3f;
    [SerializeField] private Rigidbody player;
    private PlayerMovementAdvanced pScript;
    [SerializeField] private float recoilForce;
    [SerializeField] private ParticleSystem muzzleFlash;
    [SerializeField] private GameObject impactEffect;

    [Header("ChargeMeter")]
    [SerializeField] private Transform chargeMeter;    

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
    
    private bool fired = false;

    void Start()
    {
        chargeMeter.localScale = new Vector3(1, 0, 1);
        lastShot = 0;
        pScript = player.gameObject.GetComponent<PlayerMovementAdvanced>();
    }

    // Update is called once per frame
    void Update()
    {
        WeaponSway();
        CalculateIdleSway();
        WalkMovement();

        if(Input.GetMouseButton(0) && Time.time >= lastShot)
        {
            charging = true;
            charge += Time.deltaTime;
            charge = Mathf.Clamp(charge, 0, 1.5f);
            chargeMeter.localScale = new Vector3(1, charge/1.5f, 1);
        }

        // primary mouse button, maybe change this later
        if (Input.GetMouseButtonUp(0) && charging)
        {
            charging = false;
            lastShot = Time.time + rateOfFire;
            fired = true;
            Shoot();
            StartCoroutine(DeCharge());
            //chargeMeter.localScale = new Vector3(1, 0, 1);
        }

    }

    void FixedUpdate()
    {
        if(fired)
        {
            pScript.recoilFlag = true;
            player.AddForce(cam.transform.forward * -(recoilForce * (charge/1.5f)), ForceMode.Force);
            //charge = 0;
            fired = false;
        }
    }

    void WalkMovement()
    {
    }

    void Shoot()
    {
        muzzleFlash.Play();
        //print(cam.transform.forward * -recoilForce);

        //player.AddForce(cam.transform.forward * -(recoilForce), ForceMode.Impulse);

        //player.AddForce(cam.GetComponent<Transform>().forward * -(AdjustRecoilStrength()), ForceMode.Impulse);
        //player.AddForce(Adjust(), ForceMode.Impulse);

        RaycastHit hit;
        if (Physics.Raycast(cam.transform.position, cam.transform.forward, out hit, range))
        {
            Target target = hit.transform.GetComponent<Target>();
            EnemyController enemy = hit.transform.GetComponent<EnemyController>();

            if (target != null)
            {
                target.TakeDamage(damage);
            }
            if (enemy != null)
            {
                enemy.TakeDamage(damage);
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

    IEnumerator DeCharge()
    {
        while(true)
        {
            if(Mathf.Approximately(chargeMeter.localScale.y, 0))
            {
                break;
            }

            charge -= Time.deltaTime;
            charge = Mathf.Clamp(charge, 0, 1.5f);
            chargeMeter.localScale = new Vector3(1, charge/1.5f, 1);

            yield return null;
        }
    }

    // private float AdjustRecoilStrength()
    // {
    //     float temp = cam.transform.eulerAngles.x;
    //     //print(temp);

    //     if((temp <= 30f && temp >= 0f) || (temp >= 330f && temp <= 360f))
    //     {
    //         //print("Horizontalish");
    //         return recoilForce * 1.3f;
    //     }
    //     else
    //     {
    //         return recoilForce * 0.7f;
    //     }
    // }

    // //STILL NEEDS MORE WORK
    // private Vector3 Adjust()
    // {
    //     Vector3 recoil = cam.transform.forward * -(recoilForce);

    //     //gets recoil X angle
    //     float angleInRadians = Mathf.Abs(180 + cam.transform.eulerAngles.x);

    //     float recoilX = recoil.z * Mathf.Cos(Mathf.Deg2Rad * angleInRadians);
    //     float recoilY = recoil.z * Mathf.Sin(Mathf.Deg2Rad * angleInRadians);

    //     print("Original: " + recoil + ". Angle, XCom and YCom of Z: " + angleInRadians + " " + recoilX + " " + recoilY);

    //     recoilY = recoilY * 0.7f;
    //     recoilX = recoilX * 1.3f;

    //     float newXForce = Mathf.Sqrt(Mathf.Pow(recoilX, 2) + Mathf.Pow(recoilY, 2));

    //     Debug.Log(new Vector3(recoil.x, recoilY, recoilX));
    //     return new Vector3(recoil.x, recoil.y - recoilY, -recoilX);
    // }
}
