using UnityEngine;

public class Simulation : MonoBehaviour, IExecutionMode
{
    private System.Random _rng;

    public void Load(int citySeed, int simulationSeed)
    {
        var graph = GraphGenerator.Generate
        (
            width: 10,
            height: 10,
            spacing: 5f,
            jitter: 0.0f,
            extraEdgeChance: 0.15f,
            seed: citySeed
        );
        GraphGenerator.Build(graph);

        _rng = new(simulationSeed);
    }

    private void FixedUpdate()
    {
        var incidentRoll = _rng.Next(1000);
        if (incidentRoll > 998)
        {
            Debug.Log($"Spawn Incident at {Time.time}");
        }
    }
}
