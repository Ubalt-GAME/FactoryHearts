using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class FactoryGameManager : MonoBehaviour
{
    public static FactoryGameManager Instance { get; private set; }

    [Header("Timer")]
    public float timeLimitSeconds = 120f;   // total length of one run (in seconds)
    public TMP_Text timerText;              // HUD: shows remaining time

    [Header("Score")]
    public int repairScore = 0;             // total number of successful repairs
    public TMP_Text scoreText;              // HUD: shows "Repairs: X"

    [Header("End Screen UI")]
    public GameObject endPanel;             // panel that appears when time is up
    public TMP_Text endMessageText;         // e.g. "SHIFT COMPLETE"
    public TMP_Text endScoreText;           // e.g. "Total repairs: 12"

    private float elapsedTime = 0f;
    private bool gameEnded = false;

    void Awake()
    {
        // Simple singleton pattern
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        // Make sure the game runs normally when the scene starts
        Time.timeScale = 1f;

        if (endPanel != null)
            endPanel.SetActive(false);
    }

    void Update()
    {
        // Handle hotkeys every frame
        HandleHotkeys();

        // Don't advance timer or update game after it has ended
        if (gameEnded)
            return;

        // Timer logic
        elapsedTime += Time.deltaTime;

        float remaining = Mathf.Max(0f, timeLimitSeconds - elapsedTime);
        UpdateTimerUI(remaining);

        // End the run when time runs out
        if (remaining <= 0f)
        {
            EndGame();
        }
    }

    void HandleHotkeys()
    {
        // ESC = quit the game
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            QuitGame();
        }

        // F = toggle fullscreen
        if (Input.GetKeyDown(KeyCode.F))
        {
            ToggleFullscreen();
        }
    }

    // Called by RepairTarget2D when a machine goes from damaged -> fixed
    public void RegisterRepair(RepairTarget2D machine)
    {
        if (gameEnded)
            return;

        repairScore++;
        UpdateScoreUI();

        Debug.Log("Repair registered. Total repairs = " + repairScore);
    }

    void UpdateTimerUI(float remaining)
    {
        if (timerText != null)
        {
            // Shows time as "Time: 87.3s"
            timerText.text = $"Time: {remaining:0.0}s";
        }
    }

    void UpdateScoreUI()
    {
        if (scoreText != null)
        {
            // Shows score as "Repairs: 5"
            scoreText.text = $"Repairs: {repairScore}";
        }
    }

    void EndGame()
    {
        gameEnded = true;
        Time.timeScale = 0f;   // pause the simulation

        if (endPanel != null)
            endPanel.SetActive(true);

        if (endMessageText != null)
            endMessageText.text = "SHIFT COMPLETE";

        if (endScoreText != null)
            endScoreText.text = $"Total repairs: {repairScore}";
    }

    // Restart the current level (called by Restart button)
    public void RestartLevel()
    {
        Time.timeScale = 1f;
        Scene current = SceneManager.GetActiveScene();
        SceneManager.LoadScene(current.buildIndex);
    }

    // Quit the game (ESC key)
    public void QuitGame()
    {
        Debug.Log("Quitting game...");

#if UNITY_EDITOR
        // In the editor: stop play mode
        UnityEditor.EditorApplication.isPlaying = false;
#else
        // In a built game: close the application
        Application.Quit();
#endif
    }

    // Toggle fullscreen (F11 or UI button)
    public void ToggleFullscreen()
    {
        Screen.fullScreen = !Screen.fullScreen;
    }
}