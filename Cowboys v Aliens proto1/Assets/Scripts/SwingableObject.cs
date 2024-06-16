using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwingableObject : MonoBehaviour, IGetLassoed
{
    private LineRenderer lineRenderer;
    private Transform player;
    private bool isSwinging = false;
   
    // Start is called before the first frame update
    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        player = GameObject.FindWithTag("Player").transform;
        
    }

    // Update is called once per frame
    void Update()
    {
        if (isSwinging)
        {
            lineRenderer.SetPosition(0, transform.position);
            lineRenderer.SetPosition(1, player.position);
        }
        
    }

    public void GetLassoed()
    {
        isSwinging = true;
        lineRenderer.enabled = true;
    }

    public void StopSwinging()
    {
        isSwinging = false;
        lineRenderer.enabled=false;
    }
}
