using UnityEngine;

/// <summary>
/// Attaches to the Player. Sets the player's color based on the equipped skin in ShopData.
/// </summary>
public class SkinManager : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    private string equippedSkin;

    void Start()
    {
        // Get the child visual or the root sprite renderer
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        equippedSkin = ShopData.GetEquippedSkin(); // Cache once instead of reading PlayerPrefs every frame
        if (spriteRenderer != null)
        {
            ApplySkin(equippedSkin);
        }
    }

    void Update()
    {
        if (spriteRenderer != null && equippedSkin == "Rainbow")
        {
            // Cycle hue over time (1 full cycle every 2 seconds)
            float hue = Mathf.Repeat(Time.time * 0.5f, 1f);
            spriteRenderer.color = Color.HSVToRGB(hue, 1f, 1f);
        }
    }

    private void ApplySkin(string skinName)
    {
        switch (skinName)
        {
            case "Green":
                spriteRenderer.color = Color.green;
                break;
            case "Cyan":
                spriteRenderer.color = Color.cyan;
                break;
            case "Rainbow":
                // Handled in Update()
                break;
            case "Default":
            default:
                spriteRenderer.color = Color.red; // User requested Default = Red
                break;
        }
    }
}
