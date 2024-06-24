using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CompleteTutorialTrigger : MonoBehaviour
{
    private LoadingScreen loadingScene;

    void Start()
    {
        loadingScene = FindObjectOfType<LoadingScreen>();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            loadingScene.CompleteTutorial();
      
        }
    }
}
