using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WalletPickup : MonoBehaviour
{
    [SerializeField] int currencyAmt;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            gameManager.Instance.playerscript.AddCurrency(currencyAmt);
            Destroy(gameObject);
        }
    }
}
