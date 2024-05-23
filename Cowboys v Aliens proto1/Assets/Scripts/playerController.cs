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

    [SerializeField] GameObject gunModel;
    [SerializeField] int shootDamage;
    [SerializeField] int shootDist;
    [SerializeField] float shootRate;
    [SerializeReference] List<GunStats> gunList = new List<GunStats>();


    Vector3 moveDir;
    Vector3 playerVel;
    Vector3 playerPos; //Why Do we have this?

    int jumpCount;
    int HPOrig;
    bool isShooting;
    int selectedGun;


    // Start is called before the first frame update
    void Start()
    {
        HPOrig = HP;
        UpdatePlayerUI();

    }

    // Update is called once per frame
    void Update()
    {
        if (!gameManager.Instance.isPaused)
        {
            Debug.DrawRay(Camera.main.transform.position, Camera.main.transform.forward * shootDist, Color.red);
            movement();
            selectGun();
        }
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

        if (Input.GetButton("Fire1") && gunList.Count > 0 && gunList[selectedGun].magAmmount > 0 && !isShooting)
        {
            StartCoroutine(shoot());
        }
        if (Input.GetButton("Reload"))
        {
            if (gunList[selectedGun].ammoCurrent > 0)
            {
                StartCoroutine(reload());
            }
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
    IEnumerator reload()
    {
        yield return new WaitForSeconds(gunList[selectedGun].reloadTime);
        gunList[selectedGun].ammoCurrent -= (gunList[selectedGun].magMax - gunList[selectedGun].magAmmount);
        gunList[selectedGun].magAmmount += gunList[selectedGun].magMax - gunList[selectedGun].magAmmount;

    }
    IEnumerator shoot()
    {
        isShooting = true;
        gunList[selectedGun].magAmmount--;
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
    public void getGunStats(GunStats gun)
    {
        gunList.Add(gun);
        selectedGun = gunList.Count - 1;
        shootDamage = gun.shootDamage;
        shootRate = gun.shootRate;
        shootDist = gun.shootDistance;

        gunModel.GetComponent<MeshFilter>().sharedMesh = gun.gunModel.GetComponent<MeshFilter>().sharedMesh;
        gunModel.GetComponent<MeshRenderer>().sharedMaterial = gun.gunModel.GetComponent<MeshRenderer>().sharedMaterial;
    }
    void selectGun()
    {
        if (Input.GetAxis("Mouse ScrollWheel") > 0 && selectedGun < gunList.Count - 1)
        {
            selectedGun++;
            changeGun();
        }
        else if (Input.GetAxis("Mouse ScrollWheel") < 0 && selectedGun > 0)
        {
            selectedGun--;
            changeGun();
        }
    }
    void changeGun()
    {
        shootDamage = gunList[selectedGun].shootDamage;
        shootRate = gunList[selectedGun].shootRate;
        shootDist = gunList[selectedGun].shootDistance;

        gunModel.GetComponent<MeshFilter>().sharedMesh = gunList[selectedGun].gunModel.GetComponent<MeshFilter>().sharedMesh;
        gunModel.GetComponent<MeshRenderer>().sharedMaterial = gunList[selectedGun].gunModel.GetComponent<MeshRenderer>().sharedMaterial;
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
