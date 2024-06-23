using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void ExitButton()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        SceneManager.LoadScene("MainMenu");
#else
        Application.Quit();
#endif
       // Debug.Log("Game Closed");
    }

    public void StartGame()
    {
        SceneManager.LoadScene("C_V_A_V1.0.0");
    }
}
