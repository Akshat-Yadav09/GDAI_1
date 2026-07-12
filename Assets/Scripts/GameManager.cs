using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class GameManager : MonoBehaviour
{
    [Header("In-Game UI")]
    public TMP_Text scoreText;
    
    [Header("Game Over UI")]
    public GameObject gameOverMenu;
    [Tooltip("Shows the score from this run.")]
    public TMP_Text gameOverScoreText;

    [Header("Pause UI")]
    public GameObject pauseMenu;
    public TMP_Text pauseScoreText;
    [Tooltip("The actual pause button on screen so we can hide it on death.")]
    public GameObject pauseButtonUI;

    
    public static GameManager Instance { get; private set; }

    [Header("Settings")]
    [Tooltip("Points gained per second")]
    public float scoreRate = 10f;
    
    public float Score => score;
    public bool IsGameOver => isGameOver;

    private float score = 0f;
    private int displayedScore = -1;
    private bool isGameOver = false;
    private bool isPaused = false;
    private int savedHighScore = 0;

    private const string HighScorePrefsKey = "HighScore";

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    void Start()
    {
        Time.timeScale = 1f; 
        gameOverMenu.SetActive(false);
        if (pauseMenu != null) pauseMenu.SetActive(false);

        if (scoreText != null) 
            scoreText.gameObject.SetActive(true);

        // Load the saved high score once at start
        savedHighScore = PlayerPrefs.GetInt(HighScorePrefsKey, 0);

        // Check for score multiplier power-up
        if (PowerUpManager.Instance != null && PowerUpManager.Instance.HasScoreMultiplier)
        {
            scoreRate *= 2f;
        }

        // Start the music!
        if (SoundManager.Instance != null)
        {
            SoundManager.Instance.PlayGameplayMusic();
        }
    }

    void Update()
    {
        if (!isGameOver && !isPaused)
        {
            score += Time.deltaTime * scoreRate; 

            if (DifficultyManager.Instance != null)
                DifficultyManager.Instance.UpdateDifficulty(score);

            // Only update UI text when integer value changes
            int currentScore = Mathf.FloorToInt(score);
            if (currentScore != displayedScore)
            {
                displayedScore = currentScore;

                if (scoreText != null)
                {
                    if (currentScore > savedHighScore)
                    {
                        // Beating the high score live!
                        scoreText.text = "NEW BEST: " + currentScore.ToString();
                        scoreText.color = Color.yellow;
                    }
                    else
                    {
                        scoreText.text = "Score: " + currentScore.ToString();
                        scoreText.color = Color.white;
                    }
                }
            }
        }
    }

    public void TriggerGameOver()
    {
        if (isGameOver) return; // Prevent double-triggering
        
        isGameOver = true;
        
        // Stop music & play death sound
        if (SoundManager.Instance != null)
            SoundManager.Instance.PlayDeathSound();

        // Save earned coins
        if (CoinManager.Instance != null)
            CoinManager.Instance.SaveCoins();

        // Hide in-game UI
        if (scoreText != null)
            scoreText.gameObject.SetActive(false);
            
        if (pauseButtonUI != null)
            pauseButtonUI.SetActive(false);

        StartCoroutine(GameOverRoutine());
    }

    private System.Collections.IEnumerator GameOverRoutine()
    {
        // Wait for the death explosion, camera shake, and particles to finish naturally!
        yield return new WaitForSeconds(0.8f);

        int currentScore = Mathf.FloorToInt(score);

        // Check and update high score
        int savedHighScore = PlayerPrefs.GetInt(HighScorePrefsKey, 0);
        bool isNewHighScore = currentScore > savedHighScore;

        if (isNewHighScore)
        {
            savedHighScore = currentScore;
            PlayerPrefs.SetInt(HighScorePrefsKey, savedHighScore);
            PlayerPrefs.Save();
        }

        // Show this run's score
        if (gameOverScoreText != null)
            gameOverScoreText.text = isNewHighScore
                ? "★ NEW HIGH SCORE: " + currentScore.ToString() + " ★"
                : "Score: " + currentScore.ToString();

        gameOverMenu.SetActive(true);
        Time.timeScale = 0f; // Freeze game AFTER the animation
    }

    public void RestartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    // ==========================================
    // PAUSE LOGIC
    // ==========================================

    public void TogglePause()
    {
        if (isGameOver) return;

        isPaused = !isPaused;
        if (pauseMenu != null) 
        {
            pauseMenu.SetActive(isPaused);
            if (isPaused && pauseScoreText != null)
            {
                pauseScoreText.text = "Score: " + Mathf.FloorToInt(score).ToString();
            }
        }
        Time.timeScale = isPaused ? 0f : 1f;

        // Pause or resume music
        if (SoundManager.Instance != null)
        {
            if (isPaused) SoundManager.Instance.PauseMusic();
            else SoundManager.Instance.ResumeMusic();
        }
    }

    public void ResumeGame()
    {
        if (isGameOver) return;
        
        isPaused = false;
        if (pauseMenu != null) pauseMenu.SetActive(false);
        Time.timeScale = 1f;

        if (SoundManager.Instance != null) SoundManager.Instance.ResumeMusic();
    }

    // ==========================================
    // SCENE NAVIGATION
    // ==========================================

    public void GoToMainMenu()
    {
        Time.timeScale = 1f; // Always unfreeze time before loading scenes!
        if (SoundManager.Instance != null) SoundManager.Instance.StopMusic();
        SceneManager.LoadScene("MainMenu");
    }

    public void GoToShop()
    {
        Time.timeScale = 1f;
        if (SoundManager.Instance != null) SoundManager.Instance.StopMusic();
        SceneManager.LoadScene("Store");
    }
}
