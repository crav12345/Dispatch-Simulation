using System.Collections;
using UnityEngine;

/// <summary>
/// Boots systems based on configuration passed in from App entry point
/// instance.
/// </summary>
public class Bootstrapper : MonoBehaviour
{
    [SerializeField] private ProgramRunner _runner;

    public void Bootstrap()
    {
        StartCoroutine(BootstrapCoroutine());
    }

    public IEnumerator BootstrapCoroutine()
    {
        yield return _runner.LoadProgram();
        // yield return Services.Mount();
        yield return _runner.RunProgram();
    }
}