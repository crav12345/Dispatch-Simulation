using System.Collections.Generic;
using UnityEngine;

public class Simulation : MonoBehaviour, IExecutionMode
{
    private System.Random _rng;
    private City _city;
    private GameObject _cityRoot;
    private readonly List<ISimulatedSystem> _simulatedSystems = new();

    public void Load(int citySeed, int simulationSeed)
    {
        _rng = new(simulationSeed);
        _simulatedSystems.Clear();
        _city = City.GenerateConnected(citySeed);

        if (_cityRoot != null)
        {
            Destroy(_cityRoot);
        }

        _cityRoot = _city.BuildScene(transform);
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
