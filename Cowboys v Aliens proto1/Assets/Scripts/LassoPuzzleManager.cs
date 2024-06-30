using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.ProBuilder.MeshOperations;

public class LassoPuzzleManager : MonoBehaviour
{
    public static LassoPuzzleManager Instance;

    [SerializeField] public List<LassoPuzzleTarget> targets = new List<LassoPuzzleTarget>();
    private int currTargetIndex = 0;
    [SerializeField] GameObject LoadingDoor;
    [SerializeField] AudioClip doorAppearClip;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    public void TargetLassoed(LassoPuzzleTarget target)
    {
        if (target.targetOrder == currTargetIndex)
        {
            target.TurnGreen();
            currTargetIndex++;
            if (currTargetIndex >= targets.Count)
            {
                LoadNextLevel();
            }
        }
        else
        {
            FlashAllRed();
            ResetPuzzle();
        }
    }

    private void LoadNextLevel()
    {
        LoadingDoor.SetActive(true);
        AudioSource doorAudSource = LoadingDoor.GetComponent<AudioSource>();
        if (doorAudSource != null)
        {
            doorAudSource.PlayOneShot(doorAppearClip);
        }
    }

    private void ResetPuzzle()
    {
        currTargetIndex = 0;
        foreach (LassoPuzzleTarget target in targets)
        {
            target.ResetLassoed();
        }
    }

    private void FlashAllRed()
    {
        foreach (LassoPuzzleTarget target in targets)
        {
            target.FlashRed();

        }
    }

  

}
