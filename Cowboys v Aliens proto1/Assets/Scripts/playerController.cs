using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerController : MonoBehaviour, IDamage
{
    [SerializeField] int HP;
    [SerializeField] CharacterController controller;
    [SerializeField] int speed;
    [SerializeField] int sprintMod;
    [SerializeField] int jumpMax;
    [SerializeField] int jumpSpeed;
    [SerializeField] int gravity;


    [SerializeField] int shootDamage;
    [SerializeField] int shootDist;
    [SerializeField] float shootRate;
    

    Vector3 moveDir;
    Vector3 playerVel;
    Vector3 playerPos;

    int jumpCount;
    int HPOrig;
    bool isShooting;


    // Start is called before the first frame update
    void Start()
    {
        HPOrig = HP;
        UpdatePlayerUI();

    }

    // Update is called once per frame
    void Update()
    {
        movement();
        Debug.DrawRay(Camera.main.transform.position, Camera.main.transform.forward * shootDist, Color.red);
    }
    void movement()
    {
        if (controller.isGrounded)
        {
            jumpCount = 0;
            playerVel = Vector3.zero;
        }

        moveDir = (Input.GetAxis("Horizontal") * transform.right) +
            (Input.GetAxis("Vertical") * transform.forward);
        controller.Move(moveDir * speed * Time.deltaTime);

        Sprint();

        if (Input.GetButton("Fire1") && !isShooting)
        {
            StartCoroutine(shoot());
        }
        //Crouch();

        if (Input.GetButtonDown("Jump") && jumpCount < jumpMax)
        {
            jumpCount++;
            playerVel.y = jumpSpeed;
        }
        playerVel.y -= gravity * Time.deltaTime;
        controller.Move(playerVel * Time.deltaTime);
    }
    void Sprint()
    {
        if (Input.GetButtonDown("Sprint"))
        {
            speed *= sprintMod;
        }
        else if (Input.GetButtonUp("Sprint"))
        {
            speed /= sprintMod;
        }
    }

    IEnumerator shoot()
    {
        isShooting = true;
        RaycastHit hit;
        if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, shootDist))
        {
            Debug.Log(hit.transform.name);
            IDamage dmg = hit.collider.GetComponent<IDamage>();
           
            if (hit.transform != transform && dmg != null)
            {
                dmg.TakeDamage(shootDamage);
            }
        }

        yield return new WaitForSeconds(shootRate);
        isShooting = false;
    }

    public void TakeDamage(int amount)
    {
        HP -= amount;
        UpdatePlayerUI();
        StartCoroutine(flashScreenDamage());
       
        if (HP <= 0)
        {
            gameManager.Instance.youLose();
        }
    }
    IEnumerator flashScreenDamage()
    {
        gameManager.Instance.playerFlashDamage.SetActive(true);
        yield return new WaitForSeconds(0.1f);
        gameManager.Instance.playerFlashDamage.SetActive(false);

    }
    void UpdatePlayerUI()
    {
        gameManager.Instance.playerHPBar.fillAmount = (float)HP / HPOrig;
    }

    void Crouch()
    {
        if (Input.GetButton("Crouch") && controller.isGrounded)
        {
            controller.height = 0.5f;
        }
        else
            controller.height = 1.0f;
            
    }
}
