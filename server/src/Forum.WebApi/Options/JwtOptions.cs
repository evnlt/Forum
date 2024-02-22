namespace Forum.WebApi.Options;

public class JwtOptions
{
    public const string Section = "Jwt";

    public string Secret { get; init; } = default!;
}