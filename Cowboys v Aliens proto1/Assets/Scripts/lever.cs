using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class lever : MonoBehaviour, IGetLassoed
{
    [SerializeField] Animator leverAnimator;
    [SerializeField] Animator doorAnimator;
    [SerializeField] AudioSource aud;
    [SerializeField] AudioClip pullLeverSound;
    [SerializeField] string leverTrigger = "Turn On";
    [SerializeField] string doorTrigger = "Open";

    bool on = false;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void GetLassoed()
    {
        if (!on)
        {
            on = true;
            leverAnimator.SetTrigger(leverTrigger);
            aud.PlayOneShot(pullLeverSound);
        }
      
    }

    public void TriggerOpenDoor()
    {
        if (doorAnimator != null)
        {
            doorAnimator.SetTrigger(doorTrigger);
        }
    }
}
