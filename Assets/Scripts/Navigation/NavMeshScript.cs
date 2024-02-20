using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class NavMeshScript : MonoBehaviour
{
    // Fields for navigation target
    [SerializeField] private Transform goal;
    [SerializeField] private NavMeshAgent agent;
    [SerializeField] private float updateDelay;
    private float timer = 0.0f;

    // Start is called before the first frame update
    void Start()
    {
        //Starting values
        agent = GetComponent<NavMeshAgent>();
        agent.destination = goal.position;
    }

    // Update is called once per frame
    void Update()
    {
        //timer increments each frame
        timer += Time.deltaTime;

        //Checks if time for update
        if (timer >= updateDelay)
        {
            //resets timer and updates pathfinding
            timer = 0.0f;
            agent.destination = goal.position;
        }
    }
}
