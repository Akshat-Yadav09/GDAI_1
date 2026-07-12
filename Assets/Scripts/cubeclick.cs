using UnityEngine;

public class cubeclick : MonoBehaviour
{
    public GameObject cube;

    bool shown = false;

    void Start()
    {
        cube.SetActive(false);
    }

    void Update()
    {
        if (!shown && Input.anyKeyDown)
        {
            shown = true;
            cube.SetActive(true);
        }
    }
}