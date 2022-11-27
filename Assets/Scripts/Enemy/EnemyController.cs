
using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Animations.Rigging;

public class EnemyController : MonoBehaviour
{
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
    public float strafeSpeed = 4.5f;
    public int grenades = 2;
    public float throwForce = 50f;
    public float maxThrowForce = 1000f;
    public float autoAlertRadius = 2.5f;
    public GameObject grenadeObject;
    public float armingRange = 6.0f;
    public Transform grenadeThrowPoint;
    RayCastWeapon weapon;
    RigBuilder rigs;

    private Vector2 velocity = Vector2.zero;
    private Vector2 smoothDeltaPos = Vector2.zero;



    // Start is called before the first frame update
    void Start()
    {
        target = PlayerManager.Instance.player.transform;
        targetOrientation = PlayerManager.Instance.orientation;

        rigs = GetComponent<RigBuilder>();

        weapon = GetComponentInChildren<RayCastWeapon>();
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



        stateMachine.ChangeState(initialState);
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
        bool shouldMove = (agent.velocity.magnitude > 0.5f
           && agent.remainingDistance > agent.stoppingDistance);
        animator.SetBool("Moving", shouldMove);


        animator.SetFloat("speed_x", agent.velocity.x);
        animator.SetFloat("speed_y", agent.velocity.z);




    }

    private void LateUpdate()
    {
        if (weapon.isFiring)
        {
            weapon.UpdateFiring(Time.deltaTime);
        }
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
        Vector3 direction = (target.position - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);
    }

    public void Die()
    {
        stateMachine.ChangeState(AiStateId.Death);
        animator.enabled = false;
        Destroy(agent);

    }

    public float TimeToPeak(float gravityStrength, float verticalDistance)
    {
        return Mathf.Sqrt(verticalDistance / (gravityStrength * 0.5f));
    }

    public void ThrowGrenade()
    {


        Vector3 distance = transform.position - target.position;
        float force = distance.magnitude * throwForce;
        force = Mathf.Min(force, maxThrowForce);


        grenades -= 1;
        GameObject grenade = Instantiate(grenadeObject, grenadeThrowPoint.position, grenadeThrowPoint.rotation);
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
        stateMachine.ChangeState(AiStateId.Chase);
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
