namespace Forum.WebApi.Responses;

public class AuthFailedResponse
{
    public IEnumerable<string> Errors { get; init; } = default!;
}