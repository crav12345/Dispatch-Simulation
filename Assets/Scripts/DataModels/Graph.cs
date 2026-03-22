using System.Collections.Generic;
using UnityEngine;

public class Graph : Dictionary<int, Node>
{
    public struct Config
    {
        public int seed;
        public int width;
        public int height;
        public float spacing;
        public float extraEdgeChance;
    }

    private readonly List<Edge> _edges = new();

    public void Generate(Config config)
    {
        Clear();
        _edges.Clear();

        var seed = config.seed;
        var width = config.width;
        var height = config.height;
        var spacing = config.spacing;
        var extraEdgeChance = config.extraEdgeChance;
        var rng = new System.Random(seed);
        var nodeGrid = new int[width, height];

        // Nodes
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                var pos = new Vector2(
                    x * spacing,
                    y * spacing
                );

                var nodeIndex = Keys.Count;
                Add(nodeIndex, new Node()
                {
                    ID = nodeIndex,
                    Position = pos,
                    Occupants = 0,
                    Capacity = 10,
                    ConnectedRoads = new List<Edge>()
                });
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
                AddBidirectionalEdge(a, b, rng);

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
                    if (a >= b)
                    {
                        continue;
                    }

                    if (!HasEdge(a, b) && rng.NextDouble() < extraEdgeChance)
                    {
                        AddBidirectionalEdge(a, b, rng);
                    }
                }
            }
        }
    }

    public GameObject Build(float scale = 0.25f)
    {
        var root = new GameObject("RoadGraph");
        root.transform.localScale = Vector3.one * scale;

        // 1. Compute center
        Vector2 min = this[0].Position;
        Vector2 max = this[0].Position;

        foreach (var n in Values)
        {
            min = Vector2.Min(min, n.Position);
            max = Vector2.Max(max, n.Position);
        }

        Vector2 center = (min + max) * 0.5f;

        // 2. Build edges centered
        for (int i = 0; i < _edges.Count; i++)
        {
            var edge = _edges[i];
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

            Vector2 a2 = this[edge.from].Position - center;
            Vector2 b2 = this[edge.to].Position - center;

            lr.SetPosition(0, a2);
            lr.SetPosition(1, b2);
        }

        return root;
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

    private void AddBidirectionalEdge(int a, int b, System.Random rng)
    {
        var nodeA = this[a];
        var nodeB = this[b];
        var length = Vector2.Distance(nodeA.Position, nodeB.Position);
        var speed = 8f + (float)rng.NextDouble() * 4f;
        var capacity = rng.Next(5, 20);

        var edgeA = new Edge
        {
            from = a,
            to = b,
            length = length,
            speed = speed,
            capacity = capacity,
            occupancy = 0
        };
        nodeA.ConnectedRoads.Add(edgeA);
        _edges.Add(edgeA);

        var edgeB = new Edge
        {
            from = b,
            to = a,
            length = length,
            speed = speed,
            capacity = capacity,
            occupancy = 0
        };
        nodeB.ConnectedRoads.Add(edgeB);
        _edges.Add(edgeB);
    }

    private bool HasEdge(int a, int b)
    {
        foreach (var edge in this[a].ConnectedRoads)
        {
            if ((edge.from == a && edge.to == b) || (edge.from == b && edge.to == a))
            {
                return true;
            }
        }

        return false;
    }
}