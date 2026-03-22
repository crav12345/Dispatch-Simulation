using UnityEngine;

public class Simulation : MonoBehaviour, IExecutionMode
{
    private System.Random _rng;
    private Graph _graph = new();

    public void Load(int citySeed, int simulationSeed)
    {
        _rng = new(simulationSeed);

        _graph.Generate(new Graph.Config()
        {
            seed = citySeed,
            width = 10,
            height = 50,
            spacing = 10f,
            extraEdgeChance = 0.2f
        });
        _graph.Build(0.1f);
    }

    private void FixedUpdate()
    {
        // TODO: Simulation logic.
    }
}
