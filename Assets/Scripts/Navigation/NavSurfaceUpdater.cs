using System.Collections;
using System.Collections.Generic;
using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.AI;

public class NavSurfaceUpdater : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] public NavMeshSurface surface;
    [SerializeField] private float updateDelay;
    private float timer = 0.0f;

    void Start()
    {
        surface = GetComponent<NavMeshSurface>();
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;
        if (timer >= updateDelay)
        {
            surface.BuildNavMesh();
        }
    }
}
