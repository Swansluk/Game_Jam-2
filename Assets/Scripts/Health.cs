using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Health : MonoBehaviour
{
    [SerializeField] private int hp;
    [SerializeField] private int maxHealth;

    // Start is called before the first frame update
    void Start()
    {
        hp = maxHealth;
    }

    // Method to take damage
    public void takeDamage(int damage)
    {
        hp -= damage;
    }

    // Update is called at set rate
    void FixedUpdate()
    {
        if (hp <= 0)
        {
            //Destroy(gameObject);
            //needs work...
            SceneManager.LoadScene("MainMenu");
            //SceneManager.UnloadSceneAsync("SimpleScene");
        }
    }
}
