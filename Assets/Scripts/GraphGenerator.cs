using System.Collections.Generic;
using UnityEngine;

public static class GraphGenerator
{
    public static Graph Generate(
        int width,
        int height,
        float spacing,
        float jitter,
        float extraEdgeChance,
        int seed)
    {
        var rng = new System.Random(seed);
        var graph = new Graph();
        var nodeGrid = new int[width, height];

        // Nodes
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                float jx = ((float)rng.NextDouble() - 0.5f) * jitter;
                float jy = ((float)rng.NextDouble() - 0.5f) * jitter;

                var pos = new Vector2(
                    x * spacing + jx,
                    y * spacing + jy
                );

                int nodeIndex = graph.nodes.Count;
                graph.nodes.Add(pos);
                nodeGrid[x, y] = nodeIndex;
            }
        }

        // Connected backbone
        var visited = new bool[width, height];
        var stack = new Stack<Vector2Int>();
        stack.Push(new Vector2Int(0, 0));
        visited[0, 0] = true;

        while (stack.Count > 0)
        {
            var current = stack.Pop();
            var neighbors = GetNeighbors(current, width, height);
            Shuffle(neighbors, rng);

            foreach (var n in neighbors)
            {
                if (visited[n.x, n.y]) continue;

                int a = nodeGrid[current.x, current.y];
                int b = nodeGrid[n.x, n.y];
                AddBidirectionalEdge(graph, a, b, rng);

                visited[n.x, n.y] = true;
                stack.Push(n);
            }
        }

        // Extra edges
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                int a = nodeGrid[x, y];

                foreach (var n in GetNeighbors(new Vector2Int(x, y), width, height))
                {
                    int b = nodeGrid[n.x, n.y];
                    if (a >= b) continue;

                    if (!HasEdge(graph, a, b) && rng.NextDouble() < extraEdgeChance)
                        AddBidirectionalEdge(graph, a, b, rng);
                }
            }
        }

        graph.BuildAdjacency();
        return graph;
    }

    public static GameObject Build(Graph graph, float scale = 0.25f)
    {
        var root = new GameObject("RoadGraph");
        root.transform.localScale = Vector3.one * scale;

        // 1. Compute center
        Vector2 min = graph.nodes[0];
        Vector2 max = graph.nodes[0];

        foreach (var n in graph.nodes)
        {
            min = Vector2.Min(min, n);
            max = Vector2.Max(max, n);
        }

        Vector2 center = (min + max) * 0.5f;

        // 2. Build edges centered
        for (int i = 0; i < graph.edges.Count; i++)
        {
            var edge = graph.edges[i];
            if (edge.from > edge.to) continue;

            var go = new GameObject($"Edge_{edge.from}_{edge.to}");
            go.transform.SetParent(root.transform, false);

            var lr = go.AddComponent<LineRenderer>();
            lr.positionCount = 2;
            lr.useWorldSpace = false;
            lr.widthMultiplier = 0.15f;
            lr.material = new Material(Shader.Find("Sprites/Default"));
            lr.startColor = Color.white;
            lr.endColor = Color.white;

            Vector2 a2 = graph.nodes[edge.from] - center;
            Vector2 b2 = graph.nodes[edge.to] - center;

            lr.SetPosition(0, a2);
            lr.SetPosition(1, b2);
        }

        return root;
    }

    private static void AddBidirectionalEdge(Graph graph, int a, int b, System.Random rng)
    {
        float length = Vector2.Distance(graph.nodes[a], graph.nodes[b]);
        float speed = 8f + (float)rng.NextDouble() * 4f;
        int capacity = rng.Next(5, 20);

        graph.edges.Add(new RoadEdge
        {
            from = a,
            to = b,
            length = length,
            speed = speed,
            capacity = capacity,
            occupancy = 0
        });

        graph.edges.Add(new RoadEdge
        {
            from = b,
            to = a,
            length = length,
            speed = speed,
            capacity = capacity,
            occupancy = 0
        });
    }

    private static bool HasEdge(Graph graph, int a, int b)
    {
        for (int i = 0; i < graph.edges.Count; i++)
        {
            var e = graph.edges[i];
            if ((e.from == a && e.to == b) || (e.from == b && e.to == a))
                return true;
        }

        return false;
    }

    private static List<Vector2Int> GetNeighbors(Vector2Int pos, int width, int height)
    {
        var result = new List<Vector2Int>(4);

        if (pos.x > 0) result.Add(new Vector2Int(pos.x - 1, pos.y));
        if (pos.x < width - 1) result.Add(new Vector2Int(pos.x + 1, pos.y));
        if (pos.y > 0) result.Add(new Vector2Int(pos.x, pos.y - 1));
        if (pos.y < height - 1) result.Add(new Vector2Int(pos.x, pos.y + 1));

        return result;
    }

    private static void Shuffle(List<Vector2Int> list, System.Random rng)
    {
        for (int i = list.Count - 1; i > 0; i--)
        {
            int j = rng.Next(0, i + 1);
            (list[i], list[j]) = (list[j], list[i]);
        }
    }
}