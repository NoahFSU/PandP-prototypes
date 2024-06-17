using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lasso : MonoBehaviour
{
    [SerializeField] float lassoSpeed;
    [SerializeField] float lassoDistance;

    private Rigidbody rb;
    private Vector3 startPosition;
    private LineRenderer lineRenderer;
    private playerController player;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.velocity = Camera.main.transform.forward * lassoSpeed;
        startPosition = transform.position; 
        lineRenderer = GetComponent<LineRenderer>();   
        player = GameObject.FindWithTag("Player").GetComponent<playerController>();
        if (lineRenderer != null )
        {
            lineRenderer.positionCount = 2;
        }
    }

    void Update()
    {
      if (Vector3.Distance(startPosition, transform.position) >= lassoDistance)
        {
            DestroyLasso();
        }
      if (lineRenderer != null && player != null )
        {
            lineRenderer.SetPosition(0, player.transform.position);
            lineRenderer.SetPosition(1,transform.position);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.isTrigger)
        {
            return;
        }
        if(other.CompareTag("Floor"))
        {
            DestroyLasso();
            return;
        }

        IGetLassoed gotLassoed = other.gameObject.GetComponent<IGetLassoed>();
        if (gotLassoed != null)
        {
            gotLassoed.GetLassoed();
            DestroyLasso();
            if(other.gameObject.GetComponent<SwingableObject>() != null)
            {
                player.StartSwinging(other.transform.position);
            }
        }
    }

    public void DestroyLasso()
    {
        if (player != null)
        {
            player.LassoDestroyed();
        }
        Destroy(gameObject);
    }

}
