public class Road
{
    public int FromID;
    public int ToID;
    public float Length;

    public Road()
    {
    }

    public Road(int fromID, int toID, float length)
    {
        FromID = fromID;
        ToID = toID;
        Length = length;
    }
}
