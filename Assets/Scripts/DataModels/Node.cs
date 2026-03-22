using System.Collections.Generic;
using UnityEngine;

public class Node
{
    public int ID;
    public int Occupants;
    public int Capacity;
    public Vector2 Position;
    public List<Edge> ConnectedRoads;
}