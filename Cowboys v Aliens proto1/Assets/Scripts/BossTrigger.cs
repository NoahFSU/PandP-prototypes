using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class BossTrigger : MonoBehaviour
{
    [SerializeField] private BossAI boss;   

    private void OnTriggerEnter(Collider other)
    {
        //activate the boss once player enters the scene, then turns off the trigger
        if (other.CompareTag("Player"))
        {
            boss.ActivateBoss();
            gameObject.SetActive(false);
        }
    }
}
