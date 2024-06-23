using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;



public class DoorTransition : MonoBehaviour
{

    [SerializeField] GameObject loadingScript;
    [SerializeField] int level;
    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.GetComponent<playerController>() != null)
            LoadLevel();
    }
    public void LoadLevel()
    {
        loadingScript.GetComponent<LoadingScreen>().LoadScene(level);
    }
}
