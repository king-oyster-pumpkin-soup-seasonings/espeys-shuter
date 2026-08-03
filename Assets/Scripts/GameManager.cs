using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [SerializeField] private int score;

    [Header("UI References")] [SerializeField]
    private TextMeshProUGUI textPlayerHealth;

    [SerializeField] TextMeshProUGUI textMessage;

    [SerializeField] private TextMeshProUGUI textScore;

    [SerializeField] float displayInterval;
    private float time;


    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        textMessage.text = "";
        ShowObjectives();
    }

    public void InitializeValues()
    {
        score = 0;
    }

    public void AddScore(int amount = 1)
    {
        if (amount <= 0) amount = 1;
        score += amount;

        if (textScore != null) textScore.text = score.ToString();
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

    public void ShowObjectives()
    {
        textMessage.text = "Survive and defeat the final boss!";
        StartCoroutine(StartTextInterval(displayInterval));
    }


    IEnumerator StartTextInterval(float paramDisplayInterval = 3f)
    {
        yield return new WaitForSeconds(paramDisplayInterval);
        textMessage.text = "";
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