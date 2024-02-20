using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * 
 * Goes on the Player RB object
 * For handling the players base movement and states
 *          Excludes functionality of wall running and sliding and grappling
 * 
 */

public class PlayerMovement : MonoBehaviour
{
    [Header("Keybinds")]
    public KeyCode jumpKey = KeyCode.Space;
    public KeyCode sprintKey = KeyCode.LeftShift;
    public KeyCode crouchKey = KeyCode.LeftControl;

    private float moveSpeed;
    [Header("Movement")]
    public float walkSpeed;
    public float sprintSpeed;
    public float slideSpeed;//----------Slope Sliding
    public float wallRunSpeed;//----------Wall Running

    private float desiredMoveSpeed; //----------Slope Sliding
    private float lastDesiredMoveSpeed;//
    public float speedIncreaseMultiplier;//
    public float slopeIncreaseMultiplier;//
    [HideInInspector]public bool sliding; //----------Slope Sliding

    [HideInInspector]public bool wallrunning;//----------Wall Running

    public float groundDrag;
    public float airDrag;

    [Header("GroundCheck")]
    public float playerHeight;
    public LayerMask whatIsGround;
    bool grounded;

    [Header("Jumping")]
    public float jumpPower;
    public float jumpCooldown;
    public float airMultiplier;
    bool readyToJump = true;

    [Header("Crouching")]
    public float crouchSpeed;
    public float crouchYScale;
    private float startYScale;

    [Header("Slope Handling")]
    public float maxSlopeAngle;
    private RaycastHit slopeHit;
    private bool exitingSlope = false;

    [Header("References")]
    public Transform orientation;
    float horizontalInput;
    float verticalInput;
    Vector3 moveDirection;
    Rigidbody rb;

    [HideInInspector] public bool freeze; //----------------------GRAPPLING
    [HideInInspector] public bool activeGrapple; //----------------------GRAPPLING

    public enum MovementState
    {
        freeze,//----------------------GRAPPLING
        walking,
        sprinting,
        sliding, //----------Slope Sliding
        wallrunning,//----------WALL RUNNING
        grappling,//----------------------GRAPPLING
        crouching,
        air
    }

    public MovementState state;

    void Start()
    {
        //get rb and freeze rotation
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;

        readyToJump = true;

        startYScale = transform.localScale.y;
    }

    // Update is called once per frame
    void Update()
    {
        handleGroundCheck();

        handleMoveInput();

        handleJump();
        handleCrouch();

        handleSpeedControl();
        handleMoveState();

        handleDrag();

        //Debug.Log("On Slope: " + onSlope());
        //Debug.Log("State: " + state);
    }

    private void FixedUpdate()
    {
        handleMovePlayer();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (enableMovementOnNextTouch)
        {
            activeGrapple = false;

            GetComponent<Grappling>().stopGrapple();
        }
    }

    private void handleMoveInput()
    {
        //get inputs from keyboard
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");
    }

    private void handleMoveState()
    {
        if (freeze)
        {
            state = MovementState.freeze;
            moveSpeed = 0;
            rb.velocity = Vector3.zero;
        }
        else if (wallrunning)//----------Wall Running
        {
            state = MovementState.wallrunning;
            desiredMoveSpeed = wallRunSpeed;
        }
        else if (sliding)//----------Slope Sliding //----------Wall Running(Add else)
        {
            state = MovementState.sliding;

            if (onSlope() && rb.velocity.y < 0.1)
            {
                desiredMoveSpeed = slideSpeed;
            }
            else
            {
                desiredMoveSpeed = sprintSpeed;
            }
        }//------------(add else)------------
        else if (Input.GetKey(crouchKey))
        {
            state = MovementState.crouching;
            desiredMoveSpeed = crouchSpeed;
        }
        else if (grounded && Input.GetKey(sprintKey))//sprinting state
        {
            state = MovementState.sprinting;
            desiredMoveSpeed = sprintSpeed;
        }
        else if (grounded)//walking state
        {
            state = MovementState.walking;
            desiredMoveSpeed = walkSpeed;
        }
        else//air
        {
            state = MovementState.air;
        }

        if (Mathf.Abs(desiredMoveSpeed - lastDesiredMoveSpeed) > 4f && moveSpeed != 0)
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

    private void handleGroundCheck()
    {
        //raycast down to check for ground
        grounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + 0.2f, whatIsGround);
    }

