using UnityEngine;

public static class MathUtils
{
    public static int SamplePoissonEvents(System.Random rng, float lambda, float deltaTime)
    {
        if (lambda <= 0f || deltaTime <= 0f)
        {
            return 0;
        }

        var expectedEvents = lambda * deltaTime;
        var threshold = Mathf.Exp(-expectedEvents);
        var events = 0;
        var product = 1.0;

        while (true)
        {
            events++;
            product *= rng.NextDouble();

            if (product <= threshold)
            {
                return events - 1;
            }
        }
    }

}
