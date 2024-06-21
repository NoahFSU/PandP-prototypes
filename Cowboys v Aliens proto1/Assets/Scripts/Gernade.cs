using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gernade : MonoBehaviour
{
    public int damage = 40;
    public float delay = 3f;
    public float radius = 5f;
    public float force = 700f;

    float countdown;
    bool hasExploded = false;

    public GameObject explosionEffect;
    public AudioSource aud;
    public AudioClip[] explosionSound;
    [Range(0, 1)][SerializeField] float explosionVol;
    // Start is called before the first frame update
    void Start()
    {
        countdown = delay;
    }

    // Update is called once per frame
    void Update()
    {
        countdown -= Time.deltaTime;
        if ((countdown <= 0f && !hasExploded))
        {
            Explode();
            hasExploded = true;
        }
    }
    void Explode()
    {
        Instantiate(explosionEffect, transform.position, transform.rotation);
        aud.PlayOneShot(explosionSound[Random.Range(0, explosionSound.Length)], explosionVol);
        Collider[] collidersToDamage = Physics.OverlapSphere(transform.position, radius);
        foreach(Collider nearbyObject in collidersToDamage)
        {
            IDamage dmg = nearbyObject.GetComponent<IDamage>();
            if(dmg != null)
            {
                dmg.TakeDamage(damage);
            }
        }

        Collider[] collidersToMove = Physics.OverlapSphere(transform.position, radius);
        foreach (Collider nearbyObject in collidersToMove)
        {
            Rigidbody rb = nearbyObject.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.AddExplosionForce(force, transform.position, radius);
            }
        }
        Destroy(gameObject);
    }
}
