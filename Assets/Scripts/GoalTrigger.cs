using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GoalTrigger : MonoBehaviour
{
    [SerializeField] private GameObject player;
    [SerializeField] private Health playerHealth;
    [SerializeField] private string nextLevel;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        player = other.gameObject;
        playerHealth = player.GetComponentInParent<Health>();
        if (playerHealth != null)
        {
            SceneManager.LoadScene(nextLevel);
        }
    }

}
