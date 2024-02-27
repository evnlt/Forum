namespace Forum.WebApi.Requests;

public class RefreshAccessTokenRequest
{
    public string AccessToken { get; init; } = default!;

    public string RefreshToken { get; init; } = default!;
}