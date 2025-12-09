using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class FactoryGameManager : MonoBehaviour
{
    public static FactoryGameManager Instance { get; private set; }

    [Header("Timer")]
    public float timeLimitSeconds = 120f;   // total length of one run
    public TMP_Text timerText;

    [Header("Score")]
    public int repairScore = 0;
    public TMP_Text scoreText;             // in-game HUD

    [Header("End Screen UI")]
    public GameObject endPanel;            // panel with final stats + restart button
    public TMP_Text endMessageText;        // e.g. "Shift Complete!"
    public TMP_Text endScoreText;          // e.g. "Total repairs: 12"

    private float elapsedTime = 0f;
    private bool gameEnded = false;

    void Awake()
    {
        // Simple singleton
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        // Make sure time scale is normal when scene starts
        Time.timeScale = 1f;

        if (endPanel != null)
            endPanel.SetActive(false);
    }

    void Update()
    {
        if (gameEnded) return;

        elapsedTime += Time.deltaTime;

        float remaining = Mathf.Max(0f, timeLimitSeconds - elapsedTime);
        UpdateTimerUI(remaining);

        if (remaining <= 0f)
        {
            EndGame();
        }
    }

    // Called by RepairTarget2D when a machine goes from damaged -> fixed
    public void RegisterRepair(RepairTarget2D machine)
    {
        repairScore++;
        UpdateScoreUI();

        Debug.Log("Repair registered. Total repairs = " + repairScore);
    }

    void UpdateTimerUI(float remaining)
    {
        if (timerText != null)
        {
            // e.g. "Time: 87.3s"
            timerText.text = $"Time: {remaining:0.0}s";
        }
    }

    void UpdateScoreUI()
    {
        if (scoreText != null)
        {
            scoreText.text = $"Repairs: {repairScore}";
        }
    }

    void EndGame()
    {
        gameEnded = true;
        Time.timeScale = 0f;   // pause the whole game

        if (endPanel != null)
            endPanel.SetActive(true);

        if (endMessageText != null)
            endMessageText.text = "SHIFT COMPLETE";

        if (endScoreText != null)
            endScoreText.text = $"Total repairs: {repairScore}";
    }

    // Called by the Restart button on the end panel
    public void RestartLevel()
    {
        Time.timeScale = 1f;
        Scene current = SceneManager.GetActiveScene();
        SceneManager.LoadScene(current.buildIndex);
    }
}