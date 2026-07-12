using UnityEngine;

/// <summary>
/// Attach to any UI element to make it gently bob up and down forever.
/// Perfect for title text, logos, or menu buttons!
/// </summary>
public class UIIdleFloat : MonoBehaviour
{
    [Tooltip("How far it floats up and down (in pixels)")]
    public float floatAmount = 10f;
    
    [Tooltip("How fast it bobs (higher = faster)")]
    public float speed = 2f;

    private RectTransform rectTransform;
    private Vector2 startPosition;

    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        startPosition = rectTransform.anchoredPosition;
    }

    void Update()
    {
        // Sine wave creates a smooth, endless up-and-down motion
        float offset = Mathf.Sin(Time.unscaledTime * speed) * floatAmount;
        rectTransform.anchoredPosition = startPosition + new Vector2(0f, offset);
    }
}
