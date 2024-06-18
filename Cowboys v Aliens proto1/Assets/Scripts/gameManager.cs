using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class gameManager : MonoBehaviour
{
    public static gameManager Instance;
    [Header("UI Dependencies")]
    [SerializeField] GameObject menuActive;
    [SerializeField] GameObject menuPause;
    [SerializeField] GameObject menuWin;
    [SerializeField] GameObject menuLose;
    [SerializeField] public GameObject reloadUI;
    [SerializeField] TMP_Text enemyCountText;
    [SerializeField] public TMP_Text magAmmoText;
    [SerializeField] public TMP_Text reserverAmmoText;
    public Image playerHPBar;
    public Image playerHPBarCombo;
    public Image StaminaBar;
    public Image StaminaBarCombo;
    public Image reload;
    [Header("GamePlay Dependencies")]
    public GameObject playerSpawnPos;
    public GameObject playerFlashDamage;
    public GameObject attackWarning;
    public GameObject Player;
    public GameObject Enemy;
    public GameObject checkpointPopup;
    public GameObject reticleUI;
    public GameObject hpBarUI;
    public GameObject staminaUI;
    public GameObject enemyUI;
    public GameObject ammoUI;
    public playerController playerscript;
    public bool isPaused;
    int enemyCount;

    private GameObject lassoedEnemy = null;
    private bool lassoBeingThrown = false;

    // Start is called before the first frame update
    void Awake()
    {
        StartCoroutine(StartUIDelay());
        Instance = this;
        Player = GameObject.FindWithTag("Player");
        Enemy = GameObject.FindWithTag("Enemy");
        playerscript = Player.GetComponent<playerController>();
        playerSpawnPos = GameObject.FindWithTag("Player Spawn Pos");
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Cancel"))
        {
            if (menuActive == null)
            {
                statePause();
                menuActive = menuPause;
                menuActive.SetActive(isPaused);
            }
            else if (menuActive == menuPause)
            {
                stateUnPaused();
            }
        }
    }
    public void statePause()
    {
        isPaused = !isPaused;
        Time.timeScale = 0;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.Confined;
    }

    public void stateUnPaused()
    {
        isPaused = !isPaused;
        Time.timeScale = 1;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        menuActive.SetActive(isPaused);
        menuActive = null;
    }

    public void updateGameGoal(int amount)
    {
        enemyCount += amount;
        enemyCountText.text = enemyCount.ToString("F0");

        if (enemyCount <= 0)
        {
            statePause();
            menuActive = menuWin;
            menuActive.SetActive(isPaused);
        }
    }

    public void youLose()
    {
        statePause();
        menuActive = menuLose;
        menuActive.SetActive(isPaused);
    }

    public void SetLassoedEnemy(GameObject enemy)
    {
        lassoedEnemy = enemy;
    }

    public GameObject GetLassoedEnemy()
    {
        return lassoedEnemy;
    }

    public void SetLassoBeingThrown (bool beingThrown)
    {
        lassoBeingThrown = beingThrown;
    }

    public bool IsLassoBeingThrown()
    {
        return lassoBeingThrown;
    }
    public void ClearLassoedEnemy()
    {
        lassoedEnemy = null;
    }

    IEnumerator StartUIDelay()
    {
        hpBarUI.SetActive(false);
        staminaUI.SetActive(false);
        reticleUI.SetActive(false);
        enemyUI.SetActive(false);
        ammoUI.SetActive(false);
        yield return new WaitForSeconds(2);
        hpBarUI.SetActive(true);
        staminaUI.SetActive(true);
        reticleUI.SetActive(true);
        enemyUI.SetActive(true);
        ammoUI.SetActive(true);
    }
}
