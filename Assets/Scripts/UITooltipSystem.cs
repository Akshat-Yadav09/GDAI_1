using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Place this script on an Empty GameObject in your Canvas.
/// It acts as the central manager that moves the tooltip box to your mouse cursor!
/// </summary>
public class UITooltipSystem : MonoBehaviour
{
    public static UITooltipSystem Instance;

    [Header("UI References")]
    [Tooltip("The background panel box of the tooltip")]
    public GameObject tooltipPanel;
    [Tooltip("The text inside the tooltip box")]
    public TMP_Text tooltipText;
    
    [Header("Settings")]
    [Tooltip("How long to wait before showing the tooltip (in seconds)")]
    public float hoverDelay = 0.4f;
    [Tooltip("How far away from the mouse cursor the box should appear")]
    public Vector2 mouseOffset = new Vector2(25f, -25f);

    private RectTransform panelRect;
    private Canvas parentCanvas;
    private Coroutine showRoutine;

    void Awake()
    {
        Instance = this;
        panelRect = tooltipPanel.GetComponent<RectTransform>();
        
        // Find the canvas even if this script was accidentally placed outside of it!
        parentCanvas = GetComponentInParent<Canvas>();
        if (parentCanvas == null) 
            parentCanvas = FindAnyObjectByType<Canvas>();
        
        // Hide the tooltip instantly on start
        Hide();
    }

    void Update()
    {
        // Only do math if the tooltip is actually visible on screen
        if (tooltipPanel.activeSelf)
        {
            Camera cam = parentCanvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : parentCanvas.worldCamera;
            
            // Convert mouse position to Canvas position (super accurate for any UI setup)
            if (RectTransformUtility.ScreenPointToLocalPointInRectangle(parentCanvas.transform as RectTransform, Input.mousePosition, cam, out Vector2 localPoint))
            {
                panelRect.anchoredPosition = localPoint + mouseOffset;
            }
        }
    }

    public void Show(string text)
    {
        if (showRoutine != null) StopCoroutine(showRoutine);
        showRoutine = StartCoroutine(ShowDelayed(text));
    }

    private System.Collections.IEnumerator ShowDelayed(string text)
    {
        // Wait for the delay
        yield return new WaitForSecondsRealtime(hoverDelay);

        tooltipText.text = text;
        tooltipPanel.SetActive(true);
        
        // Force Unity to recalculate the size of the box INSTANTLY 
        // so it perfectly matches the length of the new text!
        LayoutRebuilder.ForceRebuildLayoutImmediate(panelRect);

        // Force an update immediately so it doesn't flash in the wrong spot for 1 frame
        Update(); 
    }

    public void Hide()
    {
        if (showRoutine != null) StopCoroutine(showRoutine);
        tooltipPanel.SetActive(false);
    }
}
