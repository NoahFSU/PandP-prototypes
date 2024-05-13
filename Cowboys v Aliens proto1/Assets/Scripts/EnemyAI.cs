using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour, IDamage
{
    [SerializeField] Renderer model;
    [SerializeField] NavMeshAgent agent;
    [SerializeField] Transform shootingPos;

    [SerializeField] int enemyHP;
    [SerializeField] int enemySpeed;
    [SerializeField] int enemyShootingDMG;
    [SerializeField] float enemyShooingRate;
    [SerializeField] GameObject bullet;


    bool isShooting;
    bool isPlayerInRange;

    // Start is called before the first frame update
    void Start()
    {
        gameManager.Instance.updateGameGoal(1);
        agent.speed = enemySpeed;
    }

    // Update is called once per frame
    void Update()
    {
        if(isPlayerInRange)
        {
            agent.SetDestination(gameManager.Instance.Player.transform.position);
            if(!isShooting)
            {
                StartCoroutine(Shoot());
            }
        }
    }
 
    public void TakeDamage(int amount)
    {
        enemyHP -= amount;
        agent.SetDestination(gameManager.Instance.Player.transform.position);
        StartCoroutine(FlashingRed());

        if(enemyHP <= 0)
        {
            gameManager.Instance.updateGameGoal(-1);
            Destroy(gameObject);
        }
    }

    IEnumerator Shoot()
    {
        isShooting = true;
        GameObject newBullet = Instantiate(bullet, shootingPos.position, transform.rotation);
        IDamage enemyDmg = newBullet.GetComponent<IDamage>();
        if(enemyDmg != null)
        {
            enemyDmg.TakeDamage(enemyShootingDMG);
        }

        yield return new WaitForSeconds(enemyShooingRate);
        isShooting = false;
    }

    IEnumerator FlashingRed()
    {
        model.material.color = Color.red;
        yield return new WaitForSeconds(0.1f);
        model.material.color = Color.white;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = true;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = false;
        }
    }
}
