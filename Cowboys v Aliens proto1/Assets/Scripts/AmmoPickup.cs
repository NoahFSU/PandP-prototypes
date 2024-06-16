using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmmoPickup : MonoBehaviour, IGetLassoed
{
    [SerializeField] int ammoAmount;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            gameManager.Instance.playerscript.AddAmmo(ammoAmount);
            Destroy(gameObject);
        }
    }

    public void GetLassoed()
    {
       
    }
}
