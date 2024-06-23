using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.ProBuilder.MeshOperations;

public class LassoPuzzleManager : MonoBehaviour
{
    public static LassoPuzzleManager Instance;

    [SerializeField] public List<LassoPuzzleTarget> targets = new List<LassoPuzzleTarget>();
    private int currTargetIndex = 0;

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
                StartCutscene();
            }
        }
        else
        {
            FlashAllRed();
            ResetPuzzle();
        }
    }

    private void StartCutscene()
    {
        Debug.Log("Puzzle solved");
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
