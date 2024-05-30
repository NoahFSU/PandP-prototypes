using System.Collections;
using System.Collections.Generic;
using UnityEditor.Timeline;
using UnityEngine;
using UnityEngine.AI;

public class FriendlyAI : MonoBehaviour, IDamage
{
    [SerializeField] Renderer model;
    [SerializeField] NavMeshAgent agent;
    [SerializeField] Transform shootingPos;

    //uncomment when the model is fully implemented
    //[SerializeField] Transform headPos; 
    [SerializeField] FloatingHealthbar healthbar;

    [SerializeField] GameObject bullet;
    [SerializeField] float friendlyHP, friendlyMHP;
    [SerializeField] int friendlySpeed;
    [SerializeField] int friendlyShootingDMG;
    [SerializeField] float friendlyShooingRate;

    [SerializeField] int viewAngle;
    [SerializeField] int faceTargetSpeed;
    [SerializeField] int roamingDistance;
    [SerializeField] int roamTimer;

    Vector3 enemyDirection;
    Vector3 startingPos;

    float angleToTarget;
    float stoppingDistanceOrig;

    bool isShooting;
    bool isEnemyInRange;
    bool destinationChoosen;


    // Start is called before the first frame update
    void Start()
    {
        healthbar.UpdateHealthBar(friendlyHP, friendlyHP);

        startingPos = transform.position;
        stoppingDistanceOrig = agent.stoppingDistance;

        agent.speed = friendlySpeed;
    }

    // Update is called once per frame
    void Update()
    {
        if (isEnemyInRange && !CanSeeEnemy())
        {
            StartCoroutine(Roaming());
        }
        else if (!isEnemyInRange)
        {
            StartCoroutine(Roaming());
        }
    }

    IEnumerator Roaming()
    {
        if (!destinationChoosen && agent.remainingDistance < 0.05f)
        {
            destinationChoosen = true;
            agent.stoppingDistance = 0;

            yield return new WaitForSeconds(roamTimer);

            Vector3 randPos = Random.insideUnitSphere * roamingDistance;
            randPos += startingPos;

            NavMeshHit hit;
            NavMesh.SamplePosition(randPos, out hit, roamingDistance, 1);
            agent.SetDestination(hit.position);

            destinationChoosen = false;
        }
    }

    bool CanSeeEnemy()
    {
        enemyDirection = gameManager.Instance.Enemy.transform.position - transform.position;//headPos.position;
        angleToTarget = Vector3.Angle(new Vector3(enemyDirection.x, enemyDirection.y + 1, enemyDirection.z), transform.forward);

        //comment out when everything is working perfectly fine
        Debug.Log(angleToTarget);
        Debug.DrawRay(/*headPos.position*/ transform.position, enemyDirection);

        RaycastHit hit;
        if (Physics.Raycast(/*headPos.position*/ transform.position, enemyDirection, out hit))
        {
            if (hit.collider.CompareTag("Enemy") && angleToTarget <= viewAngle)
            {
                agent.stoppingDistance = stoppingDistanceOrig;
                agent.SetDestination(gameManager.Instance.Enemy.transform.position);
                if (!isShooting)
                {
                    StartCoroutine(Shoot());
                }

                if (agent.remainingDistance <= agent.stoppingDistance)
                {
                    FaceTarget();
                }
                return true;
            }
        }
        agent.stoppingDistance = 0;
        return false;
    }

    void FaceTarget()
    {
        Quaternion rot = Quaternion.LookRotation(new Vector3(enemyDirection.x, enemyDirection.y, enemyDirection.z));
        transform.rotation = Quaternion.Lerp(transform.rotation, rot, Time.deltaTime * faceTargetSpeed);
    }

    public void TakeDamage(int amount)
    {
        friendlyHP -= amount;
        healthbar.UpdateHealthBar(friendlyHP, friendlyMHP);
        agent.SetDestination(gameManager.Instance.Enemy.transform.position);
        StartCoroutine(FlashingRed());

        if (friendlyHP <= 0)
        {
            Destroy(gameObject);
        }
    }

    IEnumerator Shoot()
    {
        isShooting = true;
        GameObject newBullet = Instantiate(bullet, shootingPos.position, transform.rotation);
        IDamage enemyDmg = newBullet.GetComponent<IDamage>();
        if (enemyDmg != null)
        {
            enemyDmg.TakeDamage(friendlyShootingDMG);
        }

        yield return new WaitForSeconds(friendlyShooingRate);
        isShooting = false;
    }

    IEnumerator FlashingRed()
    {
        Color temp = model.material.color;
        model.material.color = Color.red;
        yield return new WaitForSeconds(0.1f);
        model.material.color = temp;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            isEnemyInRange = true;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            isEnemyInRange = false;
            agent.stoppingDistance = 0;
        }
    }
}