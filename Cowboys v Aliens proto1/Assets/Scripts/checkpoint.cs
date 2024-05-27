using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    [SerializeField] Renderer model;
    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player") && gameManager.Instance.playerSpawnPos.transform.position != transform.position)
        {
            gameManager.Instance.playerSpawnPos.transform.position = transform.position;
            StartCoroutine(DisplayPopup());    
        }
    }

    IEnumerator DisplayPopup()
    {
        model.material.color = Color.green;
        gameManager.Instance.checkpointPopup.SetActive(true);
        yield return new WaitForSeconds(1.5f);
        gameManager.Instance.checkpointPopup.SetActive(false);
        model.material.color = Color.white;
    }
}
