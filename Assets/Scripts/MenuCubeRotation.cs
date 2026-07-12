using UnityEngine;

public class MenuCubeRotation : MonoBehaviour
{
    [Header("Rotation")]
    public Vector3 rotationSpeed = new Vector3(20f, 40f, 15f);

    [Header("Floating")]
    public float floatHeight = 0.25f;
    public float floatSpeed = 2f;

    private Vector3 startPos;

    void Start()
    {
        startPos = transform.position;
    }

    void Update()
    {
        // Rotate
        transform.Rotate(rotationSpeed * Time.deltaTime);

        // Float up and down
        Vector3 pos = startPos;
        pos.y += Mathf.Sin(Time.time * floatSpeed) * floatHeight;
        transform.position = pos;
    }
}