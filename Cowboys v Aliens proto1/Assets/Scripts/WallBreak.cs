using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallBreak : MonoBehaviour, IDamage
{

    [SerializeField] int WallHP;
    [SerializeField] Renderer WallModel;


    // Update is called once per frame
    void Update()
    {
        
    }

    public void TakeDamage(int amount)
    {
        WallHP -= amount;
        StartCoroutine(FlashingRed());

        if (WallHP <= 0)
        {
            Destroy(gameObject);
        }
    }

    IEnumerator FlashingRed()
    {
        WallModel.material.color = Color.red;
        yield return new WaitForSeconds(0.1f);
        WallModel.material.color = Color.white;
    }
}
