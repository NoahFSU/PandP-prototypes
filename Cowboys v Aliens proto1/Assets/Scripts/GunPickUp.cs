using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunPickUp : MonoBehaviour
{
    [SerializeField] GunStats gun;
    // Start is called before the first frame update
    void Start()
    {
        gun.ammoCurrent = gun.ammoMax + gun.magMax;
        gun.ammoCurrent -= gun.magMax;
        gun.magAmmount = gun.magMax;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            gameManager.Instance.playerscript.getGunStats(gun);
            Destroy(gameObject);
        }
    }

}
