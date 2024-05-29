using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthPickup : MonoBehaviour
{
    [SerializeField] int healthAmount;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            gameManager.Instance.playerscript.RestoreHealth(healthAmount);
            gameManager.Instance.playerHPBar.fillAmount = healthAmount;
            Destroy(gameObject);
        }
    }

}
