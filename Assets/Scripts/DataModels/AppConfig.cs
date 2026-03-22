[System.Serializable]
public struct AppConfig
{
    public int CitySeed;
    public int SimulationSeed;
    public ServiceMask Services;
}

[System.Flags]
public enum ServiceMask
{
    None = 0
}
