using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

/*
 * 
 * This Script belongs on the camera
 * 
 */

public class PlayerCam : MonoBehaviour
{

    [Header("Camera")]
    public float sensX;
    public float sensY;

    public Transform orientation;
    public Transform camHolder;

    float xRotation;
    float yRotation;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    // Update is called once per frame
    void Update()
    {
        //get the mouse input
        float mouseX = Input.GetAxisRaw("Mouse X") * sensX * Time.deltaTime;
        float mouseY = Input.GetAxisRaw("Mouse Y") * sensY * Time.deltaTime;

        yRotation += mouseX;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        camHolder.rotation = Quaternion.Euler(xRotation, yRotation, 0); //WALLRUNNING TILT ADAPTED CAMHODLER
        orientation.rotation = Quaternion.Euler(0,yRotation, 0);
    }

    public void changeFOV(float endValue) 
    {
        GetComponent<Camera>().DOFieldOfView(endValue, 0.25f);
    }

    public void changeTilt(float zTilt)
    {
        transform.DOLocalRotate(new Vector3(0,0,zTilt), 0.25f);
    }
}
