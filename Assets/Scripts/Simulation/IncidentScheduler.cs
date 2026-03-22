using UnityEngine;

public class IncidentScheduler : ISimulatedSystem
{
    private const float INCIDENTS_PER_SECOND = 0.1f;

    public void Tick(System.Random rng)
    {
        var incidents = MathUtils.SamplePoissonEvents(
            rng,
            INCIDENTS_PER_SECOND,
            Time.fixedDeltaTime);

        for (int i = 0; i < incidents; i++)
        {
            Debug.Log($"New incident at {Time.time}!");
        }
    }
}
