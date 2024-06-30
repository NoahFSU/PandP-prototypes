using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeleportFunction : MonoBehaviour
{
    //-----     This is in testing faze    -------.

    //set a location.
    [SerializeField] Transform toLocation;
    [SerializeField] AudioSource aud;
    public AudioClip playerTp;
    [Range(0, 1)][SerializeField] float audPlayerTPVol;

    //set player (for Now).
    public GameObject thePlayer;
    public bool touched;


    void OnTriggerEnter(Collider Col)
    {
        if(Col.GetComponent<playerController>() != null)
        {
        gameManager.Instance.Player.GetComponent<CharacterController>().enabled = false;
        aud.PlayOneShot(playerTp, audPlayerTPVol);
        Col.transform.position = toLocation.transform.position;
        gameManager.Instance.Player.GetComponent<CharacterController>().enabled = true;
        }

    }
}

