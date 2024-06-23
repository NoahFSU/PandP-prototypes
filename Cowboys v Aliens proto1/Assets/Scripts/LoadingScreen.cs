using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadingScreen : MonoBehaviour
{
    private const string TutorialCompletedKey = "TutorialCompleted";

    public GameObject loadingScreen;
    public Slider slider;

    public void LoadScene(int sceneId)
    {
        StartCoroutine(LoadSceneAsync(sceneId));
    }

    IEnumerator LoadSceneAsync(int sceneId)
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneId);

        DontDestroyOnLoad(loadingScreen);
        DontDestroyOnLoad(this.gameObject);
        loadingScreen.SetActive(true);

        while (!operation.isDone)
        {
            float progressValue = Mathf.Clamp01(operation.progress / 0.9f);

            slider.value = progressValue;

            yield return null;
        }
        yield return new WaitForSeconds(3);
        loadingScreen.SetActive(false);
        Destroy(loadingScreen);
        Destroy(this.gameObject);
    }

    public void PlayGame()
    {
        if (PlayerPrefs.GetInt(TutorialCompletedKey, 0) == 1)
        {
            SceneManager.LoadScene("Hub Level 2 Unlock");
        }
        else
        {
            SceneManager.LoadScene("Tutorial");
        }
    }

    public void CompleteTutorial()
    {
        PlayerPrefs.SetInt(TutorialCompletedKey, 1);
    }
}
