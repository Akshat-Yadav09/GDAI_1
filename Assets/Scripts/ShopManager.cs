using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class ShopManager : MonoBehaviour
{
    [Header("UI References")]
    public TMP_Text coinText; 
    
    [Header("Skin Button Texts")]
    public TMP_Text defaultSkinText;
    public TMP_Text greenSkinText;
    public TMP_Text cyanSkinText;
    public TMP_Text rainbowSkinText;

    [Header("Power-Up Button Texts")]
    public TMP_Text doubleJumpText;
    public TMP_Text reviveText;
    public TMP_Text scoreMultiplierText;

    [Header("Pet Button Texts")]
    public TMP_Text petButtonText; 

    [Header("Effects")]
    [Tooltip("Drag your Coin Sprite here to spawn particles on purchase")]
    public Sprite coinSprite; 

    [Header("Testing")]
    [Tooltip("Check this in the Inspector to get 99,999 coins for testing")]
    public bool giveInfiniteMoney = false;

    void Start()
    {
        if (giveInfiniteMoney)
        {
            PlayerPrefs.SetInt(ShopData.CoinsKey, 99999);
            PlayerPrefs.Save();
        }
        
        UpdateCoinUI();
        UpdateAllButtonUI();
    }

    private void UpdateCoinUI()
    {
        if (coinText != null)
        {
            coinText.text = ShopData.GetCoins().ToString();
        }
    }

    public void GoToMainMenu()
    {
        // Assuming your main menu is Scene index 0 or named "MainMenu"
        // Update this name if your main menu scene is called something else!
        SceneManager.LoadScene("MainMenu");
    }

    // ==========================================
    // SKINS
    // ==========================================
    
    private void BuyOrEquipSkin(string skinName, int cost)
    {
        string ownedKey = "SkinOwned_" + skinName;
        
        // Default is always owned, others are owned if the PlayerPref is 1
        bool isOwned = skinName == "Default" || PlayerPrefs.GetInt(ownedKey, 0) == 1;

        if (isOwned)
        {
            PlayerPrefs.SetString(ShopData.EquippedSkinKey, skinName);
            PlayerPrefs.Save();
            if (SoundManager.Instance != null) SoundManager.Instance.PlayEquipSound();
            Debug.Log("Equipped " + skinName + " Skin!");
        }
        else
        {
            if (ShopData.GetCoins() >= cost)
            {
                ShopData.AddCoins(-cost);
                PlayerPrefs.SetInt(ownedKey, 1); // Mark as owned permanently
                PlayerPrefs.SetString(ShopData.EquippedSkinKey, skinName); // Auto-equip
                PlayerPrefs.Save();
                if (SoundManager.Instance != null) SoundManager.Instance.PlayPurchaseSound();
                UpdateCoinUI();
                UpdateAllButtonUI();
                SpawnPurchaseCoins(8); // Spawn 8 coins!
                Debug.Log("Bought & Equipped " + skinName + " Skin!");
            }
            else
            {
                Debug.Log("Not enough coins for " + skinName + " Skin!");
            }
        }
        UpdateAllButtonUI();
    }

    public void EquipDefaultSkin() { BuyOrEquipSkin("Default", 0); } // Default is Red
    public void EquipGreenSkin()   { BuyOrEquipSkin("Green", 500); }
    public void EquipCyanSkin()    { BuyOrEquipSkin("Cyan", 1000); }
    public void EquipRainbowSkin() { BuyOrEquipSkin("Rainbow", 5000); }


    // ==========================================
    // POWER-UPS (Link these to your PowerUp Buttons)
    // ==========================================
    public void BuyDoubleJump()
    {
        if (PlayerPrefs.GetInt(ShopData.PowerUpDoubleJumpKey, 0) == 1) return; // Already active!

        int cost = 2000;
        if (ShopData.GetCoins() >= cost)
        {
            ShopData.AddCoins(-cost);
            PlayerPrefs.SetInt(ShopData.PowerUpDoubleJumpKey, 1);
            PlayerPrefs.Save();
            if (SoundManager.Instance != null) SoundManager.Instance.PlayPurchaseSound();
            UpdateCoinUI();
            UpdateAllButtonUI();
            SpawnPurchaseCoins(5);
            Debug.Log("Bought Double Jump!");
        }
    }

    public void BuyRevive()
    {
        if (PlayerPrefs.GetInt(ShopData.PowerUpReviveKey, 0) == 1) return; // Already active!

        int cost = 1000;
        if (ShopData.GetCoins() >= cost)
        {
            ShopData.AddCoins(-cost);
            PlayerPrefs.SetInt(ShopData.PowerUpReviveKey, 1);
            PlayerPrefs.Save();
            if (SoundManager.Instance != null) SoundManager.Instance.PlayPurchaseSound();
            UpdateCoinUI();
            UpdateAllButtonUI();
            SpawnPurchaseCoins(5);
            Debug.Log("Bought Revive!");
        }
    }

    public void BuyScoreMultiplier()
    {
        if (PlayerPrefs.GetInt(ShopData.PowerUpScoreMultiplierKey, 0) == 1) return; // Already active!

        int cost = 1500;
        if (ShopData.GetCoins() >= cost)
        {
            ShopData.AddCoins(-cost);
            PlayerPrefs.SetInt(ShopData.PowerUpScoreMultiplierKey, 1);
            PlayerPrefs.Save();
            if (SoundManager.Instance != null) SoundManager.Instance.PlayPurchaseSound();
            UpdateCoinUI();
            UpdateAllButtonUI();
            SpawnPurchaseCoins(5);
            Debug.Log("Bought Score Multiplier!");
        }
    }


    // ==========================================
    // PETS
    // ==========================================
    
    public void ToggleCubePet()
    {
        string petName = "CubePet";
        int cost = 5000; // Updated to match your UI!
        string ownedKey = "PetOwned_" + petName;
        bool isOwned = PlayerPrefs.GetInt(ownedKey, 0) == 1;

        if (!isOwned)
        {
            // 1. BUY & EQUIP
            if (ShopData.GetCoins() >= cost)
            {
                ShopData.AddCoins(-cost);
                PlayerPrefs.SetInt(ownedKey, 1);
                PlayerPrefs.SetString(ShopData.EquippedPetKey, petName);
                PlayerPrefs.Save();
                if (SoundManager.Instance != null) SoundManager.Instance.PlayPurchaseSound();
                UpdateCoinUI();
                SpawnPurchaseCoins(10);
                Debug.Log("Bought & Equipped " + petName + "!");
            }
            else
            {
                Debug.Log("Not enough coins for " + petName + "!");
            }
        }
        else
        {
            // 2. TOGGLE EQUIP / UNEQUIP
            if (ShopData.GetEquippedPet() == petName)
            {
                // Currently equipped -> Unequip
                PlayerPrefs.SetString(ShopData.EquippedPetKey, "None");
                PlayerPrefs.Save();
                if (SoundManager.Instance != null) SoundManager.Instance.PlayEquipSound();
                Debug.Log("Unequipped " + petName + "!");
            }
            else
            {
                // Currently unequipped -> Equip
                PlayerPrefs.SetString(ShopData.EquippedPetKey, petName);
                PlayerPrefs.Save();
                if (SoundManager.Instance != null) SoundManager.Instance.PlayEquipSound();
                Debug.Log("Equipped " + petName + "!");
            }
        }
        
        UpdateAllButtonUI();
    }

    // ==========================================
    // UI UPDATES
    // ==========================================

    private void UpdateAllButtonUI()
    {
        UpdateSkinButtonUI("Default", defaultSkinText);
        UpdateSkinButtonUI("Green", greenSkinText);
        UpdateSkinButtonUI("Cyan", cyanSkinText);
        UpdateSkinButtonUI("Rainbow", rainbowSkinText);

        UpdatePowerUpButtonUI(ShopData.PowerUpDoubleJumpKey, doubleJumpText);
        UpdatePowerUpButtonUI(ShopData.PowerUpReviveKey, reviveText);
        UpdatePowerUpButtonUI(ShopData.PowerUpScoreMultiplierKey, scoreMultiplierText);

        UpdatePetButtonUI();
    }

    private void UpdateSkinButtonUI(string skinName, TMP_Text btnText)
    {
        if (btnText == null) return;

        string ownedKey = "SkinOwned_" + skinName;
        bool isOwned = skinName == "Default" || PlayerPrefs.GetInt(ownedKey, 0) == 1;

        if (!isOwned)
        {
            btnText.text = "Buy";
        }
        else if (ShopData.GetEquippedSkin() == skinName)
        {
            btnText.text = "Equipped";
        }
        else
        {
            btnText.text = "Equip";
        }
    }

    private void UpdatePowerUpButtonUI(string prefKey, TMP_Text btnText)
    {
        if (btnText == null) return;

        bool isOwned = PlayerPrefs.GetInt(prefKey, 0) == 1;
        if (isOwned)
        {
            btnText.text = "Active"; // Power-ups are consumed on run, no 'unequip'
        }
        else
        {
            btnText.text = "Buy";
        }
    }

    private void UpdatePetButtonUI()
    {
        if (petButtonText == null) return;

        string petName = "CubePet";
        string ownedKey = "PetOwned_" + petName;
        bool isOwned = PlayerPrefs.GetInt(ownedKey, 0) == 1;

        if (!isOwned)
        {
            // Just "Buy" like you requested
            petButtonText.text = "Buy";
        }
        else if (ShopData.GetEquippedPet() == petName)
        {
            petButtonText.text = "Unequip";
        }
        else
        {
            petButtonText.text = "Equip";
        }
    }

    // ==========================================
    // UI EFFECTS
    // ==========================================

    private void SpawnPurchaseCoins(int amount)
    {
        if (coinSprite == null)
        {
            Debug.LogWarning("ShopManager: Cannot spawn coins because Coin Sprite is missing! Please drag it into the inspector.");
            return;
        }
        
        Canvas canvas = GetComponentInParent<Canvas>();
        if (canvas == null)
        {
            canvas = FindAnyObjectByType<Canvas>();
        }
        
        if (canvas == null) return;

        for (int i = 0; i < amount; i++)
        {
            StartCoroutine(AnimateCoin(canvas.transform));
        }
    }

    private System.Collections.IEnumerator AnimateCoin(Transform parent)
    {
        // Create UI Image
        GameObject coinObj = new GameObject("CoinFX");
        coinObj.transform.SetParent(parent, false);
        
        Image img = coinObj.AddComponent<Image>();
        img.sprite = coinSprite;
        img.rectTransform.sizeDelta = new Vector2(40, 40); // Size of the particle
        
        // Spawn exactly where the mouse clicked the button (works for all canvas types)
        RectTransform canvasRect = parent.GetComponent<RectTransform>();
        Canvas parentCanvas = parent.GetComponent<Canvas>();
        Camera cam = parentCanvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : parentCanvas.worldCamera;
        
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRect, Input.mousePosition, cam, out Vector2 localPoint))
        {
            img.rectTransform.anchoredPosition = localPoint;
        }

        // Random velocity (burst outwards and upwards)
        Vector2 velocity = new Vector2(Random.Range(-400f, 400f), Random.Range(300f, 700f));
        float gravity = 2000f;
        float duration = 1.0f;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            
            // Apply gravity
            velocity.y -= gravity * Time.deltaTime;
            
            // Move
            img.rectTransform.anchoredPosition += velocity * Time.deltaTime;
            
            // Spin
            img.rectTransform.Rotate(0, 0, Random.Range(-10f, 10f));
            
            // Fade out at end
            if (elapsed > duration * 0.7f)
            {
                float alpha = 1f - ((elapsed - (duration * 0.7f)) / (duration * 0.3f));
                img.color = new Color(1f, 1f, 1f, alpha);
            }

            yield return null;
        }

        Destroy(coinObj);
    }
}
