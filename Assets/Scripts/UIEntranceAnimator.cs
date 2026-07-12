using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Attach this to a UI Canvas or Menu Manager.
/// Drag your buttons into the list in the order you want them to appear (top to bottom).
/// They will slide in from the left with a satisfying overshoot (inertia) bounce!
/// </summary>
public class UIEntranceAnimator : MonoBehaviour
{
    [Tooltip("Drag your buttons here in the order they should appear.")]
    public List<RectTransform> buttons;

    [Header("Animation Settings")]
    [Tooltip("How far off-screen they start (negative = left, positive = right)")]
    public float startOffsetX = -1000f;
    [Tooltip("Time to wait before the very first button starts moving")]
    public float initialDelay = 0.3f;
    [Tooltip("Time delay before the next button starts sliding in")]
    public float delayBetweenButtons = 0.15f;
    [Tooltip("How fast the slide animation plays")]
    public float animationDuration = 0.8f;
    [Tooltip("How bouncy the overshoot is. 8 is a good solid bounce.")]
    public float bounciness = 8f;
    [Tooltip("How many wiggles it does. 15 is a standard single bounce and settle.")]
    public float wiggles = 15f;

    [Header("Tap To Start Mode")]
    [Tooltip("If true, the buttons won't appear until you click/tap anywhere.")]
    public bool requireTapToStart = true;
    [Tooltip("Optional: Drag your 'Tap anywhere to start' Text object here so it hides when clicked.")]
    public GameObject tapToStartText;

    // Use a separate list so if you accidentally drag the same button twice, it doesn't break!
    private List<RectTransform> uniqueButtons = new List<RectTransform>();
    private List<Vector2> originalPositions = new List<Vector2>();

    void Awake()
    {
        // Save the original positions before we move anything
        foreach (var btn in buttons)
        {
            if (btn != null && !uniqueButtons.Contains(btn))
            {
                uniqueButtons.Add(btn);
                originalPositions.Add(btn.anchoredPosition);
            }
        }
    }

    void OnEnable()
    {
        // Hide all buttons instantly by moving them off-screen
        for (int i = 0; i < uniqueButtons.Count; i++)
        {
            Vector2 startPos = originalPositions[i];
            startPos.x += startOffsetX;
            uniqueButtons[i].anchoredPosition = startPos;
            
            // Turn off UIHoverFloat temporarily so it doesn't fight the animation!
            UIHoverFloat hoverScript = uniqueButtons[i].GetComponent<UIHoverFloat>();
            if (hoverScript != null)
            {
                hoverScript.enabled = false;
            }
        }

        if (requireTapToStart)
        {
            if (tapToStartText != null) tapToStartText.SetActive(true);
            StartCoroutine(WaitForTapRoutine());
        }
        else
        {
            if (tapToStartText != null) tapToStartText.SetActive(false);
            StartCoroutine(AnimateSequence());
        }
    }

    private IEnumerator WaitForTapRoutine()
    {
        // Wait until the player clicks anywhere or presses any key
        while (!Input.GetMouseButtonDown(0) && !Input.anyKeyDown)
        {
            yield return null;
        }

        // Hide the prompt text
        if (tapToStartText != null) tapToStartText.SetActive(false);

        // Small delay just so it feels responsive but not completely instant
        yield return new WaitForSecondsRealtime(0.1f);

        StartCoroutine(AnimateSequence());
    }

    private IEnumerator AnimateSequence()
    {
        // Wait for the initial delay so the scene has time to load before animating!
        yield return new WaitForSecondsRealtime(initialDelay);

        for (int i = 0; i < uniqueButtons.Count; i++)
        {
            StartCoroutine(AnimateButton(uniqueButtons[i], originalPositions[i]));
            // Wait for the delay before starting the next button
            yield return new WaitForSecondsRealtime(delayBetweenButtons);
        }
    }

    private IEnumerator AnimateButton(RectTransform rt, Vector2 targetPos)
    {
        float elapsed = 0f;
        Vector2 startPos = new Vector2(targetPos.x + startOffsetX, targetPos.y);

        while (elapsed < animationDuration)
        {
            elapsed += Time.unscaledDeltaTime;
            
            float t = elapsed / animationDuration;

            // This math formula creates a perfect physical spring with inertia!
            float spring = 1f - Mathf.Exp(-t * bounciness) * Mathf.Cos(t * wiggles);

            // Apply the spring value to slide it from Start to Target
            rt.anchoredPosition = Vector2.LerpUnclamped(startPos, targetPos, spring);

            yield return null;
        }

        // Snap exactly to final position at the end to be safe
        rt.anchoredPosition = targetPos;

        // Re-enable hover effects now that it has settled in place
        UIHoverFloat hoverScript = rt.GetComponent<UIHoverFloat>();
        if (hoverScript != null)
        {
            hoverScript.enabled = true;
        }
    }
}
