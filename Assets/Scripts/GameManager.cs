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

    
    [Header("Settings")]
    [Tooltip("Points gained per second")]
    public float scoreRate = 10f;
    
    private float score = 0f;
    private int displayedScore = -1;
    private bool isGameOver = false;
    private int savedHighScore = 0;

    private const string HighScorePrefsKey = "HighScore";

    void Start()
    {
        Time.timeScale = 1f; 
        gameOverMenu.SetActive(false);

        if (scoreText != null) 
            scoreText.gameObject.SetActive(true);

        // Load the saved high score once at start
        savedHighScore = PlayerPrefs.GetInt(HighScorePrefsKey, 0);
    }

    void Update()
    {
        if (!isGameOver)
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

        int currentScore = Mathf.FloorToInt(score);

        // Hide in-game score counter
        if (scoreText != null)
            scoreText.gameObject.SetActive(false);

        // Check and update high score
        int savedHighScore = PlayerPrefs.GetInt(HighScorePrefsKey, 0);
        bool isNewHighScore = currentScore > savedHighScore;

        if (isNewHighScore)
        {
            savedHighScore = currentScore;
            PlayerPrefs.SetInt(HighScorePrefsKey, savedHighScore);
            PlayerPrefs.Save();
        }

        // Show this run's score — highlight if it's a new high score!
        if (gameOverScoreText != null)
            gameOverScoreText.text = isNewHighScore
                ? "★ NEW HIGH SCORE: " + currentScore.ToString() + " ★"
                : "Score: " + currentScore.ToString();


        gameOverMenu.SetActive(true);
        Time.timeScale = 0f; // Freeze game
    }

    public void RestartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
