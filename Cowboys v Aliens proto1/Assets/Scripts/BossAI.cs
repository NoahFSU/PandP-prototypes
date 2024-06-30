using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class BossAI : MonoBehaviour, IDamage
{
    [Header("Dependencies")]
    [SerializeField] Animator animat;
    [SerializeField] Renderer model;
    [SerializeField] NavMeshAgent agent;
    [SerializeField] Transform shootingPos;
    [SerializeField] FloatingHealthbar healthbar;

    [SerializeField] GameObject bullet;
    [SerializeField] GameObject bomb;
    [SerializeField] GameObject enemiesToSpawn;
    [SerializeField] Transform[] enemySpawnPos;
    [SerializeField] ParticleSystem spawnEffect;

    [Header("Enemy Values")]
    [SerializeField] float bossHP;
    [SerializeField] float bossMHP;
    [SerializeField] int bossSpeed;
    [SerializeField] int bossDMG;

    [SerializeField] float bossShootRate;
    [SerializeField] float circularShootRate;
    [SerializeField] float circularBulletsAmmount;
    [SerializeField] float patternRate;
    [SerializeField] float spawnAllyRate;

    [SerializeField] int bossHealAmt;
    [SerializeField] float bossHealInterval;

    [SerializeField] int animationSpeedTrans;
    [SerializeField] int faceTargetSpeed;

    int spawnCount;
    int bombCount;
    int rapidFireCount;
    int shootCount;

    Vector3 playerDirection;

    bool isActive;
    bool isHealing;
    bool isAttacking;

    bool isDamaged;
    
    private void Awake()
    {
        healthbar = GetComponentInChildren<FloatingHealthbar>();

        agent.enabled = false;
    }

    // Start is called before the first frame update
    void Start()
    {
        bossHP = bossMHP;
        healthbar.UpdateHealthBar(bossHP, bossMHP);
    }

    // Update is called once per frame
    void Update()
    {
        if (!isActive)
        {
            return;
        }
        float animationSpeed = agent.velocity.normalized.magnitude;
        animat.SetFloat("Speed", Mathf.Lerp(animat.GetFloat("Speed"), animationSpeed, Time.deltaTime * animationSpeedTrans));

        //if (!isHealing && bossHP < bossHP * 0.05f)
        //{
        //    StartCoroutine(HealPassive());
        //}

        FaceTarget();
    }

    IEnumerator BossPattern()
    {
        while (isActive)
        {
            yield return new WaitForSeconds(patternRate);
            if (!isAttacking)
            {
                int randAttack = Random.Range(0, 4);

                switch (randAttack)
                {
                    case 0:
                        {
                            StartCoroutine(DropBombPattern());
                            break;
                        }
                    case 1:
                        {
                            StartCoroutine(SpawnAllies());
                            break;
                        }
                    case 2:
                        {
                            StartCoroutine(CircularShootingPattern());
                            break;
                        }
                    case 3:
                        {
                            StartCoroutine(Shoot());
                            break;
                        }
                }
            }
        }
    }


    public void ActivateBoss()
    {
        //will activate the boss and start the pattern
        isActive = true;
        agent.enabled = true;
        agent.speed = bossSpeed;
        StartCoroutine(BossPattern());
    }

    IEnumerator DropBombPattern()
    {
        StartCoroutine(BossAttackWarning());
        //setting the pattern for the dropping bomb pattern and turns it off after its done
        isAttacking = true;
        bombCount = 0;
        while (bombCount < 10)//keeping count of how many bombs are being dropped to ensure it stops this pattern
        {
            Vector3 playerPosition = gameManager.Instance.Player.transform.position;
            Vector3 bombPosition = new Vector3(playerPosition.x, playerPosition.y + 20f, playerPosition.z);
            animat.SetTrigger("Attack_Boss");
            Instantiate(bomb, bombPosition, Quaternion.identity);
            bombCount++;

            yield return new WaitForSeconds(0.5f);
        }
        isAttacking = false;
    }

    IEnumerator SpawnAllies()
    {
        StartCoroutine(BossAttackWarning());

        isAttacking = true;
        spawnCount = 0;
        //spawns a mini boss
        while (spawnCount < 4)
        {
            int arrayPos = Random.Range(0, enemySpawnPos.Length);
            Vector3 spawnPos = enemySpawnPos[arrayPos].position;
            Quaternion spawnRotation = enemySpawnPos[arrayPos].rotation;

            ParticleSystem effect = Instantiate(spawnEffect, spawnPos, spawnRotation);
            effect.Play();
            animat.SetTrigger("Attack_Boss");
            yield return new WaitForSeconds(effect.main.duration);

            Instantiate(enemiesToSpawn, spawnPos, spawnRotation);
            gameManager.Instance.updateGameGoal(1);

            yield return new WaitForSeconds(spawnAllyRate);
            spawnCount++;

        }
        isAttacking = false;
    }

    IEnumerator Shoot()
    {

        isAttacking = true;
        shootCount = 0;
        while (shootCount < 15)
        {
            animat.SetTrigger("Shoot_Boss");

            GameObject newBullet = Instantiate(bullet, shootingPos.position, transform.rotation);
            IDamage _bossDmg = newBullet.GetComponent<IDamage>();
            if (_bossDmg != null)
            {
                _bossDmg.TakeDamage(bossDMG);
            }
            shootCount++;
            yield return new WaitForSeconds(.5f);
        }
        isAttacking = false;
    }

    IEnumerator CircularShootingPattern()
    {
        StartCoroutine(BossAttackWarning());
        //setting the pattern for the circular shooting and turns it off after its done
        isAttacking = true;
        rapidFireCount = 0;
        float angleStep = 720f / circularBulletsAmmount;

        while (rapidFireCount < circularBulletsAmmount)//keeping count to ensure it stops after a certain time
        {
            //creates bullets that will shoot in a circular motion
            animat.SetTrigger("Shoot_Boss");

            Quaternion rotation = Quaternion.Euler(0, rapidFireCount * angleStep, 0);
            GameObject newBullet = Instantiate(bullet, shootingPos.position, rotation);
            IDamage _bossDmg = newBullet.GetComponent<IDamage>();
            Physics.IgnoreCollision(newBullet.GetComponent<Collider>(), GetComponent<Collider>());
            if (_bossDmg != null)
            {
                _bossDmg.TakeDamage(bossDMG);
            }
            rapidFireCount++;
            yield return new WaitForSeconds(circularShootRate);
        }
        isAttacking = false;
    }

    IEnumerator BossAttackWarning()
    {
        gameManager.Instance.attackWarning.SetActive(true);
        yield return new WaitForSeconds(.25f);
        gameManager.Instance.attackWarning.SetActive(false);
        yield return new WaitForSeconds(.25f);
        gameManager.Instance.attackWarning.SetActive(true);
        yield return new WaitForSeconds(.25f);
        gameManager.Instance.attackWarning.SetActive(false);

    }

    //Already made methods
    public void TakeDamage(int amount)
    {
        bossHP -= amount;
        healthbar.UpdateHealthBar(bossHP, bossMHP);
        StartCoroutine(FlashingRed());

        if (bossHP <= 0)
        {
            gameManager.Instance.statePause();
            gameManager.Instance.menuActive = gameManager.Instance.menuWin;
            gameManager.Instance.menuActive.SetActive(gameManager.Instance.isPaused);
            // gameManager.Instance.updateGameGoal(-1);
            Destroy(gameObject);
            
        }
    }

    void FaceTarget()
    {
        playerDirection = gameManager.Instance.Player.transform.position - transform.position;
        playerDirection.y = 0;
        if (playerDirection != Vector3.zero)
        {

            Quaternion rot = Quaternion.LookRotation(new Vector3(playerDirection.x, playerDirection.y, playerDirection.z));
            transform.rotation = Quaternion.Lerp(transform.rotation, rot, Time.deltaTime * faceTargetSpeed);
        }
    }

    IEnumerator FlashingRed()
    {
        if (isDamaged){
            yield break;
        }
        isDamaged = true;
        Color temp = model.material.color;
        model.material.color = Color.red;
        yield return new WaitForSeconds(0.1f);
        model.material.color = temp;

        isDamaged = false;
    }
}
