using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeleportFunction : MonoBehaviour
{
    //-----     This is in testing faze    -------.

    //set a location.
    [SerializeField] Transform toLocation;
    //set player (for Now).
    public GameObject thePlayer;
    public bool touched;


    void OnTriggerEnter(Collider Col)
    {
        touched = !touched;
        //   Col.transform.position = new Vector3(toLocation.transform.position.x, toLocation.transform.position.y, toLocation.transform.position.z);

        //gameManager.Instance.Player.transform.position = toLocation.transform.position;
        thePlayer.transform.position = toLocation.transform.position;
    }
}

