using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bomb : MonoBehaviour
{

    [SerializeField] int fallSpeed;
    [SerializeField] int bombRadius;
    [SerializeField] int bombDMG;

    [SerializeField] GameObject explosions;

    [SerializeField] Rigidbody rb;

    gameManager player;
    // Start is called before the first frame update
    void Start()
    {
        rb.GetComponent<Rigidbody>();
        rb.velocity = new Vector3(0, -fallSpeed, 0);
    }

    private void OnCollisionEnter(Collision collision)
    {
        //once the bomb collides with the ground it will explode
        if (collision.gameObject.CompareTag("Floor"))
        {
            Explode();
        }
        if (collision.gameObject.CompareTag("Player"))
        {
            Explode();
        }
    }

    void Explode()
    {
        Instantiate(explosions, transform.position, transform.rotation); //Create the explosion when the bomb is colides with the ground

        Collider[] collider = Physics.OverlapSphere(transform.position, bombRadius); //Find all colliders within the bomb radius

        for (int i = 0; i < collider.Length; i++)//if there is colliders, loop through them and dmg them basesd on dmg number
        {
            Collider nearbyObjects = collider[i];

            Rigidbody nerabyRB = nearbyObjects.GetComponent<Rigidbody>();

            if (nerabyRB != null)
            {
                nerabyRB.AddExplosionForce(bombDMG * 10, transform.position, bombRadius);
            }
            IDamage dmg = nearbyObjects.GetComponent<IDamage>();
            if (dmg != null) 
            { 
                dmg.TakeDamage(bombDMG);
            }
        }
        Destroy(gameObject);
    }

    }
