using UnityEngine;
using System.Collections;

public class FriendlyAI : MonoBehaviour, IDamage
{
    [SerializeField] int turretHP;
    [SerializeField] Transform shootingPos;
    [SerializeField] GameObject bullet;
    [SerializeField] int shootingDamage;
    [SerializeField] float shootingRate;
    [SerializeField] float rotationSpeed;
    [SerializeField] float shootingRange;

    bool isShooting;
    Transform target; // Target to shoot at

    // Start is called before the first frame update
    void Start()
    {
        if (shootingPos == null)
        {
            Debug.LogError("Shooting position not assigned for FriendlyAI: " + gameObject.name);
        }
    }

    // Update is called once per frame
    void Update()
    {
        // Find nearest enemy within shooting range
        FindNearestEnemy();

        if (target != null)
        {
            // Rotate towards the target
            Vector3 targetDir = target.position - transform.position;
            Quaternion targetRotation = Quaternion.LookRotation(targetDir);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

            // Check if the target is within shooting range and line of sight
            if (!isShooting && CanSeeTarget())
            {
                StartCoroutine(Shoot());
            }
        }
    }

    IEnumerator Shoot()
    {
        isShooting = true;

        Instantiate(bullet, shootingPos.position, transform.rotation);

        yield return new WaitForSeconds(shootingRate);

        isShooting = false;
    }

    void FindNearestEnemy()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        float closestDistance = Mathf.Infinity;
        Transform nearestEnemy = null;

        foreach (GameObject enemy in enemies)
        {
            float distance = Vector3.Distance(transform.position, enemy.transform.position);
            if (distance < closestDistance && distance <= shootingRange)
            {
                closestDistance = distance;
                nearestEnemy = enemy.transform;
            }
        }

        target = nearestEnemy;
    }

   

    bool CanSeeTarget()
    {
        if (target == null)
            return false;

        Vector3 directionToTarget = target.position - shootingPos.position;
        directionToTarget.y = directionToTarget.y + 1;
        RaycastHit hit;
        Debug.DrawRay(transform.position, directionToTarget);

        if (Physics.Raycast(transform.position, directionToTarget, out hit))
        {
            if (hit.collider.CompareTag("Enemy"))
            {
                return true;
            }
        }

        return false;
    }

    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
    }

    public void TakeDamage(int amount)
    {
        turretHP -= amount;

        if (turretHP <= 0)
        {
            Destroy(gameObject);
        }
    }
    public void Immobilize()
    {

    }
}
