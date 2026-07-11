using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Attaches to the pet GameObject (a child of the Player).
/// Records the player's past positions and follows with a slight delay.
/// </summary>
public class PetFollower : MonoBehaviour
{
    [Header("Settings")]
    public float followDelay = 0.15f; // Seconds behind the player
    public float xOffset = -1.2f;     // Distance behind the player on X axis
    
    private Transform playerTransform;
    private Queue<Vector3> positionHistory = new Queue<Vector3>();
    private Queue<float> timeHistory = new Queue<float>();

    void Start()
    {
        // Must be a child of the player for this to work easily
        playerTransform = transform.parent;
        
        string equippedPet = ShopData.GetEquippedPet();
        // Default to showing it in editor for testing, or if "CubePet" is equipped
        if (equippedPet != "CubePet" && Application.isPlaying)
        {
            // For now, only hide if explicitly "None". Let's show it by default so the user can test it!
            if (equippedPet == "None" || equippedPet == "") 
            {
                gameObject.SetActive(false); // Hide if no pet equipped
                return;
            }
        }

        SetupPetVisuals();

        // Unparent so it moves independently in world space
        transform.SetParent(null);
        
        // Snap to starting offset immediately
        transform.position = playerTransform.position + new Vector3(xOffset, 0f, 0f);
    }

    private void SetupPetVisuals()
    {
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        if (sr == null)
        {
            sr = gameObject.AddComponent<SpriteRenderer>();
        }

        // Auto-generate a simple cube if no sprite is assigned
        if (sr.sprite == null)
        {
            Texture2D tex = new Texture2D(8, 8);
            Color[] pixels = new Color[64];
            for (int i = 0; i < pixels.Length; i++) pixels[i] = Color.white;
            tex.SetPixels(pixels);
            tex.Apply();
            sr.sprite = Sprite.Create(tex, new Rect(0, 0, 8, 8), new Vector2(0.5f, 0.5f), 16f);
        }

        // Try to match player color but darker
        SpriteRenderer playerSr = playerTransform.GetComponentInChildren<SpriteRenderer>();
        if (playerSr != null)
        {
            Color pColor = playerSr.color;
            // Make the pet a slightly darker shade of the player
            sr.color = new Color(pColor.r * 0.8f, pColor.g * 0.8f, pColor.b * 0.8f, 1f);
        }
        
        transform.localScale = Vector3.one * 0.5f; // Pet is half size of player
        sr.sortingOrder = 9; // Render slightly behind player
    }

    void Update()
    {
        if (playerTransform == null) return;

        // Record current player position
        positionHistory.Enqueue(playerTransform.position);
        timeHistory.Enqueue(Time.time);

        // Remove positions older than the delay
        while (timeHistory.Count > 0 && Time.time - timeHistory.Peek() >= followDelay)
        {
            timeHistory.Dequeue();
            Vector3 targetPos = positionHistory.Dequeue();
            
            // Apply X offset since the player's X is stationary in the world
            targetPos.x = playerTransform.position.x + xOffset;
            
            transform.position = targetPos; // Apply to pet
        }

        // Update color to match rainbow if rainbow is active
        if (ShopData.GetEquippedSkin() == "Rainbow")
        {
            SpriteRenderer sr = GetComponent<SpriteRenderer>();
            SpriteRenderer playerSr = playerTransform.GetComponentInChildren<SpriteRenderer>();
            if (sr != null && playerSr != null)
            {
                Color pColor = playerSr.color;
                sr.color = new Color(pColor.r * 0.8f, pColor.g * 0.8f, pColor.b * 0.8f, 1f);
            }
        }
    }
}
