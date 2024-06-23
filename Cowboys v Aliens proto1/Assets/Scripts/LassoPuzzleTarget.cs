using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LassoPuzzleTarget : MonoBehaviour, IGetLassoed
{
   [SerializeField] public int targetOrder;
    private bool isLassoed = false;
    private Renderer targetRenderer;
    private Color originalColor;
    // Start is called before the first frame update
    private void Start()
    {
        targetRenderer = GetComponent<Renderer>();
        originalColor = targetRenderer.material.color;
    }
    public void GetLassoed()
    {
        if (!isLassoed)
        {
            isLassoed = true;
            LassoPuzzleManager.Instance.TargetLassoed(this);
        }
    }

    public void ResetLassoed()
    {
        isLassoed=false;
        targetRenderer.material.color = originalColor;
    }

    public void TurnGreen()
    {
        targetRenderer.material.color = Color.green;
    }    

    public void FlashRed()
    {
        StartCoroutine(FlashRedCoroutine());
    }

    private IEnumerator FlashRedCoroutine()
    {
        Color redColor = Color.red;
        for (int i = 0; i < 3; i++)
        {
            targetRenderer.material.color = redColor;
            yield return new WaitForSeconds(0.25f);
            targetRenderer.material.color = originalColor;
            yield return new WaitForSeconds(0.25f);
        }
    }
}
