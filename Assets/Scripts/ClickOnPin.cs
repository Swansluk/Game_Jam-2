using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.UI.Image;

public class ClickOnPin : MonoBehaviour
{
    [SerializeField] float maxDistance = 20f; // Maximum distance to detect pins
    [SerializeField] LayerMask pinLayerMask; // Layer mask for pins
    [SerializeField] GameObject hand;

    void Start()
    {
    }

    void Update()
    {
        //Left mouse print
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            //Move hand up and down no matter if clicked pin or not.
            MoveHand moveHand = hand.GetComponent<MoveHand>();
            moveHand.movingState = "Up";

            //Send out the raycast
            if (Physics.Raycast(ray, out hit, maxDistance, pinLayerMask))
            {

                //Check if obect is a pin
                if (hit.collider.CompareTag("Pin"))
                {

                    //Check if player is close enough to pin
                    if (Vector3.Distance(transform.position, hit.transform.position) <= maxDistance)
                    {
                        //Grab the movePin script from the pin and set moving to true
                        MovePin movePin = hit.collider.GetComponent<MovePin>();
                        movePin.moving = true;
                    }
                }
            }
        }
    }
}
