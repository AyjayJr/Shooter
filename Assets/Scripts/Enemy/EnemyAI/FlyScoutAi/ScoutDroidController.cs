using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ScoutDroidController : MonoBehaviour
{
    public static Action onSpawned;
    public static Action<bool> onDeath;
    public float visionRadius = 10f;
    public float shootingRange = 6f;
    public float YOffset = 2.5f;
    [HideInInspector]
    public Transform target;
    public float speed = 2f;
    public float health = 50f;
    public bool isDead = false;
    public FlyStateMachine stateMachine;
    public AiFlyStateId initialState = AiFlyStateId.Idle;
    public GameObject explosionEffect;
    public RayCastWeapon pistol1;
    public RayCastWeapon pistol2;
    public Material[] eyeEmmisionMaterials;
    public MeshRenderer sphereRef;
    [HideInInspector]
    public bool isFlying;
    [HideInInspector]
    public bool isAboveTarget;

    private Vector3 startingPos;
    private Quaternion startingRot;
    private float maxHealth;
    private bool revived = false;

    void Start()
    {
        startingPos = this.transform.localPosition;
        startingRot = this.transform.localRotation;
        maxHealth = health;
        target = PlayerManager.Instance.player.transform;

        stateMachine = new FlyStateMachine(this);
        stateMachine.RegisterState(new AiFlyChaseState());
        stateMachine.RegisterState(new AiFlyDeathState());
        stateMachine.RegisterState(new AiFlyIdleState());
        stateMachine.RegisterState(new AiFlyAttackState());

        stateMachine.ChangeState(initialState);

        GameManager.Instance.onRespawn += ResetAi;
        onSpawned?.Invoke();
    }

    // Update is called once per frame
    void Update()
    {
        stateMachine.Update();
        isFlying = !Physics.Raycast(transform.position, Vector3.down, YOffset);
        if (target != null)
            isAboveTarget = !Physics.Raycast(target.position, Vector3.down, YOffset);
    }

    private void LateUpdate()
    {
        if (pistol1.isFiring)
        {
            pistol1.UpdateFiring(Time.deltaTime);
        }
        if (pistol2.isFiring)
        {
            pistol2.UpdateFiring(Time.deltaTime);
        }
    }


    private void OnDestroy()
    {
        GameManager.Instance.onRespawn -= ResetAi;
    }

    private void ResetAi()
    {
        transform.localPosition = startingPos;
        transform.localRotation = startingRot;
        health = maxHealth;
        isDead = false;
        revived = true;
        GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
        GetComponent<Rigidbody>().useGravity = false;
        GetComponent<Rigidbody>().drag = 10f;
        target = PlayerManager.Instance.player.transform;

        stateMachine = new FlyStateMachine(this);
        stateMachine.RegisterState(new AiFlyChaseState());
        stateMachine.RegisterState(new AiFlyDeathState());
        stateMachine.RegisterState(new AiFlyIdleState());
        stateMachine.RegisterState(new AiFlyAttackState());

        stateMachine.ChangeState(initialState);
    }

    public void setColliderState(bool state)
    {

        Collider[] colliders = GetComponentsInChildren<Collider>();

        foreach (Collider collider in colliders)
        {
            collider.enabled = state;
        }
        Collider[] parentColliders = GetComponents<Collider>();

        GetComponent<Collider>().enabled = !state;
        foreach (Collider collider in parentColliders)
        {
            collider.enabled = !state;
        }
    }

    public void Attack()
    {
        pistol1.StartFiring();
        pistol2.StartFiring();
    }

    public void StopAttack()
    {
        pistol1.StopFiring();
        pistol2.StopFiring();
    }

    public void FaceTarget()
    {
        Vector3 direction = (target.position - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, direction.y, direction.z));
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);
    }

    public void Die()
    {
        if (isDead) return;
        GameObject explosion = Instantiate(explosionEffect, this.transform);
        GetComponent<AudioSource>().Play();
        explosion.transform.parent = null;
        GetComponent<Rigidbody>().drag = 1f;
        stateMachine.ChangeState(AiFlyStateId.Death);
        Destroy(explosion, 1.5f);
        isDead = true;
        onDeath?.Invoke(revived);
        this.enabled = false;
    }

    public void TakeDamage(float amount)
    {
        health -= amount;
        if (health <= 0f)
        {
            Die();
        }
    }
}
