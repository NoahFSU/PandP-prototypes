using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CompleteTutorialTrigger : MonoBehaviour
{
    private LoadingScreen loadingScreen;

    void Start()
    {
        loadingScreen = FindObjectOfType<LoadingScreen>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            loadingScreen.CompleteTutorial();
            SceneManager.LoadScene("Hub Level 2 Unlock");
        }
    }
}
