using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveHand : MonoBehaviour
{
    private float timer = 0;
    public string movingState = "Stall";
    [SerializeField] float maxMovement = 30;
    [SerializeField] float speed = 0.5f;

    void Start()
    {

    }


    void Update()
    {
        //Move pin along given vector until timer has reached max
        if (movingState == "Up")
        {
            timer += 1;
            transform.position += transform.forward * speed;
        }
        if (movingState == "Down")
        {
            timer -= 1;
            transform.position -= transform.forward * speed;
        }

        //Up -> Down
        if (timer == maxMovement)
        {
            movingState = "Down";
        }
        //Down -> Stall
        if (timer == 0)
        {
           movingState = "Stall";
        }
    }
}

