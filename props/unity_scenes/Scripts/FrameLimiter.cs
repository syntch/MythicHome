using UnityEngine;

public class FrameLimiter : MonoBehaviour
{
    void Awake()
    {
        // Disable VSync to let targetFrameRate take control
        QualitySettings.vSyncCount = 0;
        // Lock render rate to standard 60 FPS
        Application.targetFrameRate = 60;
    }
}