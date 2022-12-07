using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovementAdvanced : MonoBehaviour
{
    [Header("Movement")]
    private float moveSpeed;
    public float walkSpeed;
    public float sprintSpeed;
    public float slideSpeed;

    private float desiredMoveSpeed;
    private float lastDesiredMoveSpeed;

    public float speedIncreaseMultiplier;
    public float slopeIncreaseMultiplier;

    public float groundDrag;
    public float footstepThreshold;
    [Range(1.5f, 3)]
    public float timeBetweenFootsteps = 2.4f;

    [Header("Jumping")]
    public float jumpForce;
    public float slideJumpForce;
    public float jumpCooldown;
    public float airMultiplier;
    bool readyToJump;

    [Header("Crouching")]
    public float crouchSpeed;
    public float crouchYScale;
    private float startYScale;

    [Header("Recoil")]
    [HideInInspector] public bool recoilFlag;
    public float recoilMaxSpeed = 15;
    private float recoilPlaceHolder;

    [Header("Keybinds")]
    public KeyCode jumpKey = KeyCode.Space;
    public KeyCode sprintKey = KeyCode.LeftShift;
    public KeyCode crouchKey = KeyCode.LeftControl;

    [Header("Ground Check")]
    public float playerHeight;
    public LayerMask whatIsGround;
    bool grounded;

    [Header("Gunbob")]
    public WeaponController weaponController;
    public grappleBob grapple;

    [Header("Slope Handling")]
    public float maxSlopeAngle;
    private RaycastHit slopeHit;
    private bool exitingSlope;

    public bool inputEnabled = true;
    public Transform orientation;

    private bool sprinting = false;

    float horizontalInput;
    float verticalInput;

    Vector3 moveDirection;

    Rigidbody rb;
    private float footstepTimer;

    public MovementState state;
    public enum MovementState
    {
        walking,
        sprinting,
        crouching,
        sliding,
        air,
        recoil
    }

    public bool sliding;

    private void Start()
    {
        recoilPlaceHolder = recoilMaxSpeed;
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;

        readyToJump = true;

        startYScale = transform.localScale.y;
    }

    private void Update()
    {
        if (!inputEnabled) return;

        // ground check
        grounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + 0.2f, whatIsGround);

        MyInput();
        SpeedControl();
        StateHandler();

        // handle drag
        if (grounded)
            rb.drag = groundDrag;
        else
            rb.drag = 0;

        // handle weapon bob
        weaponController.currentSpeed = AllowGunbob() ? moveSpeed : 0;
        grapple.currentSpeed = AllowGunbob() ? moveSpeed : 0;
    }

    private void FixedUpdate()
    {
        if (!inputEnabled) return;

        MovePlayer();
    }

    private void MyInput()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && !UIManager.Instance.IsPopupActive())
        {
            GameManager.Instance.TogglePause(!GameManager.Instance.IsPaused);
        }

        if (GameManager.Instance.IsPaused) return;

        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");

        // when to jump
        if(Input.GetKey(jumpKey) && readyToJump && grounded)
        {
            readyToJump = false;

            Jump();

            Invoke(nameof(ResetJump), jumpCooldown);
        }

        // start crouch
        // if (Input.GetKeyDown(crouchKey))
        // {
        //     transform.localScale = new Vector3(transform.localScale.x, crouchYScale, transform.localScale.z);
        //     rb.AddForce(Vector3.down * 5f, ForceMode.Impulse);
        // }

        // stop crouch
        // if (Input.GetKeyUp(crouchKey))
        // {
        //     transform.localScale = new Vector3(transform.localScale.x, startYScale, transform.localScale.z);
        // }
    }

    private void StateHandler()
    {
        // Mode - Sliding
        if (sliding)
        {
            state = MovementState.sliding;

            if (OnSlope() && rb.velocity.y < 0.1f)
                desiredMoveSpeed = slideSpeed;

            else
                desiredMoveSpeed = sprintSpeed;
        }

        // Mode - Crouching
        // else if (Input.GetKey(crouchKey))
        // {
        //     state = MovementState.crouching;
        //     desiredMoveSpeed = crouchSpeed;
        // }

        // Mode - Sprinting
        else if(grounded && Input.GetKey(sprintKey))
        {
            state = MovementState.sprinting;
            desiredMoveSpeed = sprintSpeed;
            if (grounded && GameManager.Instance.toggleSprint && Input.GetKeyDown(sprintKey))
                sprinting = !sprinting;
            if (!GameManager.Instance.toggleSprint)
                sprinting = false;
        }

        // Mode - Walking
        else if (grounded && !sprinting)
        {
            state = MovementState.walking;
            desiredMoveSpeed = walkSpeed;
        }

        else if (recoilFlag)
        {
            state = MovementState.recoil;
            desiredMoveSpeed = recoilMaxSpeed;
        }

        // Mode - Air
        else
        {
            state = MovementState.air;
        }

        if(recoilFlag)
        {
            return;
        }
        // check if desiredMoveSpeed has changed drastically
        else if(Mathf.Abs(desiredMoveSpeed - lastDesiredMoveSpeed) > 4f && moveSpeed != 0)
        {
            StopAllCoroutines();
            StartCoroutine(SmoothlyLerpMoveSpeed());
        }
        else
        {
            moveSpeed = desiredMoveSpeed;
        }

        lastDesiredMoveSpeed = desiredMoveSpeed;
    }

    private IEnumerator SmoothlyLerpMoveSpeed()
    {
        // smoothly lerp movementSpeed to desired value
        float time = 0;
        float difference = Mathf.Abs(desiredMoveSpeed - moveSpeed);
        float startValue = moveSpeed;

        while (time < difference)
        {
            moveSpeed = Mathf.Lerp(startValue, desiredMoveSpeed, time / difference);

            if (OnSlope())
            {
                float slopeAngle = Vector3.Angle(Vector3.up, slopeHit.normal);
                float slopeAngleIncrease = 1 + (slopeAngle / 90f);

                time += Time.deltaTime * speedIncreaseMultiplier * slopeIncreaseMultiplier * slopeAngleIncrease;
            }
            else
                time += Time.deltaTime * speedIncreaseMultiplier;

            yield return null;
        }

        moveSpeed = desiredMoveSpeed;
    }

    private void MovePlayer()
    {
        // calculate movement direction
        moveDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;

        // on slope
        if (OnSlope() && !exitingSlope)
        {
            rb.AddForce(GetSlopeMoveDirection(moveDirection) * moveSpeed * 20f, ForceMode.Force);

            if (rb.velocity.y > 0)
                rb.AddForce(Vector3.down * 80f, ForceMode.Force);
        }

        // on ground
        else if(grounded)
            rb.AddForce(moveDirection.normalized * moveSpeed * 10f, ForceMode.Force);

        // in air
        else if(!grounded)
            rb.AddForce(moveDirection.normalized * moveSpeed * 10f * airMultiplier, ForceMode.Force);

        if ((rb.velocity.magnitude > moveSpeed / footstepThreshold) && grounded && !sliding && footstepTimer <= 0)
        {
            SoundManager.Instance.PlayRandomFootstepConcrete();
            footstepTimer = timeBetweenFootsteps / moveSpeed;
        } 
        else
        {
            footstepTimer -= Time.fixedDeltaTime;
        }

        // turn gravity off while on slope
        rb.useGravity = !OnSlope();
    }

    private void SpeedControl()
    {
        // limiting speed on slope
        if (OnSlope() && !exitingSlope)
        {
            if (rb.velocity.magnitude > moveSpeed)
                rb.velocity = rb.velocity.normalized * moveSpeed;
        }
        //KEEP WORKING ON THIS: TRY TO HAVE HORIZONTAL FORCE EXCEED MAX SPRINT SPEED (Try to get the horizontal force to add up to 15 max, similar to vertical)
        else if(recoilFlag)
        {
            if(grounded)
            {
                recoilFlag = false;
                return;
            }

            if(rb.velocity.y > 15)
            {
                rb.velocity = new Vector3(rb.velocity.x, Mathf.Clamp(rb.velocity.y, 0, 15), rb.velocity.z);
            }

            Vector3 flatVel = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

            recoilPlaceHolder -= Time.deltaTime;
            recoilPlaceHolder = Mathf.Clamp(recoilPlaceHolder, 10, recoilMaxSpeed);

            // limit velocity if needed
            if (flatVel.magnitude > recoilPlaceHolder)
            {
                Vector3 limitedVel = flatVel.normalized * recoilPlaceHolder;
                rb.velocity = new Vector3(limitedVel.x, rb.velocity.y, limitedVel.z);
            }
        }

        // limiting speed on ground or in air
        else
        {
            Vector3 flatVel = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

            // limit velocity if needed
            if (flatVel.magnitude > moveSpeed)
            {
                Vector3 limitedVel = flatVel.normalized * moveSpeed;
                rb.velocity = new Vector3(limitedVel.x, rb.velocity.y, limitedVel.z);
            }
            //Debug.Log("moveSpeed = " + flatVel.magnitude);
        }

    }

    private void Jump()
    {
        exitingSlope = true;

        if (sliding)
        {
            rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
            rb.AddForce(transform.up * slideJumpForce, ForceMode.Impulse);
        }
        // reset y velocity
        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
        rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
    }
    private void ResetJump()
    {
        readyToJump = true;

        exitingSlope = false;
    }

    public bool OnSlope()
    {
        if(Physics.Raycast(transform.position, Vector3.down, out slopeHit, playerHeight * 0.5f + 0.3f))
        {
            float angle = Vector3.Angle(Vector3.up, slopeHit.normal);
            return angle < maxSlopeAngle && angle != 0;
        }

        return false;
    }

    public Vector3 GetSlopeMoveDirection(Vector3 direction)
    {
        return Vector3.ProjectOnPlane(direction, slopeHit.normal).normalized;
    }

    private bool AllowGunbob()
    {
        if (horizontalInput != 0 || verticalInput != 0)
        {
            if (grounded)
            {
                return true;
            }
        }

        return false;
    }
}