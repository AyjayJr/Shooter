using System;
using System.Collections;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Animations.Rigging;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class EnemyController : MonoBehaviour
{
    public static Action onSpawned;
    public static Action<bool> onDeath;
    public float visionRadius = 10f;
    public float shootingRange = 6f;
    public Transform target;
    public Transform targetOrientation;
    public NavMeshAgent agent;
    public Animator animator;
    public float health = 50f;
    public bool isDead = false;
    public AiStateMachine stateMachine;
    public AiStateId initialState = AiStateId.Idle;
    public float chaseSpeed = 8.0f;
    public float fleeingSpeed = 12.0f;
    public float strafeSpeed = 4.5f;
    public float fleeDistance = 2f;
    public bool isRolling = false;

    
    public int grenades = 2;
    public float throwForce = 50f;
    public float maxThrowForce = 1000f;
    public float autoAlertRadius = 2.5f;
    public GameObject grenadeObject;
    public float armingRange = 6.0f;
    public Transform grenadeThrowPoint;
    public EnemySensor sensor;
    RayCastWeapon weapon;
    RigBuilder rigs;

    public bool wasChasingPlayer = false;
    private float maxHealth;
    private Vector3 startingPos;
    private Quaternion startingRot;
    private Vector3 wStartingPos;
    private Quaternion wStartingRot;
    private bool revived = false;
    public GameObject vanishEffect;
  


    // Start is called before the first frame update
    void Start()
    {
        startingPos = transform.localPosition;
        startingRot = transform.localRotation;
        maxHealth = health;
        target = PlayerManager.Instance.player.transform;
        targetOrientation = PlayerManager.Instance.orientation;
        sensor = GetComponent<EnemySensor>();
        rigs = GetComponent<RigBuilder>();
        weapon = GetComponentInChildren<RayCastWeapon>();
        wStartingPos = weapon.transform.localPosition;
        wStartingRot = weapon.transform.localRotation;
        SetRigBuilder();

        agent = GetComponent<NavMeshAgent>();
        agent.speed = 6f;
        agent.updatePosition = true;
        agent.updateRotation = false;
        animator = GetComponent<Animator>();
        animator.applyRootMotion = false;

        setRigidbodyState(true);
        setColliderState(false);
        stateMachine = new AiStateMachine(this);
        stateMachine.RegisterState(new AiChaseState());
        stateMachine.RegisterState(new AiDeathState());
        stateMachine.RegisterState(new AiIdleState());
        stateMachine.RegisterState(new AiAttackState());
        stateMachine.RegisterState(new AIPlayerDeathState());
        stateMachine.RegisterState(new AiFleeState());


        stateMachine.ChangeState(initialState);

        GameManager.Instance.onRespawn += ResetAi;
        onSpawned?.Invoke();
    }

    // Update is called once per frame
    void Update()
    {

        stateMachine.Update();
        if (!PlayerManager.Instance.isAlive)
        {
            stateMachine.ChangeState(AiStateId.PlayerDeath);
        }
        if (isDead)
        {
            return;
        }

        if (sensor.objects.Count > 0)
        {
            foreach(UnityEngine.Object obj in sensor.objects)
            {
                grenade grenade = obj.GetComponent<grenade>();
                if (grenade != null && stateMachine.currentState != AiStateId.Flee)
                {
                    stateMachine.ChangeState(AiStateId.Flee);
                }
                
            }
        }
        
        bool shouldMove = (agent.velocity.magnitude > 0.5f
           && agent.remainingDistance > agent.stoppingDistance);
        animator.SetBool("Moving", shouldMove);

        if (stateMachine.currentState == AiStateId.Attack)
        {
            animator.SetFloat("speed_x", agent.velocity.x * (target.position - transform.position).normalized.x);
            animator.SetFloat("speed_y", agent.velocity.z * (target.position - transform.position).normalized.z);
        }
        else
        {
            animator.SetFloat("speed_x", agent.velocity.x * transform.right.normalized.x);
            animator.SetFloat("speed_y", agent.velocity.z * transform.forward.normalized.z);
        }




    }


    private void LateUpdate()
    {
        if (weapon.isFiring)
        {
            weapon.UpdateFiring(Time.deltaTime);
        }
    }

    private void OnDestroy()
    {
        GameManager.Instance.onRespawn -= ResetAi;
    }

    private void OnDisable()
    {
        GameManager.Instance.onRespawn -= ResetAi;
    }

    private void ResetAi()
    {
        transform.localPosition = startingPos;
        transform.localRotation = startingRot;
        weapon.transform.localPosition = wStartingPos;
        weapon.transform.localRotation = wStartingRot;
        health = maxHealth;
        isDead = false;
        revived = true;
        target = PlayerManager.Instance.player.transform;
        targetOrientation = PlayerManager.Instance.orientation;

        agent.enabled = true;
        agent.speed = 6f;
        agent.updatePosition = true;
        agent.updateRotation = false;
        animator.applyRootMotion = false;
        animator.enabled = true;

        setRigidbodyState(true);
        setColliderState(false);
        stateMachine = new AiStateMachine(this);
        stateMachine.RegisterState(new AiChaseState());
        stateMachine.RegisterState(new AiDeathState());
        stateMachine.RegisterState(new AiIdleState());
        stateMachine.RegisterState(new AiAttackState());
        stateMachine.RegisterState(new AIPlayerDeathState());
        stateMachine.RegisterState(new AiFleeState());

        stateMachine.ChangeState(initialState);
    }

    public void setRigidbodyState(bool state)
    {

        Rigidbody[] rigidbodies = GetComponentsInChildren<Rigidbody>();

        foreach (Rigidbody rigidbody in rigidbodies)
        {
            rigidbody.isKinematic = state;
        }

        GetComponent<Rigidbody>().isKinematic = true;

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

    public void RollStart()
    {
        animator.applyRootMotion = true;
        agent.updatePosition = true;
        float rand_x = Random.Range(-180, 180);
        float rand_z = Random.Range(-180, 180);
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(transform.rotation.x + rand_x, 0, transform.rotation.z + rand_z));
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 1f);

     }

    public void RollEnd()
    {
        animator.applyRootMotion = false;
        agent.updatePosition = true;
        agent.nextPosition = transform.position;
    }

    public void Attack()
    {
        weapon.StartFiring();
    }

    public void SetRigBuilder()
    {
        foreach (MultiAimConstraint component in GetComponentsInChildren<MultiAimConstraint>())
        {
            var data = component.data.sourceObjects;
            data.SetTransform(0, target);
            component.data.sourceObjects = data;
        }
        rigs.Build();
        rigs.enabled = false;
    }

    public void Aim()
    {
        rigs.enabled = true;
    }

    public void StopAim()
    {
        rigs.enabled = false;
    }


    public void StopAttack()
    {
        weapon.StopFiring();
    }

    public void FaceTarget()
    {
        if (!isRolling)
        {
            Vector3 direction = (target.position - transform.position).normalized;
            Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);
        }
    }

    public void Die()
    {
        stateMachine.ChangeState(AiStateId.Death);
        animator.enabled = false;
        agent.enabled = false;
        onDeath?.Invoke(revived);
    }

    public void ThrowGrenade()
    {


        Vector3 distance = transform.position - target.position;
        float force = distance.magnitude * throwForce;
        force = Mathf.Min(force, maxThrowForce);

        
        grenades -= 1;
        GameObject grenade = Instantiate(grenadeObject, grenadeThrowPoint.position, Quaternion.identity);
        grenade.transform.rotation = new Quaternion(-0.707106829f, 0, 0, 0.707106829f);
        Rigidbody rb = grenade.GetComponent<Rigidbody>();
        rb.AddForce(transform.forward * force);

    }

    public void TakeDamage(float amount)
    {
        health -= amount;
        if (health <= 0f)
        {
            Die();
        }
    }

    public void PlayerShootingAlert()
    {
        if (stateMachine.currentState == AiStateId.Idle)
        {
            stateMachine.ChangeState(AiStateId.Chase);
        }
    }

    public IEnumerator moveToPoint(Vector3 waypoint)
    {
        agent.enabled = true;
        agent.isStopped = false;
        WaitForSeconds Wait = new WaitForSeconds(3);
        while (true)
        {
            agent.SetDestination(
                waypoint
            );

            yield return null;
            yield return new WaitUntil(() => agent.remainingDistance <= agent.stoppingDistance);
            yield return Wait;
        }
    }
}
