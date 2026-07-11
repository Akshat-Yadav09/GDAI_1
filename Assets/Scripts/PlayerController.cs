using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float jumpForce = 14f;
    [Header("GD-Style Fall")]
    public float fallMultiplier = 3.5f;   // how much harder gravity pulls you down

    [Header("GD-Style Rotation")]
    [Tooltip("Drag the child sprite/visual object here. Leave empty to rotate the whole GameObject.")]
    public Transform visualTransform;
    public float rotationSpeed = 400f;    // degrees per second while airborne

    private Rigidbody2D rb;
    private GameManager gm;
    private PlayerTrail trail;
    private PlayerExplosion explosion;
    private bool isGrounded = true;
    private bool wasGrounded = true;
    private bool jumpRequested = false;
    private int jumpCount = 0;
    private float targetRotationZ = 0f;   // the next 90° snap target

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        gm = FindAnyObjectByType<GameManager>();
        if (gm == null) Debug.LogWarning("PlayerController: GameManager not found in scene!");

        trail = GetComponent<PlayerTrail>();
        explosion = GetComponent<PlayerExplosion>();

        // Lock X so friction from obstacles can't push the player sideways.
        // Also freeze rigidbody rotation since we handle rotation visually.
        rb.constraints = RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezeRotation;

        // If no visual assigned, rotate this transform directly
        if (visualTransform == null)
            visualTransform = transform;
    }

    /// <summary>
    /// Call this from a UI Button's OnClick event for mobile jump.
    /// </summary>
    public void Jump()
    {
        jumpRequested = true;
    }

    void Update()
    {
        // Allow holding spacebar, clicking/holding anywhere on screen (mobile touch), 
        // OR using the UI button to jump.
        bool isHoldingJump = Input.GetMouseButton(0) || Input.GetKey(KeyCode.Space) || jumpRequested;
        bool isNewJumpPress = Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Space) || jumpRequested;

        if (isHoldingJump && isGrounded)
        {
            rb.linearVelocity = Vector2.up * jumpForce;
            isGrounded = false;
            if (trail != null) trail.SetGrounded(false);

            // Set the next 90° rotation target (clockwise = negative Z)
            targetRotationZ -= 90f;
            jumpCount = 1;
            
            jumpRequested = false; // consumed
        }
        else if (isNewJumpPress && !isGrounded && jumpCount == 1)
        {
            if (PowerUpManager.Instance != null && PowerUpManager.Instance.HasDoubleJump)
            {
                // Double Jump!
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
                targetRotationZ -= 90f;
                jumpCount = 2;
                
                jumpRequested = false; // consumed
            }
        }
        
        // Reset the UI button request if unused
        jumpRequested = false;

        // Smoothly rotate toward the target while airborne
        if (!isGrounded)
        {
            float currentZ = visualTransform.localEulerAngles.z;
            // Convert to signed angle for smooth Lerp
            if (currentZ > 180f) currentZ -= 360f;
            float newZ = Mathf.MoveTowards(currentZ, targetRotationZ, rotationSpeed * Time.deltaTime);
            visualTransform.localEulerAngles = new Vector3(0f, 0f, newZ);
        }
    }

    void FixedUpdate()
    {
        // When falling, crank up gravity so the drop feels snappy like GD
        if (rb.linearVelocity.y < 0)
        {
            rb.linearVelocity += Vector2.up * Physics2D.gravity.y * (fallMultiplier - 1f) * Time.fixedDeltaTime;
        }
    }

    void LateUpdate()
    {
        wasGrounded = isGrounded;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        HandleCollision(collision);
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        // Reset isGrounded when leaving any surface (falling off edges)
        if (collision.gameObject.CompareTag("Ground") || collision.gameObject.CompareTag("Obstacle"))
        {
            // Only un-ground if we aren't currently touching another ground/obstacle block
            if (!IsTouchingGroundOrObstacle())
            {
                isGrounded = false;
                if (trail != null) trail.SetGrounded(false);
            }
        }
    }

    private bool IsTouchingGroundOrObstacle()
    {
        Collider2D col = GetComponent<Collider2D>();
        if (col == null) return false;
        
        ContactPoint2D[] contacts = new ContactPoint2D[10];
        int count = col.GetContacts(contacts);
        for (int i = 0; i < count; i++)
        {
            GameObject obj = contacts[i].collider.gameObject;
            if (obj.CompareTag("Ground") || obj.CompareTag("Obstacle"))
            {
                // Ensure we are resting on top of it, not just grazing a wall
                if (contacts[i].normal.y > 0.1f) return true;
            }
        }
        return false;
    }

    private void HandleCollision(Collision2D collision)
    {
        // 1. Hitting the ground is always safe
        if (collision.gameObject.CompareTag("Ground")) 
        { 
            if (!wasGrounded && CameraShake.Instance != null)
                CameraShake.Instance.Shake();
            isGrounded = true;
            jumpCount = 0;
            if (trail != null) trail.SetGrounded(true);
            SnapRotation();
        }
        
        // 2. Hitting an obstacle requires some math
        else if (collision.gameObject.CompareTag("Obstacle")) 
        { 
            // Check ALL contact normals — use the highest Y to avoid
            // false deaths when clipping a corner
            float maxNormalY = float.MinValue;
            for (int i = 0; i < collision.contactCount; i++)
            {
                maxNormalY = Mathf.Max(maxNormalY, collision.GetContact(i).normal.y);
            }

            // If the best normal is pointing UP (y > 0.5), we landed on top!
            // We also check if the player's bottom is roughly at or above the block's top.
            // This prevents "ghost collisions" where sliding across multiple perfectly aligned
            // blocks causes a tiny horizontal collision on the seams.
            Collider2D playerCol = GetComponent<Collider2D>();
            float playerBottom = playerCol != null ? playerCol.bounds.min.y : transform.position.y;
            float obsTop = collision.collider.bounds.max.y;

            bool isSafelyOnTop = maxNormalY > 0.5f || (playerBottom >= obsTop - 0.1f);

            if (isSafelyOnTop) 
            {
                if (!wasGrounded && CameraShake.Instance != null)
                    CameraShake.Instance.Shake();
                isGrounded = true; // Safe to jump again
                jumpCount = 0;
                if (trail != null) trail.SetGrounded(true);
                SnapRotation();
            }
            // Otherwise we hit the side — game over
            else 
            {
                Die(collision.gameObject);
            }
        }

        // 3. Hitting a SPIKE (Instant death from any angle)
        else if (collision.gameObject.CompareTag("Spike"))
        {
            Die(collision.gameObject);
        }
    }

    private void Die(GameObject killer = null)
    {
        // Try Revive PowerUp
        if (PowerUpManager.Instance != null && PowerUpManager.Instance.HasRevive && !PowerUpManager.Instance.ReviveUsed)
        {
            PowerUpManager.Instance.ReviveUsed = true;
            
            // Destroy the thing that killed us so we don't instantly die again
            if (killer != null && (killer.CompareTag("Obstacle") || killer.CompareTag("Spike")))
            {
                Destroy(killer);
            }

            // Auto-jump to save them
            rb.linearVelocity = Vector2.up * jumpForce;
            targetRotationZ -= 90f;
            jumpCount = 1; // Used up a jump
            
            if (CameraShake.Instance != null) CameraShake.Instance.Shake();
            
            // Add a little visual cue (e.g., flash the trail)
            if (trail != null) trail.SetGrounded(false);
            isGrounded = false;

            return; // Abort death
        }

        if (trail != null) trail.ClearOnDeath();
        if (explosion != null) explosion.Explode();
        if (gm != null) gm.TriggerGameOver();
    }

    /// <summary>
    /// Snap the visual to the nearest 90° on landing (just like GD).
    /// </summary>
    private void SnapRotation()
    {
        targetRotationZ = Mathf.Round(targetRotationZ / 90f) * 90f;
        visualTransform.localEulerAngles = new Vector3(0f, 0f, targetRotationZ);
    }
}