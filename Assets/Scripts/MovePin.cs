using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovePin : MonoBehaviour
{
    private float timer = 0;
    public bool moving = false;
    private bool doneMoving = false;
    [SerializeField] Vector3 moveDirection;
    [SerializeField] float maxMovement = 50;

    void Start()
    {
        
    }


    void Update()
    {
        //Move pin along given vector until timer has reached max
        if (moving && !doneMoving)
        {
            timer += 1;
            transform.position += moveDirection;
        }
        if (timer == maxMovement)
        {
            moving = false;
            doneMoving = true;
        }
    }
}
