using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class buttonFunctions : MonoBehaviour
{
    public void Resume()
    {
        gameManager.Instance.stateUnPaused();
    }

    public void RespawnPlayer()
    {
        gameManager.Instance.playerscript.SpawnPlayer();
        gameManager.Instance.stateUnPaused();
    }
    public void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        gameManager.Instance.stateUnPaused();
    }

    public void Quit()
    {
        SceneManager.LoadScene("MainMenu");
    }

    public void LoadLevel(int level)
    {
        SceneManager.LoadScene(level);
        gameManager.Instance.stateUnPaused();
    }

}
