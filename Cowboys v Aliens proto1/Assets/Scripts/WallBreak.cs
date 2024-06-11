using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class WallBreak : MonoBehaviour, IDamage
{

    [SerializeField] int WallHP;
    [SerializeField] Renderer WallModel;
    [SerializeField] List<GameObject> drops = new List<GameObject>();


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
            int ran = Random.Range(0, 100);
            if ( ran > 40 && drops.Count != 0)
            {
                if ( ran < 70)
                {
                    if ( ran % 2 > 0)
                    {
                        GameObject smallH = Instantiate(drops[0], transform.position, transform.rotation);
                    }
                    else
                    {
                        GameObject smallA = Instantiate(drops[1], transform.position, transform.rotation);
                    }
                }
                else
                {
                    if (ran % 2 > 0)
                    {
                        GameObject bigH = Instantiate(drops[2], transform.position, transform.rotation);
                    }
                    else
                    {
                        GameObject bigA = Instantiate(drops[3], transform.position, transform.rotation);
                    }
                }
            }
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
