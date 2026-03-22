public struct Edge
{
    public int from;
    public int to;
    public int occupancy;
    public int capacity;
    public float length;
    public float speed;

    public readonly float TravelTime => length / speed * (1f + occupancy / (float)capacity);
}