namespace Forum.WebApi.Extentions;

public static class ConfigurationExtensions
{
    private const string DbConnectionStringKey = "ForumDB";

    public static string GetDbConnectionString(this IConfiguration configuration)
    {
        return configuration.GetConnectionString(DbConnectionStringKey) ?? throw new KeyNotFoundException();
    }
}