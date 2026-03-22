using System.Collections.Generic;
using UnityEngine;

public class Intersection
{
    public readonly Vector2 Position;
    private readonly List<Road> _connectedRoads = new();

    public IReadOnlyList<Road> ConnectedRoads => _connectedRoads;

    public Intersection(Vector2 position)
    {
        Position = position;
    }

    public void AddConnectedRoad(Road road)
    {
        _connectedRoads.Add(road);
    }
}
