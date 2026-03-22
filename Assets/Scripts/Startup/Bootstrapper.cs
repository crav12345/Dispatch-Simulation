using System.Collections;
using UnityEngine;

/// <summary>
/// Boots systems based on configuration passed in from App entry point
/// instance.
/// </summary>
public class Bootstrapper : MonoBehaviour
{
    [SerializeField] private ProgramRunner _runner;

    public void Bootstrap(AppConfig appConfig)
    {
        StartCoroutine(BootstrapCoroutine(appConfig));
    }

    public IEnumerator BootstrapCoroutine(AppConfig appConfig)
    {
        var citySeed = appConfig.CitySeed;
        var simulationSeed = appConfig.SimulationSeed;
        yield return _runner.LoadProgram(citySeed, simulationSeed);
        // yield return Services.Mount();
        yield return _runner.RunProgram();
    }
}