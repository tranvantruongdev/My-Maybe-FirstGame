public static class GameSetting
{
    public enum LoadType
    {
        New,
        Load
    }
    public enum Difficult
    {
        Easy,
        Normal,
        Hard
    }

    public static LoadType loadType;
    public static string username = "test";
    public static string uid = "123456";
    public static Difficult difficult;
}
