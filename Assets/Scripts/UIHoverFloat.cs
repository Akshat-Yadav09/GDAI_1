using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// Attach to any UI element to make it scale up and float up when hovered.
/// It also automatically plays Hover and Click sounds!
/// </summary>
public class UIHoverFloat : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    [Header("Hover Settings")]
    public float floatAmount = 15f;    // How many pixels it floats up
    public float scaleAmount = 1.05f;  // How much bigger it gets (1.05 = 5% bigger)
    public float animationSpeed = 12f; // How fast it moves

    [Tooltip("If true, automatically plays sounds from the SoundManager.")]
    public bool playSounds = true;

    private RectTransform rectTransform;
    private Vector2 originalPosition;
    private Vector3 originalScale;
    
    private bool isHovering = false;

    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        originalPosition = rectTransform.anchoredPosition;
        originalScale = rectTransform.localScale;
    }

    void Update()
    {
        // Calculate target values based on hover state
        Vector2 targetPos = isHovering ? originalPosition + new Vector2(0, floatAmount) : originalPosition;
        Vector3 targetScale = isHovering ? originalScale * scaleAmount : originalScale;

        // Smoothly animate towards targets (Using unscaledDeltaTime so it works when paused!)
        rectTransform.anchoredPosition = Vector2.Lerp(rectTransform.anchoredPosition, targetPos, Time.unscaledDeltaTime * animationSpeed);
        rectTransform.localScale = Vector3.Lerp(rectTransform.localScale, targetScale, Time.unscaledDeltaTime * animationSpeed);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        isHovering = true;
        
        if (playSounds && SoundManager.Instance != null)
        {
            SoundManager.Instance.PlayHoverSound();
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isHovering = false;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (playSounds && SoundManager.Instance != null)
        {
            SoundManager.Instance.PlayClickSound();
        }
    }
}
