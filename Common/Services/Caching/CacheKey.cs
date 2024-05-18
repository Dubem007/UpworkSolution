namespace Common.services.Caching;

public class CacheKey
{
    public static string GetTokenKey(long userId)
    {
        return $"UserToken:User_{userId}";
    }

    public static string GetAppKey(string key, string appName)
    {
        return $"{appName}:{key}";
    }
}
