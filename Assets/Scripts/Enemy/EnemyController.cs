
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Animations.Rigging;

public class EnemyController : MonoBehaviour
{
    public float visionRadius = 10f;
    public float shootingRange = 6f;
    public Transform target;
    public NavMeshAgent agent;
    public Animator animator;
    public float health = 50f;
    public bool isDead = false;
    public AiStateMachine stateMachine;
    public AiStateId initialState = AiStateId.Idle;
    public float armingRange = 6.0f;
    RayCastWeapon weapon;
    RigBuilder rigs;
    // Start is called before the first frame update
    void Start()
    {
        target = PlayerManager.Instance.player.transform;
        rigs = GetComponent<RigBuilder>();

        weapon = GetComponentInChildren<RayCastWeapon>();
        SetRigBuilder();

        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        
        setRigidbodyState(true);
        setColliderState(false);
        stateMachine = new AiStateMachine(this);
        stateMachine.RegisterState(new AiChaseState());
        stateMachine.RegisterState(new AiDeathState());
        stateMachine.RegisterState(new AiIdleState());
        stateMachine.RegisterState(new AiAttackState());
        

        stateMachine.ChangeState(initialState);
    }

    // Update is called once per frame
    void Update()
    {
        stateMachine.Update();
        
        if (isDead)
        {
            return;
        }
       
        if (agent.hasPath)
        {
            animator.SetFloat("Speed", agent.velocity.magnitude);
        }
        else
        {
            animator.SetFloat("Speed", 0);
        }
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

        GetComponent<Rigidbody>().isKinematic = !state;

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
        Destroy(agent);

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
