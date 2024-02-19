using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grappling : MonoBehaviour
{
    [Header("Grappling")]
    private PlayerMovement pm;
    public Transform cam;
    public Transform gunTip;
    public LayerMask whatIsGrappleable;
    public LineRenderer lr;

    [Header("Grappling")]
    public float maxGrappleDistance;
    public float grappleDelayTime;
    public float overshootYAxis;

    private Vector3 grapplePoint;

    [Header("Cooldown")]
    public float grapplingCD;
    private float grapplingDCTimer;

    [Header("Input")]
    public KeyCode grapplingKey = KeyCode.Mouse1;

    [Header("References")]
    public PlayerCam playerCam;

    private bool grappling;

    void Start()
    {
        pm = GetComponent<PlayerMovement>();
    }

    void Update()
    {
        if (Input.GetKeyDown(grapplingKey))
        {
            startGrapple();
        }

        if(grapplingDCTimer > 0) grapplingDCTimer -= Time.deltaTime;
    }

    private void LateUpdate()
    {
        if (grappling) lr.SetPosition(0, gunTip.transform.position);
    }

    private void startGrapple()
    {
        if (grapplingDCTimer > 0) return;

        //playerCam.changeFOV(95f);

        grappling = true;

        pm.freeze = true;

        RaycastHit hit;
        if(Physics.Raycast(cam.position, cam.forward, out hit, maxGrappleDistance, whatIsGrappleable))
        {
            grapplePoint = hit.point;

            Invoke(nameof(handleGrappling), grappleDelayTime);
        }
        else
        {
            grapplePoint = cam.position + cam.forward * maxGrappleDistance;

            Invoke(nameof(stopGrapple), grappleDelayTime);
        }

        lr.enabled = true;
        lr.SetPosition(1,grapplePoint);
    }

    private void handleGrappling()
    {
        pm.freeze = false;

        Vector3 lowestPoint = new Vector3(transform.position.x, transform.position.y - 1f, transform.position.z);

        float grapplePointRelativeYPos = grapplePoint.y - lowestPoint.y;
        float highestPointOnArc = grapplePointRelativeYPos + overshootYAxis;

        if (grapplePointRelativeYPos < 0) highestPointOnArc = overshootYAxis;

        pm.JumpToPosition(grapplePoint, highestPointOnArc);

        Invoke(nameof(stopGrapple), 1f);
    }

    public void stopGrapple()
    {
        //playerCam.changeFOV(90f);

        pm.freeze = false;

        grappling = false;

        grapplingDCTimer = grapplingCD;

        lr.enabled = false;
    }
}
