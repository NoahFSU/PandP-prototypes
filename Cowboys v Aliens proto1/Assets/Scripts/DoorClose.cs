using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorClose : MonoBehaviour
{
    [SerializeField] GameObject door;

    private void OnTriggerEnter(Collider other)
    {
        door.SetActive(false);
        if (other.CompareTag("Player"))
        {
            door.SetActive(true);
            gameObject.SetActive(false);
        }
    }
}
