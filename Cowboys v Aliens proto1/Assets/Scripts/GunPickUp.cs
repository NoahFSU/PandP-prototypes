using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunPickUp : MonoBehaviour
{
    public Sprite gunSprite;
    private bool isPickedUp = false;
    [SerializeField] GunStats gun;
    // Start is called before the first frame update
    void Start()
    {
        gun.totalAmmo = gun.ammoCurrent + gun.magAmmount;
        gun.ammoCurrent = gun.ammoMax + gun.magMax;
        gun.ammoCurrent -= gun.magMax;
        gun.magAmmount = gun.magMax;
    }
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !isPickedUp)
        {
            isPickedUp = true;
            playerController playerInventory = other.GetComponent<playerController>();
            if (playerInventory != null )
            {
                playerInventory.PickupGun(gunSprite);
                gameManager.Instance.playerscript.getGunStats(gun);
                Destroy(gameObject);

            }
        }
    }
}
