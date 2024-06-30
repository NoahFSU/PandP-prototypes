using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class BossTrigger : MonoBehaviour
{
    [SerializeField] GameObject bossObject;
    [SerializeField] private BossAI boss;   

    private void OnTriggerEnter(Collider other)
    {
        bossObject.SetActive(false);
        //activate the boss once player enters the scene, then turns off the trigger
        if (other.CompareTag("Player"))
        {
            bossObject.SetActive(true);
            boss.ActivateBoss();
            gameObject.SetActive(false);
        }
    }
}
