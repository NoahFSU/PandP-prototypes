using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour, IDamage, IGetLassoed
{
    [Header("Dependencies")]
    [SerializeField] Animator anima;
    [SerializeField] Renderer model;
    [SerializeField] NavMeshAgent agent;
    [SerializeField] Transform shootingPos;
    [SerializeField] Transform headPos;
    [SerializeField] FloatingHealthbar healthbar;
    [SerializeField] GameObject bullet;
    [Header("Enemy Values")]
    [SerializeField] float enemyHP, enemyMHP = 3f;
    [SerializeField] int enemySpeed;
    [SerializeField] int enemyShootingDMG;
    [SerializeField] float enemyShooingRate;
    [SerializeField] int viewAngle;
    [SerializeField] int faceTargetSpeed;
    [SerializeField] int roamingDistance;
    [SerializeField] int roamTimer;
    [SerializeField] int animationSpeedTrans;
    [SerializeField] float immobilizeDuration;
    [SerializeField] List<GameObject> drops = new List<GameObject>();
    [Range(0,100)] [SerializeField] int dropRate;
    [SerializeField] GameObject head;


    Vector3 playerDirection;
    Vector3 startingPos;

    float angleToTarget;
    float stoppingDistanceOrig;

    bool isShooting;
    bool isPlayerInRange;
    bool destinationChoosen;
    bool isImmobilized;

    bool isDamaged;

    private void Awake()
    {
        healthbar = GetComponentInChildren<FloatingHealthbar>();
    }
    // Start is called before the first frame update
    void Start()
    {
        enemyHP = enemyMHP;

        healthbar.UpdateHealthBar(enemyHP, enemyMHP);

        startingPos = transform.position;
        stoppingDistanceOrig = agent.stoppingDistance;

        agent.enabled = true;
        agent.speed = enemySpeed;
    }

    // Update is called once per frame
    void Update()
    {
        float animationSpeed = agent.velocity.normalized.magnitude;

        anima.SetFloat("Speed", Mathf.Lerp(anima.GetFloat("Speed"), animationSpeed, Time.deltaTime * animationSpeedTrans));


        if (isPlayerInRange && !CanSeePlayer())
        {
            StartCoroutine(Roaming());
        }
        else if (!isPlayerInRange)
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

    bool CanSeePlayer()
    {
        playerDirection = gameManager.Instance.Player.transform.position - headPos.position;
        angleToTarget = Vector3.Angle(new Vector3(playerDirection.x, playerDirection.y + 1, playerDirection.z), transform.forward);

        RaycastHit hit;
        if (Physics.Raycast(headPos.position, playerDirection, out hit))
        {
            if (hit.collider.CompareTag("Player") && angleToTarget <= viewAngle)
            {
                agent.stoppingDistance = stoppingDistanceOrig;
                agent.SetDestination(gameManager.Instance.Player.transform.position);
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
        Quaternion rot = Quaternion.LookRotation(new Vector3(playerDirection.x, playerDirection.y, playerDirection.z));
        transform.rotation = Quaternion.Lerp(transform.rotation, rot, Time.deltaTime * faceTargetSpeed);
    }

    public void TakeDamage(int amount)
    {
        
        enemyHP -= amount;
        healthbar.UpdateHealthBar(enemyHP, enemyMHP);
        agent.SetDestination(gameManager.Instance.Player.transform.position);
        StartCoroutine(FlashingRed());

        if (enemyHP <= 0)
        {
           // gameManager.Instance.updateGameGoal(-1);
           if( Random.Range(1,100) <= dropRate && drops.Count != 0)
            {
                GameObject drop = Instantiate(drops[Random.Range(1,drops.Count) - 1], transform.position, transform.rotation);
            }
            Destroy(gameObject);
        }
    }

    public void GetLassoed()
    {
        if (!isImmobilized)
        {
            StartCoroutine(ImmobilizeCoroutine());
        }
    }

    IEnumerator ImmobilizeCoroutine()
    {
        isImmobilized = true;
        agent.isStopped = true;
        anima.enabled = false;
        gameManager.Instance.SetLassoedEnemy(gameObject);
        yield return new WaitForSeconds(immobilizeDuration);
        agent.isStopped = false;
        anima.enabled = true;
        isImmobilized = false;
        gameManager.Instance.ClearLassoedEnemy();
    }

    IEnumerator Shoot()
    {
        isShooting = true;

        anima.SetTrigger("Shoot");

        yield return new WaitForSeconds(enemyShooingRate);
        isShooting = false;
    }

    public void CreateBullet()
    {
        Vector3 targetDirec = (gameManager.Instance.Player.transform.position - shootingPos.position).normalized;
        GameObject newBullet = Instantiate(bullet, shootingPos.position, Quaternion.LookRotation(targetDirec));
        IDamage enemyDmg = newBullet.GetComponent<IDamage>();

        if (enemyDmg != null)
        {
            enemyDmg.TakeDamage(enemyShootingDMG);
        }
    }

    IEnumerator FlashingRed()
    {
        if (isDamaged)
        {
            yield break;
        }
        isDamaged = true;
        Color temp = model.material.color;
        model.material.color = Color.red;
        yield return new WaitForSeconds(0.1f);
        model.material.color = temp;

        isDamaged = false;
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
            agent.stoppingDistance = 0;
        }
    }
}
