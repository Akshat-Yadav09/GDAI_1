using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// Attach this to any button or panel in your game.
/// When the player hovers over it, it will tell the UITooltipSystem to show this specific text!
/// </summary>
public class UITooltip : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [TextArea(2, 5)]
    [Tooltip("The text that will appear in the hover box")]
    public string description = "Write your description here!";

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (UITooltipSystem.Instance != null)
        {
            UITooltipSystem.Instance.Show(description);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (UITooltipSystem.Instance != null)
        {
            UITooltipSystem.Instance.Hide();
        }
    }
    
    // Also hide the tooltip if this button is destroyed or disabled while hovering
    void OnDisable()
    {
        if (UITooltipSystem.Instance != null)
        {
            UITooltipSystem.Instance.Hide();
        }
    }
}
