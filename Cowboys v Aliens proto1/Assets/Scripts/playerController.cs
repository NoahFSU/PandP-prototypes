using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
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
    [SerializeField] float crouchSpeedMod = 0.5f;

    [SerializeField] GameObject muzzleFlash;
    [SerializeField] GameObject gunModel;
    [SerializeField] int shootDamage;
    [SerializeField] int shootDist;
    [SerializeField] float shootRate;
    [SerializeReference] List<GunStats> gunList = new List<GunStats>();
    [SerializeField] AudioSource aud;
    [SerializeField] AudioClip[] audPlayerHit;
    [Range(0, 1)][SerializeField] float audPlayerHitVol;
    [SerializeField] AudioClip[] audJump;
    [Range(0, 1)][SerializeField] float audJumpVol;
    [SerializeField] Animator ReloadAnim;
    [SerializeField] GameObject lassoPrefab;



    Vector3 moveDir;
    Vector3 playerVel;

    bool isReloading = false;
    int jumpCount;
    int HPOrig;
    bool isShooting;
    int selectedGun;
    bool isCrouching = false;
    float origHeight;
    int origSpeed;

    // Start is called before the first frame update
    void Start()
    {
        HPOrig = HP;
        origHeight = controller.height;
        origSpeed = speed;
        SpawnPlayer();

    }


    // Update is called once per frame
    void Update()
    {
        if (!gameManager.Instance.isPaused)
        {
            Debug.DrawRay(Camera.main.transform.position, Camera.main.transform.forward * shootDist, Color.red);
            movement();
            selectGun();
            if (Input.GetButtonDown("Fire2"))
            {
                ThrowLasso();
            }
        }
    }
    void OnEnable()
    {

        isReloading = false;
        ReloadAnim.SetBool("Reloading", false);

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
        Crouch();

        if (Input.GetButtonDown("Jump") && jumpCount < jumpMax)
        {
            aud.PlayOneShot(audJump[Random.Range(0, audJump.Length)], audJumpVol);
            jumpCount++;
            playerVel.y = jumpSpeed;
        }
        playerVel.y -= gravity * Time.deltaTime;
        controller.Move(playerVel * Time.deltaTime);

        if (isReloading)
            return;
        if (Input.GetButton("Fire1") && gunList.Count > 0 && gunList[selectedGun].magAmmount > 0 && !isShooting)
        {
            StartCoroutine(shoot());
        }
        else if (Input.GetButton("Fire1") && gunList.Count > 0 && gunList[selectedGun].magAmmount <= 0 && !isShooting)
        {
            GetComponent<AudioSource>().PlayOneShot(gunList[selectedGun].emptySound, gunList[selectedGun].emptyVol);
        }
        if (Input.GetButtonDown("Reload"))
        {
            if (gunList[selectedGun].ammoCurrent > 0)
            {
                StartCoroutine(reload());
            }
        }
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
        ReloadAnim.SetBool("Reloading", true);
        isReloading = true;
        //  gameManager.Instance.reloadUI.SetActive(true);

        GetComponent<AudioSource>().PlayOneShot(gunList[selectedGun].reloadSound, gunList[selectedGun].reloadVol);
        yield return new WaitForSeconds(gunList[selectedGun].reloadTime - .25f);
        ReloadAnim.SetBool("Reloading", false);
        yield return new WaitForSeconds(.25f);
        gunList[selectedGun].ammoCurrent -= (gunList[selectedGun].magMax - gunList[selectedGun].magAmmount);
        gunList[selectedGun].magAmmount += gunList[selectedGun].magMax - gunList[selectedGun].magAmmount;
        isReloading = false;
        UpdateAmmoUi();

        // gameManager.Instance.reloadUI.SetActive(false);
    }
    IEnumerator shoot()
    {
        isShooting = true;

        GetComponent<AudioSource>().PlayOneShot(gunList[selectedGun].shootSound, gunList[selectedGun].shootVol);
        gunList[selectedGun].magAmmount--;

        StartCoroutine(FlashMuzzle());

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
        if (hit.collider.gameObject.GetComponent<MatStats>() != null)
        {
            Instantiate(hit.collider.gameObject.GetComponent<MatStats>().hitEffect, hit.point, Quaternion.identity);
        }


        UpdateAmmoUi();
        yield return new WaitForSeconds(shootRate);
        isShooting = false;
    }
    IEnumerator FlashMuzzle()
    {
        muzzleFlash.SetActive(true);
        yield return new WaitForSeconds(0.05f);
        muzzleFlash.SetActive(false);
    }
    public void TakeDamage(int amount)
    {
        aud.PlayOneShot(audPlayerHit[Random.Range(0, audPlayerHit.Length)], audPlayerHitVol);
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
    void UpdateAmmoUi()
    {

        gameManager.Instance.magAmmoText.text = gunList[selectedGun].magAmmount.ToString("F0");

        gameManager.Instance.reserverAmmoText.text = gunList[selectedGun].ammoCurrent.ToString("F0");


    }
    public void getGunStats(GunStats gun)
    {
        gunList.Add(gun);

        selectedGun = gunList.Count - 1;

        shootDamage = gun.shootDamage;
        shootRate = gun.shootRate;
        shootDist = gun.shootDistance;

        gameManager.Instance.magAmmoText.text = gun.magMax.ToString("F0");
        gameManager.Instance.reserverAmmoText.text = gun.ammoCurrent.ToString("F0");

        gunModel.GetComponent<MeshFilter>().sharedMesh = gun.gunModel.GetComponent<MeshFilter>().sharedMesh;
        gunModel.GetComponent<MeshRenderer>().sharedMaterial = gun.gunModel.GetComponent<MeshRenderer>().sharedMaterial;
    }
    void selectGun()
    {

        if (Input.GetAxis("Mouse ScrollWheel") > 0 && selectedGun < gunList.Count - 1)
        {
            selectedGun++;
            changeGun();
            UpdateAmmoUi();
        }
        else if (Input.GetAxis("Mouse ScrollWheel") < 0 && selectedGun > 0)
        {
            selectedGun--;
            changeGun();
            UpdateAmmoUi();
        }
    }
    void changeGun()
    {
        shootDamage = gunList[selectedGun].shootDamage;
        shootRate = gunList[selectedGun].shootRate;
        shootDist = gunList[selectedGun].shootDistance;

        gunModel.GetComponent<MeshFilter>().sharedMesh = gunList[selectedGun].gunModel.GetComponent<MeshFilter>().sharedMesh;
        gunModel.GetComponent<MeshRenderer>().sharedMaterial = gunList[selectedGun].gunModel.GetComponent<MeshRenderer>().sharedMaterial;
        GetComponent<AudioSource>().PlayOneShot(gunList[selectedGun].equipSound, gunList[selectedGun].equipVol);
    }

    public void SpawnPlayer()
    {
        HP = HPOrig;
        UpdatePlayerUI();

        controller.enabled = false;
        transform.position = gameManager.Instance.playerSpawnPos.transform.position;
        controller.enabled = true;
    }
    void Crouch()
    {
        if (Input.GetButtonDown("Crouch"))
        {
            isCrouching = !isCrouching;
            if (isCrouching)
            {
                controller.height = origHeight / 2;
                speed = (int)(speed * crouchSpeedMod);
            }
            else
            {
                controller.height = origHeight;
                speed = (int)(speed / crouchSpeedMod);
            }
        }

    }

    //methods for item pickups
    public void RestoreHealth(int amount)
    {
        HP += amount;
        if (HP > HPOrig)
        {
            HP = HPOrig;
        }
        UpdatePlayerUI();
    }

    public void AddAmmo(int amount)
    {
        if (gunList.Count > 0)
        {
            gunList[selectedGun].ammoCurrent += amount;

            gameManager.Instance.reserverAmmoText.text = gunList[selectedGun].ammoCurrent.ToString("F0");
        }

    }

    void ThrowLasso()
    {
        Vector3 spawnPosition = Camera.main.transform.position + Camera.main.transform.forward;
        GameObject lassoInstance = Instantiate(lassoPrefab, spawnPosition, Camera.main.transform.rotation);
        Rigidbody rb = lassoInstance.GetComponent<Rigidbody>();
        rb.velocity = Camera.main.transform.forward * 10f;
    }
}
