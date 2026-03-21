using UnityEngine;

/// <summary>
/// Application entry point. Enables configuration in a single location in
/// Unity inspector. Triggers bootstrap process.
/// </summary>
public class App : MonoBehaviour
{
    private void Start()
    {
        DontDestroyOnLoad(this);

        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = 60;

        var bootstrapper = GetComponentInChildren<Bootstrapper>();
        bootstrapper.Bootstrap();
    }
}