using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Attach this to a UI Image to make its color slowly breathe between two colors!
/// </summary>
[RequireComponent(typeof(Image))]
public class UIPulseBackground : MonoBehaviour
{
    [Tooltip("The first color of the gradient")]
    public Color colorA = new Color(0.05f, 0.05f, 0.1f); // Very dark blue

    [Tooltip("The second color to pulse towards")]
    public Color colorB = new Color(0.15f, 0.05f, 0.2f); // Dark purple

    [Tooltip("How fast the colors blend back and forth")]
    public float pulseSpeed = 0.5f;

    private Image bgImage;

    void Start()
    {
        bgImage = GetComponent<Image>();
    }

    void Update()
    {
        // Smoothly blend between 0 and 1 over time (unscaledTime so it works even if paused)
        float t = (Mathf.Sin(Time.unscaledTime * pulseSpeed) + 1f) / 2f;
        
        bgImage.color = Color.Lerp(colorA, colorB, t);
    }
}
