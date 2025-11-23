using UnityEngine;

public class EnvironmentControl2D : MonoBehaviour
{
    [Range(0f, 1f)]
    public float lightLevel = 1f;
    private bool isFullScreen = false;

    void Update()
    {
        // Toggle lightLevel with Space (1 or 0.2)
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (lightLevel > 0.5f)
                lightLevel = 0.2f;
            else
                lightLevel = 1f;

            Debug.Log("Light level is now: " + lightLevel);
        }

        // Fullscreen toggle
        if (Input.GetKeyDown(KeyCode.F))
        {
            isFullScreen = !isFullScreen;
            Screen.fullScreen = isFullScreen;
        }

        // Quit (only works in builds, not editor)
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }
    }
}