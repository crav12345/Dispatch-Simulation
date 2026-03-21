[System.Serializable]
public struct AppConfig
{
    public ServiceMask Services;
}

[System.Flags]
public enum ServiceMask
{
    None = 0
}
