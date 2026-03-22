using System.Collections.Generic;
using UnityEngine;

public class Graph
{
    public List<Vector2> nodes = new();
    public List<RoadEdge> edges = new();
    public Dictionary<int, List<RoadEdge>> adjacency = new();

    public void BuildAdjacency()
    {
        adjacency.Clear();

        for (int i = 0; i < nodes.Count; i++)
            adjacency[i] = new List<RoadEdge>();

        foreach (var edge in edges)
            adjacency[edge.from].Add(edge);
    }
}

public struct RoadEdge
{
    public int from;
    public int to;
    public float length;
    public float speed;
    public int capacity;
    public int occupancy;

    public readonly float TravelTime => length / speed * (1f + occupancy / (float)capacity);
}