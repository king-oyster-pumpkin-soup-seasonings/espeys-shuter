using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [SerializeField] private int score, wave;

    [Header("UI References")] [SerializeField]
    private TextMeshProUGUI textPlayerHealth;

    [SerializeField] TextMeshProUGUI textMessage;
    [SerializeField] private TextMeshProUGUI textScore;
    [SerializeField] private TextMeshProUGUI textWave;

    [SerializeField] float displayInterval;
    private float time;
    public bool isMessageDone;


    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        InitializeValues();
        StartCoroutine(StartGameIntroMessageRoutine());
        UpdateWaveText(wave);
    }

    public void InitializeValues()
    {
        score = 0;
        wave = 1;
        isMessageDone = false;
        textMessage.text = "";
    }

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

    private IEnumerator StartGameIntroMessageRoutine()
    {
        isMessageDone = false;

        if (textMessage != null) textMessage.text = "";
        yield return new WaitForSeconds(1.5f);

        if (textMessage != null) textMessage.text = "Survive and defeat the final boss!";
        yield return new WaitForSeconds(displayInterval + 0.5f);

        if (textMessage != null) textMessage.text = "";
        yield return new WaitForSeconds(1.25f);

        if (textMessage != null) textMessage.text = $"WAVE {wave}";
        yield return new WaitForSeconds(displayInterval);

        if (textMessage != null) textMessage.text = "";
        isMessageDone = true;
    }

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

    public void DeclareGameOver()
    {
        StartCoroutine(GameOverRoutine());
    }

    private IEnumerator GameOverRoutine()
    {
        yield return new WaitForSeconds(0.3f);
        SceneManager.LoadScene("GameOverScene");
    }

    public void DeclareGameComplete()
    {
        SceneManager.LoadScene("LevelCompleteScene");
    }
}