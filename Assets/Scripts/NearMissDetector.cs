using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Collections;

/// <summary>
/// Detects when a spike enters and exits a larger trigger zone around the player
/// without killing them, awarding a near-miss bonus.
/// </summary>
public class NearMissDetector : MonoBehaviour
{
    [Header("UI")]
    [Tooltip("Drag a world-space TextMeshPro element here for the popup")]
    public TMP_Text popupText;
    
    [Header("Settings")]
    [Tooltip("Radius of the near-miss detection zone")]
    public float detectionRadius = 1.2f;
    [Tooltip("Cooldown to prevent spamming from the same cluster of spikes")]
    public float cooldown = 0.3f;

    private CircleCollider2D triggerCol;
    private HashSet<GameObject> trackedSpikes = new HashSet<GameObject>();
    private float lastMissTime = 0f;

    /// <summary>
    /// Call this when obstacles are mass-deactivated (e.g. Revive screen wipe)
    /// to prevent stale references from building up.
    /// </summary>
    public void ClearTracked()
    {
        trackedSpikes.Clear();
    }

    void Start()
    {
        // Auto-create trigger collider if missing
        triggerCol = GetComponent<CircleCollider2D>();
        if (triggerCol == null)
        {
            triggerCol = gameObject.AddComponent<CircleCollider2D>();
        }
        triggerCol.isTrigger = true;
        triggerCol.radius = detectionRadius;

        if (popupText != null)
        {
            popupText.gameObject.SetActive(false);
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Spike"))
        {
            trackedSpikes.Add(other.gameObject);
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Spike") && trackedSpikes.Contains(other.gameObject))
        {
            trackedSpikes.Remove(other.gameObject);

            // If game is over, they didn't miss it, they died!
            if (GameManager.Instance != null && GameManager.Instance.IsGameOver)
                return;

            // Trigger near miss if off cooldown
            if (Time.time - lastMissTime >= cooldown)
            {
                lastMissTime = Time.time;
                TriggerNearMiss();
            }
        }
    }

    private void TriggerNearMiss()
    {
        if (CoinManager.Instance != null)
        {
            CoinManager.Instance.AddNearMissBonus();
        }

        // Show UI Popup
        if (popupText != null)
        {
            StopAllCoroutines();
            StartCoroutine(ShowPopup());
        }
    }

    private IEnumerator ShowPopup()
    {
        popupText.gameObject.SetActive(true);
        popupText.text = "NEAR MISS! +" + (CoinManager.Instance != null ? CoinManager.Instance.coinsPerNearMiss : 5);
        popupText.color = Color.yellow;
        popupText.transform.localScale = Vector3.one * 0.5f;

        float duration = 0.5f;
        float elapsed = 0f;

        Vector3 startPos = transform.position + Vector3.up * 1f;
        Vector3 endPos = startPos + Vector3.up * 1f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            
            // Float up
            popupText.transform.position = Vector3.Lerp(startPos, endPos, t);
            
            // Pop scale
            popupText.transform.localScale = Vector3.one * Mathf.Lerp(0.5f, 1f, Mathf.Sin(t * Mathf.PI));

            // Fade out at end
            if (t > 0.5f)
            {
                Color c = popupText.color;
                c.a = Mathf.Lerp(1f, 0f, (t - 0.5f) * 2f);
                popupText.color = c;
            }

            yield return null;
        }

        popupText.gameObject.SetActive(false);
    }
}
