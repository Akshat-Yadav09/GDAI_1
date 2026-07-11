using UnityEngine;

/// <summary>
/// Shared data constants and helper methods for Shop integration.
/// Both the Gameplay scene and the Shop scene should use these keys to stay in sync.
/// </summary>
public static class ShopData
{
    // Currency
    public const string CoinsKey = "Coins";

    // Skins
    // Expected values: "Default", "Green", "Purple", "Cyan", "Rainbow"
    public const string EquippedSkinKey = "EquippedSkin";

    // Single-use Power-ups (0 = inactive, 1 = active)
    public const string PowerUpReviveKey = "PowerUp_Revive";
    public const string PowerUpScoreMultiplierKey = "PowerUp_ScoreMultiplier";
    public const string PowerUpDoubleJumpKey = "PowerUp_DoubleJump";

    // Pets
    // Expected values: "None", "CubePet"
    public const string EquippedPetKey = "EquippedPet";

    // --- Helper Methods ---

    public static int GetCoins() => PlayerPrefs.GetInt(CoinsKey, 0);
    public static void AddCoins(int amount)
    {
        int current = GetCoins();
        PlayerPrefs.SetInt(CoinsKey, current + amount);
        PlayerPrefs.Save();
    }

    public static string GetEquippedSkin() => PlayerPrefs.GetString(EquippedSkinKey, "Default");
    public static string GetEquippedPet() => PlayerPrefs.GetString(EquippedPetKey, "None");

    public static bool ConsumePowerUp(string key)
    {
        bool isActive = PlayerPrefs.GetInt(key, 0) == 1;
        if (isActive)
        {
            // Clear it so it's only used for one game
            PlayerPrefs.SetInt(key, 0);
            PlayerPrefs.Save();
        }
        return isActive;
    }
}
