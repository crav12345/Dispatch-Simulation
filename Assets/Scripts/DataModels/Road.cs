public class Road
{
    public readonly int FromIdx;
    public readonly int ToIdx;
    public readonly float Length;

    public Road(int fromIdx, int toIdx, float length)
    {
        FromIdx = fromIdx;
        ToIdx = toIdx;
        Length = length;
    }
}
