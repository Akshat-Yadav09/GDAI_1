using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// Attach this to any button or clickable UI element.
/// It will automatically play the generic click sound from the SoundManager when clicked!
/// </summary>
public class UIButtonSound : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler
{
    public void OnPointerClick(PointerEventData eventData)
    {
        if (SoundManager.Instance != null)
        {
            SoundManager.Instance.PlayClickSound();
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (SoundManager.Instance != null)
        {
            SoundManager.Instance.PlayHoverSound();
        }
    }
}
