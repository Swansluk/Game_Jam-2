using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallRunning : MonoBehaviour
{
    [Header("WallRunning")]
    public LayerMask whatIsWall;
    public LayerMask whatIsGround;
    public float wallRunForce;
    public float wallClimbSpeed;
    public float maxWallRunTime;
    private float wallRunTimer;

    [Header("Wall Jumping")]
    public float wallJumpForce;
    public float walllJumpSideForce;

    [Header("Input")]
    public KeyCode upwardsRunKey = KeyCode.LeftShift;
    public KeyCode downwardsRunKey = KeyCode.LeftControl;
    public KeyCode wallJumpKey = KeyCode.Space;
    private bool upwardsRunning;
    private bool downwardsRunning;
    private float hInput;
    private float vInput;

    [Header("Detection")]
    public float wallCheckDistance;
    public float minJumpHeight;
    private RaycastHit leftWallHit;
    private RaycastHit rightWallHit;
    private bool wallLeft;
    private bool wallRight;


    [Header("Exiting")]
    private bool exitingWall;
    public float exitWallTime;
    private float exitWallTimer;

    [Header("Gravity")]
    public bool useGravity;
    public float gravityCounterForce;

    [Header("References")]
    public Transform orientation;
    public PlayerCam cam;
    private PlayerMovement pm;
    private Rigidbody rb;

    // Start is called before the first frame update
    void Start()
    {
        pm = GetComponent<PlayerMovement>();
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        checkForWall();
        handleStates();
    }

    private void FixedUpdate()
    {
        if (pm.wallrunning)
        {
            handleWallRunning();
        }
    }

    private void checkForWall()
    {
        wallRight = Physics.Raycast(transform.position, orientation.right, out rightWallHit, wallCheckDistance, whatIsWall);
        wallLeft = Physics.Raycast(transform.position, -orientation.right, out leftWallHit, wallCheckDistance, whatIsWall);
    }

    private bool aboveGround()
    {
        return !Physics.Raycast(transform.position, Vector3.down, minJumpHeight, whatIsGround);
    }

    private void handleStates()
    {
        //get inputs
        hInput = Input.GetAxisRaw("Horizontal");
        vInput = Input.GetAxisRaw("Vertical");

        upwardsRunning = Input.GetKey(upwardsRunKey);
        downwardsRunning = Input.GetKey(downwardsRunKey);

        //wallRunning
        if ((wallLeft || wallRight) && vInput > 0 && aboveGround() && !exitingWall)
        {
            //start wall run
            if (!pm.wallrunning)
            {
                startWallRun();
            }

            if (wallRunTimer > 0)
            {
                wallRunTimer -= Time.deltaTime;
            }
            if (wallRunTimer <= 0 && pm.wallrunning)
            {
                exitingWall = true;
                exitWallTimer = exitWallTime;
            }

            //walljump
            if (Input.GetKeyDown(wallJumpKey))
            {
                wallJump();
            }
        }
        else if (exitingWall) //exiting the wall
        {
            if (pm.wallrunning)
            {
                stopWallRun();
            }
            if (exitWallTimer > 0)
            {
                exitWallTimer -= Time.deltaTime;
            }
            if (exitWallTimer <= 0)
            {
                exitingWall = false;
            }
        }
        else
        {
            if (pm.wallrunning)
            {
                stopWallRun();
            }
        }
    }

    private void startWallRun()
    {
        pm.wallrunning = true;

        wallRunTimer = maxWallRunTime;

        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);//moved from handleWallRunning to account for using gravity

        cam.changeFOV(90);
        if (wallLeft) cam.changeTilt(-5f);
        if (wallRight) cam.changeTilt(5f);
    }

    private void handleWallRunning()
    {
        Debug.Log("WALL RUNNING");

        //no y movement
        rb.useGravity = useGravity;

        //calc wall Forward
        Vector3 wallNormal = wallRight ? rightWallHit.normal : leftWallHit.normal;
        Vector3 wallForward = Vector3.Cross(wallNormal, transform.up);

        //handle oppisite direction
        if ((orientation.forward - wallForward).magnitude > (orientation.forward - -wallForward).magnitude)
        {
            wallForward = -wallForward;
        }

        //add force
        rb.AddForce(wallForward * wallRunForce, ForceMode.Force);

        //climbing up and down
        if (upwardsRunning)
        {
            rb.velocity = new Vector3(rb.velocity.x, wallClimbSpeed, rb.velocity.z);
        }
        else if (downwardsRunning)
        {
            rb.velocity = new Vector3(rb.velocity.x, -wallClimbSpeed, rb.velocity.z);
        }
        //push to wall force
        if (!(wallLeft && hInput > 0) && !(wallRight && hInput < 0)) {
            rb.AddForce(-wallNormal * 100, ForceMode.Force);
        }

        //counteract gravity
        if (useGravity)
        {
            rb.AddForce(transform.up * gravityCounterForce, ForceMode.Force);
        }
    }

    private void stopWallRun()
    {
        pm.wallrunning = false;

        cam.changeFOV(80f);
        cam.changeTilt(0f);
    }

    #region Wall Jumping

    private void wallJump()
    {
        Debug.Log("Trying to WALL JUMP");

        //exiting the wall;
        exitingWall = true;
        exitWallTimer = exitWallTime;

        //determine the force
        Vector3 wallNorm = wallRight ? rightWallHit.normal : leftWallHit.normal;
        Vector3 wallJumpingVectorForce = transform.up * wallJumpForce + wallNorm * walllJumpSideForce;

        //add force
        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
        rb.AddForce(wallJumpingVectorForce, ForceMode.Impulse);
    }

    #endregion
}
