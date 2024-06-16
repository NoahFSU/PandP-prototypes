using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class playerController : MonoBehaviour, IDamage
{
    [SerializeField] CharacterController controller;

    [Header("Player Values")]
    [SerializeField] int HP;

    [SerializeField] int speed;
    [SerializeField] int sprintMod;
    int maxStamina;
    [SerializeField] int regenSTimer;
    [SerializeField] int currentStamina;
    [SerializeField] float regenTickSpeed = 0.1f;
    [SerializeField] float drainTickSpeed = 1f;
    [SerializeField] int jumpMax;
    [SerializeField] int jumpSpeed;
    [SerializeField] int gravity;
    [SerializeField] float crouchSpeedMod = 0.5f;


    [Header("Gun Settings")]
    [SerializeField] GameObject muzzleFlash;
    [SerializeField] GameObject gunModel;
    [SerializeField] int shootDamage;
    [SerializeField] int shootDist;
    [SerializeField] float shootRate;
    [SerializeReference] List<GunStats> gunList = new List<GunStats>();
    [SerializeField] Animator GunAnim;
    [SerializeField] GameObject lassoPrefab;

    [Header("Audio Settings")]
    [SerializeField] AudioSource aud;
    [SerializeField] AudioClip[] audPlayerHit;
    [Range(0, 1)][SerializeField] float audPlayerHitVol;
    [SerializeField] AudioClip[] audJump;
    [Range(0, 1)][SerializeField] float audJumpVol;




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
    WaitForSeconds regenTick;
    WaitForSeconds drainTick;
    Coroutine regen;
    Coroutine comboRegen;
    bool sprinting;

    // Start is called before the first frame update
    void Start()
    {
        HPOrig = HP;
        origHeight = controller.height;
        origSpeed = speed;
        maxStamina = currentStamina;
        regenTick = new WaitForSeconds(regenTickSpeed);
        drainTick = new WaitForSeconds(drainTickSpeed);
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
        GunAnim.SetBool("Reloading", false);

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
        if (Input.GetButtonDown("Sprint") && currentStamina >= 0)
        {
            speed *= sprintMod;
            sprinting = true;
            if (regen != null)
                StopCoroutine(regen);
            StartCoroutine(StaminaLoss());
        }
        else if (Input.GetButtonUp("Sprint") || (sprinting && currentStamina == 0))
        {

            if (sprinting)
                speed /= sprintMod;
            sprinting = false;
            gameManager.Instance.StaminaBarCombo.fillAmount = (float)currentStamina / maxStamina;
            regen = StartCoroutine(StaminaRegen());
        }

    }
    IEnumerator StaminaLoss()
    {
        while (sprinting && currentStamina - 1 >= 0)
        {


            --currentStamina;
            gameManager.Instance.StaminaBar.fillAmount = (float)currentStamina / maxStamina;
            yield return drainTick;

        }
    }
    IEnumerator StaminaRegen()
    {

        yield return new WaitForSeconds(regenSTimer);
        while (currentStamina < maxStamina)
        {
            ++currentStamina;
            gameManager.Instance.StaminaBar.fillAmount = (float)currentStamina / maxStamina;
            if (gameManager.Instance.StaminaBarCombo.fillAmount <= gameManager.Instance.StaminaBar.fillAmount)
            {
                gameManager.Instance.StaminaBarCombo.fillAmount = gameManager.Instance.StaminaBar.fillAmount;
            }
            yield return regenTick;
        }
        regen = null;
    }
    IEnumerator reload()
    {
        GunAnim.SetBool("Reloading", true);
        isReloading = true;
        //  gameManager.Instance.reloadUI.SetActive(true);

        GetComponent<AudioSource>().PlayOneShot(gunList[selectedGun].reloadSound, gunList[selectedGun].reloadVol);
        yield return new WaitForSeconds(gunList[selectedGun].reloadTime - .25f);
        GunAnim.SetBool("Reloading", false);
        yield return new WaitForSeconds(.25f);
        gunList[selectedGun].ammoCurrent -= (gunList[selectedGun].magMax - gunList[selectedGun].magAmmount);
        gunList[selectedGun].magAmmount += gunList[selectedGun].magMax - gunList[selectedGun].magAmmount;
        isReloading = false;
        UpdateAmmoUi();

        // gameManager.Instance.reloadUI.SetActive(false);
    }
    IEnumerator shoot()
    {
        GunAnim.SetTrigger("Shooting");
        isShooting = true;

        GetComponent<AudioSource>().PlayOneShot(gunList[selectedGun].shootSound, gunList[selectedGun].shootVol);
        gunList[selectedGun].magAmmount--;

        StartCoroutine(FlashMuzzle());

        for (int i = 0; i < gunList[selectedGun].projAmmount; i++)
        {
            RaycastHit hit;
            if (Physics.Raycast(Camera.main.transform.position, Accuracy(), out hit, shootDist))
            {
                Debug.Log(hit.transform.position);
                IDamage dmg = hit.collider.GetComponent<IDamage>();
                if (hit.transform != transform && dmg != null)
                {
                    dmg.TakeDamage(shootDamage);

                }
                if (hit.collider.gameObject.GetComponent<MatStats>() != null && hit.transform != transform)
                {
                    Instantiate(hit.collider.gameObject.GetComponent<MatStats>().hitEffect, hit.point, Quaternion.identity);
                }
            }

        }
        UpdateAmmoUi();

        yield return new WaitForSeconds(shootRate);
        isShooting = false;
    }
    Vector3 Accuracy()
    {
        Vector3 targetPos = Camera.main.transform.position + Camera.main.transform.forward * shootDist;
        targetPos = new Vector3(
            targetPos.x + Random.Range(-gunList[selectedGun].inaccuracyDistance, gunList[selectedGun].inaccuracyDistance),
            targetPos.y + Random.Range(-gunList[selectedGun].inaccuracyDistance, gunList[selectedGun].inaccuracyDistance),
            targetPos.z + Random.Range(-gunList[selectedGun].inaccuracyDistance, gunList[selectedGun].inaccuracyDistance)
            );
        Vector3 direction = targetPos - Camera.main.transform.position;
        return direction.normalized;
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
        if (comboRegen != null)
            StopCoroutine(comboRegen);
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
        if (gameManager.Instance.playerHPBarCombo.fillAmount < gameManager.Instance.playerHPBar.fillAmount)
            gameManager.Instance.playerHPBarCombo.fillAmount = gameManager.Instance.playerHPBar.fillAmount;
        else
            comboRegen = StartCoroutine(comboHealth());
    }
    IEnumerator comboHealth()
    {

        yield return new WaitForSeconds(1);
        if (gameManager.Instance.playerHPBarCombo.fillAmount > gameManager.Instance.playerHPBar.fillAmount)
            gameManager.Instance.playerHPBarCombo.fillAmount = gameManager.Instance.playerHPBar.fillAmount;

        comboRegen = null;
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
