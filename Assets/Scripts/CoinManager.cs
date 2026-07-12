using UnityEngine;
using TMPro;

/// <summary>
/// Tracks coins earned during the current run.
/// </summary>
public class CoinManager : MonoBehaviour
{
    public static CoinManager Instance { get; private set; }

    [Header("UI")]
    [Tooltip("Optional: Text to display coins earned in this run")]
    public TMP_Text runCoinsText;

    [Header("Settings")]
    [Tooltip("Every X score points = 1 coin. Lower = more generous.")]
    public int scorePerCoin = 3;
    [Tooltip("Coins earned per near miss dodge.")]
    public int coinsPerNearMiss = 15;

    private int coinsFromNearMisses = 0;
    private int currentRunTotal = 0;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    /// <summary>
    /// Call this when a near miss happens.
    /// </summary>
    public void AddNearMissBonus()
    {
        coinsFromNearMisses += coinsPerNearMiss;
        UpdateUI(0); // Will update based on 0 score for now, but gets fixed in Update()
    }

    void Update()
    {
        if (GameManager.Instance != null && !GameManager.Instance.IsGameOver)
        {
            UpdateUI(GameManager.Instance.Score);
        }
    }

    private void UpdateUI(float currentScore)
    {
        currentRunTotal = Mathf.FloorToInt(currentScore / scorePerCoin) + coinsFromNearMisses;
        if (runCoinsText != null)
        {
            runCoinsText.text = "Coins: " + currentRunTotal;
        }
    }

    /// <summary>
    /// Called when the player dies to actually add the coins to their bank.
    /// </summary>
    public void SaveCoins()
    {
        if (currentRunTotal > 0)
        {
            ShopData.AddCoins(currentRunTotal);
        }
    }
}
