using UnityEngine;

public class ObstacleMover : MonoBehaviour
{
    public float speed = 8f;
    public float deadZone = -25f; // The X coordinate where it disappears
    private Rigidbody2D rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody2D>();
        }
        // Use Kinematic so they don't physically collide with the Ground!
        rb.bodyType = RigidbodyType2D.Kinematic;
        
        // Enable Continuous collision on the Kinematic body so it sweeps against the player
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        rb.useFullKinematicContacts = true;
    }

    void FixedUpdate()
    {
        float currentSpeed = DifficultyManager.Instance != null
            ? DifficultyManager.Instance.CurrentSpeed
            : speed;

        // Using linearVelocity on a Kinematic body produces perfect continuous sweeps
        rb.linearVelocity = new Vector2(-currentSpeed, 0f);
    }

    void Update()
    {
        // If it goes past the player and off-screen, deactivate it
        if (transform.position.x < deadZone)
        {
            gameObject.SetActive(false);
        }
    }
}