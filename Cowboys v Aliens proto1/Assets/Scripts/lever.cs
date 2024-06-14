using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class lever : MonoBehaviour, IGetLassoed
{
    [SerializeField] Animator anima;
    //create a serialize field for an event to trigger. Trigger that event in the GetLassoed function
    [SerializeField] AudioClip pullLeverSound;

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
            anima.SetBool("Turning On", true);
        }
        

    }
}
