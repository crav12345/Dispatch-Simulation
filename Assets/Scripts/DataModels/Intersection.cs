using System.Collections.Generic;
using UnityEngine;

public class Intersection
{
    public int ID;
    public Vector2 Position;
    public List<Road> ConnectedRoads = new();

    public Intersection(int id, Vector2 position)
    {
        ID = id;
        Position = position;
    }
}
