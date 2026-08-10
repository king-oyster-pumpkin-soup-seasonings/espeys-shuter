using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [SerializeField] private int score;
    public int wave, killCount;

    [Header("UI References")] [SerializeField]
    private TextMeshProUGUI textPlayerHealth;

    [SerializeField] TextMeshProUGUI textMessage;
    [SerializeField] private TextMeshProUGUI textScore;
    [SerializeField] private TextMeshProUGUI textWaveLabel, textWave;
    [SerializeField] private TextMeshProUGUI textBossHPLabel, textBossHP;

    [SerializeField] float displayInterval;
    private float time;
    public bool isMessageDone;
    public bool isSelectingWeapon;
    public bool waveIsOngoing;

    [SerializeField] private SpriteRenderer shieldHUDSpriteRenderer;
    [SerializeField] TextMeshProUGUI textShieldDuration;
    [SerializeField] private TextMeshPro textShieldDurationAroundPlayer;

    public static Action OnWaveStart;
    public static Action OnWaveComplete;

    // SETUPS
    private void OnEnable()
    {
        EnemyBoss.EnemyBossDied += DeclareGameComplete;
        Health.OnHealthChangeAlt += UpdateBossHealthText;
    }

    private void OnDisable()
    {
        EnemyBoss.EnemyBossDied -= DeclareGameComplete;
        Health.OnHealthChangeAlt -= UpdateBossHealthText;
    }

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        InitializeValues();
        // StartCoroutine(StartGameIntroMessageRoutine());
        UpdateShieldHUD(0);
        UpdateWaveText(wave);
        wave = 6; // debug test A
        HandleWeaponSelection(); // debug test A
        // StartWave(); //test. should be deleted or commented: comment corutine startgameintromessageroutine first!
    }

    // DEBUG  --------------------
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Y) && isSelectingWeapon == false && waveIsOngoing)
        {
            TriggerWaveComplete();
        }
    } // DEBUG -------------------

    public void InitializeValues()
    {
        score = 0;
        wave = 1;
        isMessageDone = false;
        isSelectingWeapon = false;
        killCount = 0;
        textMessage.text = "";
        textShieldDuration.text = "";
        textBossHPLabel.enabled = false;
        textBossHP.enabled = false;
    }


    // GAME CONTROLS
    void HandleWeaponSelection()
    {
        isSelectingWeapon = true;
        textMessage.text = "Choose a Weapon:";
        WeaponSpawner.Instance.TriggerWeaponChooser();
    }

    public void TriggerWeaponSelectionComplete()
    {
        if (isSelectingWeapon == false)
        {
            textMessage.text = "";
            StartCoroutine(IntermissionBeforeStartinWave());
        }
    }

    public void TriggerWaveComplete()
    {
        waveIsOngoing = false;
        score += 1000;
        wave++;
        OnWaveComplete?.Invoke();
        if (wave <= 5) StartCoroutine(IntermissionBeforeChoosingWeapon());
        else StartWave();
    }

    public void StartWave()
    {
        killCount = 0;
        UpdateWaveText(wave);
        if (wave != 6) ShowTextMessage($"Wave {wave}", displayInterval);
        else if (wave == 6) ShowTextMessage("Final Boss!", displayInterval);
        waveIsOngoing = true;

        OnWaveStart?.Invoke();

        if (wave == 6)
        {
            textWaveLabel.enabled = false;
            textWave.enabled = false;
            StartCoroutine(RoutineDelayBeforeDisplayingBossHP());
        }
    }


    // HUD CONTROLS
    public void AddScore(int amount = 1)
    {
        if (amount <= 0) amount = 1;
        score += amount;

        if (textScore != null) textScore.text = score.ToString();
    }

    public void UpdateWaveText(int wave)
    {
        textWave.text = wave.ToString();
    }

    public void UpdatePlayerHealthText(int playerHealth)
    {
        string iconifiedPlayerHealth = "";
        for (int i = 0; i < playerHealth; i++)
        {
            iconifiedPlayerHealth += "<> ";
        }

        if (textPlayerHealth != null) textPlayerHealth.text = iconifiedPlayerHealth;
    }

    public void UpdateShieldHUD(float shieldDuration)
    {
        // shieldHUDSpriteRenderer.enabled = true; // debug
        if (shieldDuration <= 0)
        {
            shieldHUDSpriteRenderer.enabled = false;
            textShieldDuration.text = "";
            textShieldDurationAroundPlayer.text = "";
        }
        else
        {
            shieldHUDSpriteRenderer.enabled = true;
            textShieldDuration.text = Mathf.CeilToInt(shieldDuration).ToString();
            textShieldDurationAroundPlayer.text = Mathf.CeilToInt(shieldDuration).ToString();
        }
    }

    public void UpdateBossHealthText(int bossHP)
    {
        Debug.Log("I GOT THE BOSS BROADCAST METHOD, updating textboss HP");
        if (textBossHP != null)
        {
            if (bossHP > 0) textBossHP.text = bossHP.ToString();
            else textBossHP.text = "0";
        }
    }


    // MESSAGE CONTROLS
    public void ShowTextMessage(string message, float duration = 2f, float delay = 0f)
    {
        StartCoroutine(DisplayMessageRoutine(message, duration, delay));
    }

    private IEnumerator DisplayMessageRoutine(string message, float duration, float delay)
    {
        isMessageDone = false;
        if (delay > 0) yield return new WaitForSeconds(delay);

        if (textMessage != null) textMessage.text = message;
        yield return new WaitForSeconds(duration);
        if (textMessage != null) textMessage.text = "";
        isMessageDone = true;
    }


    // GAME STATES
    public void DeclareGameOver()
    {
        StartCoroutine(GameOverRoutine());
    }

    private IEnumerator GameOverRoutine()
    {
        yield return new WaitForSeconds(2.25f);
        SceneManager.LoadScene("GameOverScene");
    }

    public void DeclareGameComplete()
    {
        waveIsOngoing = false;
        StartCoroutine(GameCompleteRoutine());
    }


    // COROUTINES
    private IEnumerator GameCompleteRoutine()
    {
        yield return new WaitForSeconds(3f);
        SceneManager.LoadScene("LevelCompleteScene");
    }

    private IEnumerator StartGameIntroMessageRoutine()
    {
        isMessageDone = false;

        if (textMessage != null) textMessage.text = "";
        yield return new WaitForSeconds(1.5f);

        if (textMessage != null) textMessage.text = "Survive and defeat the final boss!";
        yield return new WaitForSeconds(displayInterval + 0.5f);

        if (textMessage != null) textMessage.text = "";
        yield return new WaitForSeconds(1.25f);

        StartWave();
    }

    private IEnumerator IntermissionBeforeStartinWave()
    {
        yield return new WaitForSeconds(1.75f);
        StartWave();
    }

    private IEnumerator IntermissionBeforeChoosingWeapon()
    {
        yield return new WaitForSeconds(1.75f);
        HandleWeaponSelection();
    }

    private IEnumerator RoutineDelayBeforeDisplayingBossHP()
    {
        yield return new WaitForSeconds(4f);
        textBossHPLabel.enabled = true;
        textBossHP.enabled = true;
    }
}