    private IEnumerator SmoothlyLerpMoveSpeed()//----------Slope Sliding
    {

        // smoothly lerp movementSpeed to desired value
        float time = 0;
        float difference = Mathf.Abs(desiredMoveSpeed - moveSpeed);
        float startValue = moveSpeed;

        while (time < difference)
        {
            moveSpeed = Mathf.Lerp(startValue, desiredMoveSpeed, time / difference);

            if (onSlope())
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
    }//-----------------------------------------------------------------

    private void handleMovePlayer()
    {
        if (activeGrapple) return; //----------------------GRAPPLING

        //calculate movement speed
        moveDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;

        //on slope
        if (onSlope() && !exitingSlope)
        {
            rb.AddForce(getSlopeMoveDirection(moveDirection) * moveSpeed * 20f, ForceMode.Force);

            if (rb.velocity.y != 0)
            {
                rb.AddForce(Vector3.down * 80f, ForceMode.Force);
            }
        }

        //add force
        else if (grounded) {//on ground
            rb.AddForce(moveDirection.normalized * moveSpeed * 10f, ForceMode.Force);
        }
        else if(!grounded)//in air
        {
            rb.AddForce(moveDirection.normalized * moveSpeed * 10f * airMultiplier, ForceMode.Force);
        }

        //turn off gravity while standing on a slope
        if(!wallrunning) rb.useGravity = !onSlope(); //WALL RUNNING###################### implemented with gravity
    }

    #region Drag and Speed Control
    private void handleDrag()
    {
        if (grounded && !activeGrapple)//on ground //----------------------GRAPPLING
        {
            rb.drag = groundDrag;
        }
        else //in air
        {
            rb.drag = airDrag;
        }
    }

    private void handleSpeedControl()
    {
        if (activeGrapple) return; //----------------------GRAPPLING

        if (onSlope() && !exitingSlope)
        {
            if (rb.velocity.magnitude > moveSpeed)
            {
                rb.velocity = rb.velocity.normalized * moveSpeed;
            }
        }
        else
        {
            Vector3 flatVel = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

            //limit velocity if needed
            if (flatVel.magnitude > moveSpeed)
            {
                //resize the velocity vector and set it to the new velocity maintaining y velocity
                Vector3 limitedVel = flatVel.normalized * moveSpeed;
                rb.velocity = new Vector3(limitedVel.x, rb.velocity.y, limitedVel.z);
            }
        }
    }
    #endregion

    #region Jump
    private void handleJump()
    {
        if (Input.GetKey(jumpKey) && readyToJump && grounded)
        {
            readyToJump = false;
            Jump();
            Invoke(nameof(resetJump), jumpCooldown);
        }
    }

    private void Jump()
    {
        //Debug.Log("Trying to Jump");
        exitingSlope = true;
        //reset so we always jumnp the exact same hieght
        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
        //apply jump force
        rb.AddForce(transform.up * jumpPower, ForceMode.Impulse);
    }

    //----------------------GRAPPLING
    private bool enableMovementOnNextTouch;


    public void JumpToPosition(Vector3 targetPos, float trajectoryHieght)
    {
        activeGrapple = true;

        velToSet = calcJumpVelocity(transform.position, targetPos, trajectoryHieght);

        Invoke(nameof(setVelocity), 0.01f);
    }

    private Vector3 velToSet;
    private void setVelocity()
    {
        enableMovementOnNextTouch = true;
        rb.velocity = velToSet;
    }

    //----------------------GRAPPLING

    private void resetJump()
    {
        readyToJump = true;
        exitingSlope = false;
    }
    #endregion

    #region Crouching

    private void handleCrouch()
    {
        if (Input.GetKeyDown(crouchKey))
        {
            transform.localScale = new Vector3(transform.localScale.x, crouchYScale, transform.localScale.z);
            rb.AddForce(Vector3.down * 5f, ForceMode.Impulse);
        }
        if (Input.GetKeyUp(crouchKey))
        {
            transform.localScale = new Vector3(transform.localScale.x, startYScale, transform.localScale.z);
        }
    }

    #endregion

    #region Slope Movement (Public)

    public Vector3 getSlopeMoveDirection(Vector3 direction)
    {
        return Vector3.ProjectOnPlane(direction, slopeHit.normal).normalized;
    }

    public bool onSlope()
    {
        if (Physics.Raycast(transform.position, Vector3.down, out slopeHit, playerHeight * 0.5f + 1f))
        {
            float angle = Vector3.Angle(Vector3.up, slopeHit.normal);
            return angle < maxSlopeAngle && angle != 0;
        }

        return false;
    }

    #endregion

    //----------------------GRAPPLING

    public Vector3 calcJumpVelocity(Vector3 start, Vector3 end, float trajectoryHeight)
    {
        float gravity = Physics.gravity.y;
        float displacementY = end.y - start.y;
        Vector3 displacementXZ = new Vector3(end.x - start.x, 0f, end.z - start.z);

        Vector3 velY = Vector3.up * Mathf.Sqrt(-2 * gravity * trajectoryHeight);
        Vector3 velXZ = displacementXZ / (Mathf.Sqrt(-2 * trajectoryHeight / gravity)
            + Mathf.Sqrt(2 * (displacementY - trajectoryHeight) / gravity));

        return velXZ + velY;
    }

    //----------------------GRAPPLING
}
