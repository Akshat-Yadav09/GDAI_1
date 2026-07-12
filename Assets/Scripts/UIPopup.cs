using UnityEngine;

/// <summary>
/// Attach this to any UI Panel (like Game Over or Pause Menu)
/// to make it pop up with a beautiful, bouncy animation when it appears.
/// It uses unscaled time so it works perfectly even when the game is frozen!
/// </summary>
public class UIPopup : MonoBehaviour
{
    [Tooltip("How fast the popup animates in")]
    public float animationSpeed = 8f;
    
    private float timer = 0f;

    void OnEnable()
    {
        // Reset timer and shrink to nothing the moment the menu is turned on
        timer = 0f;
        transform.localScale = Vector3.zero;
    }

    void Update()
    {
        // Stop calculating once the animation is fully settled (around timer = 5)
        if (timer > 5f) return;

        timer += Time.unscaledDeltaTime * animationSpeed;

        // A highly stable math function that creates a perfect "bouncy/overshoot" effect!
        float scale = 1f - Mathf.Exp(-timer) * Mathf.Cos(timer * 2.5f);
        
        transform.localScale = new Vector3(scale, scale, 1f);
    }
}
