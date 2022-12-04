using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UIElements;

public class ExploderController : MonoBehaviour
{
    System.Random rand = new System.Random();
    public Transform target;
    public NavMeshAgent agent;
    public ExploderStateMachine stateMachine;
    public ExploderStateId initialState = ExploderStateId.Armed;
    public WayPointSystem wayPointSystem;
    public float visionRadius = 6.0f;
    public float blinkRate = 5.0f;
    public Light armedLight;
    public float armingRange = 7.0f;
    public float explosionRadius = 6.0f;
    public float activationRadius = 2.0f;
    public float explosionForce = 2000.0f;
    public float explosionDamage = 100.0f;
    public bool isDead = false;

    public GameObject explosionEffect;
    public SkinnedMeshRenderer meshRenderer;
    private BoxCollider boxCollider;
    public float health = 15.0f;
    float accumulatedTime;
    bool isBlinking = false;
    private Vector3 startingPos;
    private Quaternion startingRot;
    private float maxHealth;


    // Start is called before the first frame update
    void Start()
    {
        startingPos = transform.localPosition;
        startingRot = transform.localRotation;
        maxHealth = health;
        target = PlayerManager.Instance.player.transform;

        boxCollider = GetComponent<BoxCollider>();
        agent = GetComponent<NavMeshAgent>();
        armedLight = GetComponentInChildren<Light>();
        armedLight.enabled = false;
        stateMachine = new ExploderStateMachine(this);
        stateMachine.RegisterState(new ExploderChaseState());
        stateMachine.RegisterState(new ExploderPatrolState());
        stateMachine.RegisterState(new ExploderArmedState());
        stateMachine.RegisterState(new ExploderDeathState());


        stateMachine.ChangeState(initialState);

        GameManager.Instance.onRespawn += ResetAi;
    }

    // Update is called once per frame
    void Update()
    {
        stateMachine.Update();
        if (isBlinking)
        {
            BlinkLight(Time.deltaTime);
        }
    }

    private void OnDestroy()
    {
        GameManager.Instance.onRespawn -= ResetAi;
    }

    private void ResetAi()
    {
        startingPos = transform.localPosition;
        startingRot = transform.localRotation;
        health = maxHealth;
        isDead = false;

        meshRenderer.enabled = true;
        boxCollider.enabled = true;
        armedLight.enabled = true;
        target = PlayerManager.Instance.player.transform;
        agent = GetComponent<NavMeshAgent>();
        armedLight = GetComponentInChildren<Light>();
        armedLight.enabled = false;
        stateMachine = new ExploderStateMachine(this);
        stateMachine.RegisterState(new ExploderChaseState());
        stateMachine.RegisterState(new ExploderPatrolState());
        stateMachine.RegisterState(new ExploderArmedState());
        stateMachine.RegisterState(new ExploderDeathState());


        stateMachine.ChangeState(initialState);
    }

    public void StartBlinking()
    {
        accumulatedTime = 0;
        armedLight.enabled = true;
        isBlinking = true;
    }
    public void StopBlinking()
    {
        accumulatedTime = 0;
        armedLight.enabled = false;
        isBlinking = false;
    }
    public void BlinkLight(float deltaTime)
    {
        float blinkInterval = 1.0f / blinkRate;
        accumulatedTime -= deltaTime;

        if (accumulatedTime <= 0.0f)
        {
            armedLight.enabled = !armedLight.enabled;
            accumulatedTime += blinkInterval;
        }
    
        
    }

    public void Explode()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, explosionRadius);
        foreach(Collider collider in colliders)
        {
            Rigidbody rb = collider.GetComponent<Rigidbody>();
            EnemyController enemy = collider.GetComponent<EnemyController>();
            PlayerTarget target = collider.GetComponent<PlayerTarget>();
            
            if (enemy != null)
            {
                enemy.TakeDamage(explosionDamage);
            }
            if (rb != null)
            {
                rb.AddExplosionForce(explosionForce, transform.position, explosionRadius);
            }
            if (target != null)
            {
                target.DamagePlayer(explosionDamage);
            }
          
        }
        Instantiate(explosionEffect, transform.position, transform.rotation);
        this.enabled = false;
        meshRenderer.enabled = false;
        boxCollider.enabled = false;
        armedLight.enabled = false;
        isBlinking = false;
    }

    public WayPoint NextWayPoint(int currentWayPoint)
    {
        // find closest waypoints
        int randomIndex;
        if (rand.Next(3) == 0 && wayPointSystem.wayPoints != null)
        {
            randomIndex = rand.Next(wayPointSystem.wayPoints.Length);
            return wayPointSystem.wayPoints[randomIndex];
        }
        else
        {
            randomIndex = rand.Next(4);
        }
     
        return wayPointSystem.wayPoints[currentWayPoint].neighboirs[randomIndex];
    }

    public void TakeDamage(float amount)
    {
        health -= amount;
        if (health <= 0f)
        {
            stateMachine.ChangeState(ExploderStateId.Death);
        }
    }
}
