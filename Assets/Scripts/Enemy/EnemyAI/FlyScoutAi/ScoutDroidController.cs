using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ScoutDroidController : MonoBehaviour
{
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

    void Start()
    {
        target = PlayerManager.Instance.player.transform;

        stateMachine = new FlyStateMachine(this);
        stateMachine.RegisterState(new AiFlyChaseState());
        stateMachine.RegisterState(new AiFlyDeathState());
        stateMachine.RegisterState(new AiFlyIdleState());
        stateMachine.RegisterState(new AiFlyAttackState());

        stateMachine.ChangeState(initialState);
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
        GameObject explosion = Instantiate(explosionEffect, this.transform);
        GetComponent<AudioSource>().Play();
        explosion.transform.parent = null;
        GetComponent<Rigidbody>().drag = 1f;
        stateMachine.ChangeState(AiFlyStateId.Death);
        Destroy(explosion, 1.5f);
        Destroy(this);
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
