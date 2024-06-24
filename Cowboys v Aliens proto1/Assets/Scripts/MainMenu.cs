using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    void Start()
    {
#if UNITY_WEBGL
        transform.Find("MainMenu").Find("Quit").gameObject.SetActive(false);
#endif
    }
    public void ExitButton()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        SceneManager.LoadScene("MainMenu");
#elif UNITY_WEBGL

#else
        Application.Quit();
#endif
    }

    public void StartGame()
    {
        SceneManager.LoadScene("C_V_A_V1.0.0");
    }
}
