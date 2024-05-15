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
        gameManager.Instance.Player.GetComponent<CharacterController>().enabled = false;
        Col.transform.position = toLocation.transform.position;
        gameManager.Instance.Player.GetComponent<CharacterController>().enabled = true;
    }
}

