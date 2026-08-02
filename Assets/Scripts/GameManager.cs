using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    // --- variables ---
    public static GameManager Instance { get; private set; }

    [SerializeField] private int score;

    [Header("UI References")] [SerializeField]
    private TextMeshProUGUI textPlayerHealth;

    [SerializeField] private TextMeshProUGUI textScore;

    // --- methods ---
    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void InitializeValues()
    {
        // data
        score = 0;
    }

    public void AddScore(int amount = 1)
    {
        // data
        if (amount <= 0) amount = 1;
        score += amount;

        // ui
        if (textScore != null) textScore.text = score.ToString();
    }

    public void UpdatePlayerHealthText(int playerHealth)
    {
        // data
        string iconifiedPlayerHealth = "";
        for (int i = 0; i < playerHealth; i++)
        {
            iconifiedPlayerHealth += "<> ";
        }

        // ui
        if (textPlayerHealth != null) textPlayerHealth.text = iconifiedPlayerHealth;

        // data
        if (playerHealth <= 0) DeclareGameOver();
    }

    public void DeclareGameOver()
    {
        SceneManager.LoadScene("GameOverScene");
    }

    public void DeclareGameComplete()
    {
        SceneManager.LoadScene("LevelCompleteScene");
    }
}
