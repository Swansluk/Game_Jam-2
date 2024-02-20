using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sliding : MonoBehaviour
{
    [Header("Keybinds")]
    public KeyCode slideKey = KeyCode.LeftControl;

    [Header("References")]
    public Transform orientaiton;
    public Transform playerObj;
    private Rigidbody rb;
    private PlayerMovement pm;

    [Header("Sliding")]
    public float maxSlideTime;
    public float slideForce;
    private float slideTimer;

    public float slideYScale;
    private float startYScale;

    private float horizontalInput;
    private float verticalInput;

    private Vector3 inputDirection;

    //private bool sliding (deleted for momentum)

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        pm = GetComponent<PlayerMovement>();

        startYScale = playerObj.localScale.y;
    }

    private void Update()
    {
        handleInput();
    }

    private void FixedUpdate()
    {
        if (pm.sliding)
        {
            handleSlidingMovement();
        }
    }

    private void handleInput()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal"); //ad
        verticalInput = Input.GetAxisRaw("Vertical"); //ws

        if (Input.GetKeyDown(slideKey) && (horizontalInput != 0 || verticalInput != 0))
        {
            StartSlide();
        }

        if (Input.GetKeyUp(slideKey) && pm.sliding) //-------------momentum
        {
            StopSlide();
        }
    }

    private void StartSlide()
    {
        pm.sliding = true; //-------------momentum

        playerObj.localScale = new Vector3(playerObj.localScale.x, slideYScale, playerObj.localScale.z);
            rb.AddForce(Vector3.down * 5f, ForceMode.Impulse);

            inputDirection = orientaiton.forward * verticalInput + orientaiton.right * horizontalInput;

            slideTimer = maxSlideTime;
    }

    private void handleSlidingMovement()
    {
        //NORMAL SLIDING
        //Originally Implemented Without if statement (only the main body)
        if (!pm.onSlope() || rb.velocity.y > -0.1f)
        {
            rb.AddForce(inputDirection.normalized * slideForce, ForceMode.Force);

            slideTimer -= Time.deltaTime;
        }
        else//sliding on slope
        {
            rb.AddForce(pm.getSlopeMoveDirection(inputDirection) * slideForce, ForceMode.Force);
        }

        if (slideTimer <= 0)
        {
            StopSlide();
        }
    }

    private void StopSlide()
    {
        pm.sliding = false; //-------------momentum

        playerObj.localScale = new Vector3(playerObj.localScale.x, startYScale, playerObj.localScale.z);
    }


}
