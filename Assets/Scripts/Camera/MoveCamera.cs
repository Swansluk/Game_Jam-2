using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/*
 * 
 * Placed on the Camera Holder
 * For moving the Camera with the Player
 * 
 */


public class MoveCamera : MonoBehaviour
{

    public Transform target;

    // Update is called once per frame
    private void Update()
    {
        transform.position = target.position;
    }
}
