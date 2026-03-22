using System.Collections.Generic;
using UnityEngine;

public class Simulation : MonoBehaviour, IExecutionMode
{
    private System.Random _rng;
    private readonly List<ISimulatedSystem> _simulatedSystems = new();

    public void Load(int citySeed, int simulationSeed)
    {
        _rng = new(simulationSeed);
        _simulatedSystems.Add(new IncidentScheduler());
    }

    private void FixedUpdate()
    {
        foreach (var system in _simulatedSystems)
        {
            system.Tick(_rng);
        }
    }
}
