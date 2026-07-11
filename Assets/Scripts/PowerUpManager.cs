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

    // Track if revive was already used during this run
    public bool ReviveUsed { get; set; } = false;

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
}
