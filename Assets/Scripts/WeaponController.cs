using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class WeaponController : MonoBehaviour
{
    private enum Weapons
    {
        Pistol,
        Gauss,
        Rifle
    }

    [Header("Shooting")]
    [SerializeField] private Camera cam; 
    [SerializeField] private float fireRate;
    [SerializeField] private float damage = 5f;
    [SerializeField] private float nextTimeToFire = 0f;
    [SerializeField] private float range = 100f;
    [SerializeField] private float impactForce = 3f;
    [SerializeField] private ParticleSystem muzzleFlash;
    [SerializeField] private GameObject impactEffect;
    [SerializeField] private LayerMask shootableLayerMask;
    [SerializeField] private GrapplingScript grapple;
    private Weapons selectedWeapon;
    private Weapons previousSelectedWeapon;
    public float soundRadius = 10;

    [Header("Gun Bob")]
    [SerializeField] private Transform weaponHolder;
    
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

    // gauss cannon variables
    private float lastShot;
    [HideInInspector] public float charge;
    [HideInInspector] public bool charging;
    [SerializeField] private Rigidbody player;
    private PlayerMovementAdvanced pScript;
    [SerializeField] private float recoilForce;
    [SerializeField] public LineRenderer laser;
    private float rateOfFire = 2;
    [Header("ChargeMeter")]
    [SerializeField] public Transform chargeMeter;    
    private bool fired = false;
    [HideInInspector] public Coroutine uncharge;
    [HideInInspector] public Coroutine laserShot;

    private bool unlockedGauss = false;
    private bool unlockedARfile = false;
    private AudioSource chargeSFX;
    private bool isPlayingCharge = false;

    void Start()
    {
        chargeSFX = GetComponent<AudioSource>();
        SelectWeapon();
        chargeMeter.localScale = new Vector3(1, 0, 1);
        lastShot = 0;
        pScript = player.gameObject.GetComponent<PlayerMovementAdvanced>();
        Pickup.onPickup += UnlockedWeapon;
        if (SceneManager.GetActiveScene().name != "Tutorial")
        {
            unlockedARfile = true;
            unlockedGauss = true;
        }
    }

    void FixedUpdate()
    {
        Vector3 target = new Vector3(xPos, yPos, 0);
        Vector3 desiredPos = Vector3.SmoothDamp(weaponHolder.localPosition, target, ref smoothV, 0.1f);
        weaponHolder.localPosition = desiredPos;

        if(fired)
        {
            pScript.recoilFlag = true;
            laserShot = StartCoroutine(LaserBeam());
            player.AddForce(cam.transform.forward * -(recoilForce * (charge/1.5f)), ForceMode.Force);
            fired = false;
        }
    }
    
    // Update is called once per frame
    void Update()
    {
        if (!PlayerManager.Instance.isAlive) return;
        
        previousSelectedWeapon = selectedWeapon;

        if (GameManager.Instance.IsPaused) return;

        // primary mouse button, maybe change this later
        if (selectedWeapon == Weapons.Pistol)
        {
            damage = 5f;
            fireRate = 3f;
            if (Input.GetMouseButton(0) && Time.time >= nextTimeToFire)
            {
                nextTimeToFire = Time.time + 1f / fireRate;
                Shoot();
            }
        }

        if (selectedWeapon == Weapons.Gauss)
        {
            if(Input.GetMouseButton(0) && Time.time >= lastShot)
            {
                if (!isPlayingCharge)
                {
                    chargeSFX.Play();
                    isPlayingCharge = true;
                }
                charging = true;
                charge += Time.deltaTime;
                charge = Mathf.Clamp(charge, 0, 1.5f);
                chargeMeter.localScale = new Vector3(1, charge/1.5f, 1);
                damage = 100f * charge;
            }

            // primary mouse button, maybe change this later
            if (Input.GetMouseButtonUp(0) && charging)
            {
                charging = false;
                lastShot = Time.time + rateOfFire;
                fired = true;
                Shoot();
                uncharge = StartCoroutine(DeCharge());
            }
        }
        else if (selectedWeapon == Weapons.Rifle)
        {
            damage = 5f;
            fireRate = 10f;
            if (Input.GetMouseButton(0) && Time.time >= nextTimeToFire)
            {
                nextTimeToFire = Time.time + 1f / fireRate;
                Shoot();
            }
        }

		// these next two ifs make the mouse wheel scroll loop through weapon selections
        if (!GameManager.Instance.disableScrollWheel)
        {
            if (Input.GetAxis("Mouse ScrollWheel") > 0f && (unlockedGauss || unlockedARfile))
            {
                if ((int)selectedWeapon >= transform.childCount - 1)
                {
                    selectedWeapon = Weapons.Pistol;
                }
                else
                {
                    selectedWeapon++;
                }
                if (selectedWeapon == Weapons.Rifle && !unlockedARfile)
                    selectedWeapon = Weapons.Gauss;
            }

            if (Input.GetAxis("Mouse ScrollWheel") < 0f && (unlockedGauss || unlockedARfile))
            {
                if (selectedWeapon <= 0 && unlockedARfile)
                {
                    selectedWeapon = Weapons.Rifle;
                }
                else
                {
                    selectedWeapon--;
                }
                if ((int)selectedWeapon == -1 && !unlockedARfile)
                    selectedWeapon = Weapons.Pistol;
            }
        }

		
        // map weapons to num keys 1 and 2 with the possibility for more
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            selectedWeapon = Weapons.Pistol;
        }
        if (Input.GetKeyDown(KeyCode.Alpha2) && transform.childCount >= 2 && unlockedGauss)
        {
            selectedWeapon = Weapons.Gauss;
        }
        if (Input.GetKeyDown(KeyCode.Alpha3) && transform.childCount >= 2 && unlockedARfile)
        {
            selectedWeapon = Weapons.Rifle;
        }

        if (previousSelectedWeapon != selectedWeapon)
        {
            SelectWeapon();
        }
        
        // Weapon sway and bob
        Gunbob();
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
    void AlertEnemies()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, soundRadius);
        foreach (Collider collider in colliders)
        {
            EnemyController enemy = collider.GetComponent<EnemyController>();

            if (enemy != null)
            {
                enemy.PlayerShootingAlert();
            }

        }     
    }
    void Shoot()
    {
        AlertEnemies();
        muzzleFlash.Play();
        if (selectedWeapon == Weapons.Pistol)
            SoundManager.Instance.PlaySFXOnce(SoundManager.GameSounds.PlayerPistolShoot);
        if (selectedWeapon == Weapons.Gauss)
            SoundManager.Instance.PlaySFXOnce(SoundManager.GameSounds.FireGauss);
        if (selectedWeapon == Weapons.Rifle)
            SoundManager.Instance.PlaySFXOnce(SoundManager.GameSounds.FireARifle);

        RaycastHit hit;
        if (Physics.Raycast(cam.transform.position, cam.transform.forward, out hit, range, shootableLayerMask))
        {
            Target target = hit.transform.GetComponent<Target>();
            EnemyController enemy = hit.transform.GetComponent<EnemyController>();
            ExploderController exploder = hit.transform.GetComponent<ExploderController>();
            ScoutDroidController flyEnemy = hit.transform.GetComponent<ScoutDroidController>();
            BossController bossController = hit.transform.GetComponent<BossController>();

            if (target != null)
            {
                target.TakeDamage(damage);
            }
            if (enemy != null)
            {
                enemy.TakeDamage(damage);
            }
            if (bossController != null)
            {
                bossController.TakeDamage(damage);
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

    // When we get here selected weapon is the new weapon and previousSelectedWeapon 
    void SelectWeapon()
    {
        int i = 0;
        foreach (Transform weapon in transform)
        {
            if (i == (int)selectedWeapon)
            {
                weapon.gameObject.SetActive(true);
            } else {
                weapon.gameObject.SetActive(false);
            }
            i++;
        }

        // cancel grapple coroutines if they're active
        if(grapple.PullTowards() != null)
        {
            StopCoroutine(grapple.PullTowards());
            Destroy(grapple.joint);
            grapple.isDeployed = false;
            grapple.lr.positionCount = 0;
            grapple.player.GetComponent<ConstantForce>().force = Vector3.zero;
            grapple.limb.rotation = grapple.pCamera.rotation;
        }
        else if(grapple.rotate != null)
        {
            StopCoroutine(grapple.rotate);
            grapple.limb.rotation = grapple.pCamera.rotation;
        }

        // cancel gauss coroutines if they're active
        if(uncharge != null)
        {
            StopCoroutine(uncharge);
            charge = 0;
            charging = false;
            chargeMeter.localScale = new Vector3(1, 0, 1);
        }
        else if(laserShot != null)
        {
            StopCoroutine(laserShot);
            laser.enabled = false;
        }
        else if (charge > 0)
        {
            charge = 0;
            charging = false;
            chargeMeter.localScale = new Vector3(1, 0, 1);
        }
    }
    
    IEnumerator DeCharge()
    {
        if (isPlayingCharge)
        {
            chargeSFX.Stop();
            isPlayingCharge = false;
        }
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

    IEnumerator LaserBeam()
    {
        Vector3 startDestination = laser.transform.position;
        Vector3 finalDestination = (laser.transform.forward * range + laser.transform.position);

        laser.positionCount = 2;
        laser.SetPosition(0, startDestination);
        laser.SetPosition(1, finalDestination);

        while(Vector3.Distance(startDestination, finalDestination) >= 2f)
        {
            startDestination = Vector3.Lerp(startDestination, finalDestination, 0.05f);

            laser.SetPosition(0, startDestination);

            yield return null;
        }

        laser.positionCount = 0;
    }

    private void UnlockedWeapon(Pickup.PickedUpType pickedUpType)
    {
        switch (pickedUpType)
        {
            case Pickup.PickedUpType.GuassCannon:
                {
                    unlockedGauss = true;
                    break;
                }
            case Pickup.PickedUpType.AssaultRifle:
                {
                    unlockedARfile = true;
                    break;
                }
        }
    }
}
