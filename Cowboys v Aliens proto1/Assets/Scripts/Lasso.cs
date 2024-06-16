using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lasso : MonoBehaviour
{
    [SerializeField] float lassoSpeed;
    [SerializeField] float lassoDistance;
    [SerializeField] GameObject lassoAroundObjectPrefab;

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
            InstantiateLassoAroundObject(other.gameObject);
        }
    }

    private void DestroyLasso()
    {
        if (player != null)
        {
            player.LassoDestroyed();
        }
        Destroy(gameObject);
    }

    private void InstantiateLassoAroundObject(GameObject obj)
    {
        GameObject lassoAroundObject = Instantiate(lassoAroundObjectPrefab, obj.transform.position, Quaternion.identity);
        lassoAroundObject.transform.SetParent(obj.transform);
        lassoAroundObject.GetComponent<LineRenderer>().SetPosition(0, player.transform.position);
        lassoAroundObject.GetComponent<LineRenderer>().SetPosition(1, obj.transform.position);
        gameManager.Instance.SetLassoedEnemy(obj);
    }

    


}
