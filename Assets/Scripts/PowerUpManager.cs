using UnityEngine;

/// <summary>
/// Loads power-ups at the start of a run and instantly consumes them.
/// Exposes booleans so the rest of the game knows what's active.
/// </summary>
public class PowerUpManager : MonoBehaviour
{
    public static PowerUpManager Instance { get; private set; }

    public bool HasRevive { get; private set; }
    public bool HasScoreMultiplier { get; private set; }
    public bool HasDoubleJump { get; private set; }

    [Header("UI Indicators")]
    [Tooltip("Drag your Heart icon here")]
    public GameObject reviveIcon;
    [Tooltip("Drag your Feather icon here")]
    public GameObject doubleJumpIcon;
    [Tooltip("Drag your 2X Text here")]
    public GameObject scoreMultiplierIcon;

    // Track if revive was already used during this run
    public bool ReviveUsed { get; private set; } = false;

    void Awake()
    {
        //PlayerPrefs.SetInt("PowerUp_DoubleJump", 1);
        //PlayerPrefs.SetString("EquippedSkin", "Rainbow");
        //PlayerPrefs.SetString("EquippedPet", "CubePet");

        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        // Load and instantly consume them so they are only used for THIS run
        HasRevive = ShopData.ConsumePowerUp(ShopData.PowerUpReviveKey);
        HasScoreMultiplier = ShopData.ConsumePowerUp(ShopData.PowerUpScoreMultiplierKey);
        HasDoubleJump = ShopData.ConsumePowerUp(ShopData.PowerUpDoubleJumpKey);
    }

    void Start()
    {
        // Show or hide the icons depending on if the player bought them for this run
        if (reviveIcon != null) reviveIcon.SetActive(HasRevive);
        if (doubleJumpIcon != null) doubleJumpIcon.SetActive(HasDoubleJump);
        if (scoreMultiplierIcon != null) scoreMultiplierIcon.SetActive(HasScoreMultiplier);
    }

    public void UseRevive()
    {
        ReviveUsed = true;
        // Hide the heart icon so the player knows their revive is gone!
        if (reviveIcon != null) reviveIcon.SetActive(false);
    }
}
