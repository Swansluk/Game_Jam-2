using System.Collections;
using System.Collections.Generic;
using UnityEditor.Presets;
using UnityEngine;

public class EnemyScript : MonoBehaviour
{
    [SerializeField] private int damage;
    [SerializeField] private float dmgCooldown;
    private Health enemyHp;
    private float timer = 0.0f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    //activates when collision is detected
    private void OnCollisionEnter(Collision collision)
    {
        enemyHp = collision.gameObject.GetComponent<Health>();
        if (enemyHp != null)
        {
            enemyHp.takeDamage(damage);
        }
    }

    // activates 
    private void OnCollisionStay(Collision collision)
    {
        //checks if enemy has hp
        if (enemyHp != null)
        {
            //increments timer
            timer += Time.deltaTime;
            //if able to do damage it does it, resets timer
            if (timer >= dmgCooldown)
            {
                enemyHp.takeDamage(damage);
                timer = 0.0f;
            }
        }
    }

    //resets target Health component once collision ends
    private void OnCollisionExit(Collision collision)
    {
        enemyHp = null;
        timer = 0.0f;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
