
using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Animations.Rigging;
using UnityEngine.UI;

public class BossController : MonoBehaviour
{
  
    public Transform target;
    public Transform targetOrientation;
    public NavMeshAgent agent;
    public Animator animator;
    public float health = 50f;
    public bool isDead = false;
    public GameObject forceField;
    public BossStateMachine stateMachine;
    public BossStateID initialState = BossStateID.Attack;

    public const float FORCE_FIELD_REGEN = 5f;
    bool forceFieldActive = true;
    float forceFieldTimer = FORCE_FIELD_REGEN;
    public float throwForce = 50f;
    public float maxThrowForce = 1000f;

    public GameObject ovenObject;
    public Transform ovenThrowPoint;
    public Canvas bossHealthPrefab;
    private Slider bossHealthBar;
    private float maxHealth;


    // Start is called before the first frame update
    void Start()
    {
        maxHealth = health;
        target = PlayerManager.Instance.player.transform;
        targetOrientation = PlayerManager.Instance.orientation;


        agent = GetComponent<NavMeshAgent>();

        animator = GetComponent<Animator>();
        animator.applyRootMotion = false;
        stateMachine = new BossStateMachine(this);
        stateMachine.RegisterState(new BossAttackState());
        stateMachine.RegisterState(new BossPlayerDeathState());



        bossHealthBar = Instantiate(bossHealthPrefab, null).transform.GetChild(0).GetComponent<Slider>();

        stateMachine.ChangeState(initialState);
    }

    // Update is called once per frame
    void Update()
    {
        if (!forceFieldActive)
        {
            forceFieldTimer -= Time.deltaTime;
            if (forceFieldTimer < 0)
            {
                forceFieldTimer = FORCE_FIELD_REGEN;
                forceField.SetActive(true);
                forceFieldActive = true;
            }
        }
        stateMachine.Update();

        if (!PlayerManager.Instance.isAlive)
        {
            stateMachine.ChangeState(BossStateID.PlayerDeath);
        }
        if (isDead)
        {
            return;
        }



    }


    public void FaceTarget()
    {
        Vector3 direction = (target.position - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 3f);
    }


    public void ThrowOven()
    {

        forceField.SetActive(false);
        forceFieldActive = false;

        Vector3 displacement = new Vector3(
              target.position.x,
              ovenThrowPoint.position.y,
              target.position.z
            ) - ovenThrowPoint.position;
        float deltaY = target.position.y - ovenThrowPoint.position.y;
        float deltaXZ = displacement.magnitude;

        float gravity = Mathf.Abs(Physics.gravity.y);
        float throwStrength = Mathf.Clamp(
                Mathf.Sqrt(
                        gravity * (deltaY + Mathf.Sqrt(Mathf.Pow(deltaY,2)
                        + Mathf.Pow(deltaXZ,2)))),
                        0.01f,
                        maxThrowForce 
                    );
        float angle = Mathf.PI / 2f - (0.5f * (Mathf.PI / 2f - (deltaY / deltaXZ)));

        Vector3 initialVelocity = Mathf.Cos(angle) * throwStrength * displacement.normalized
            + Mathf.Sin(angle) * throwStrength * Vector3.up;

        GameObject oven = Instantiate(ovenObject, ovenThrowPoint.position, ovenThrowPoint.rotation);
        Rigidbody rb = oven.GetComponent<Rigidbody>();


        rb.velocity = initialVelocity;

    }

    public void TakeDamage(float amount)
    {
        health -= amount;
        bossHealthBar.value = health / maxHealth;
        if (health <= 0f)
        {
            Die();
        }
    }

    public void Die()
    {
        stateMachine.ChangeState(BossStateID.Death);
        animator.enabled = false;
        Destroy(agent);
    }

}
