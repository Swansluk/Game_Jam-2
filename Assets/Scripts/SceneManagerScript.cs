using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class SceneManagerScript : MonoBehaviour
{
    [SerializeField] public string newGameScene;
   public void LoadSceneOnTrigger(Collider other) 
   {
    if(other.CompareTag("Player")) {
        
        SceneManager.LoadScene(newGameScene);
    }
    
   }
}
