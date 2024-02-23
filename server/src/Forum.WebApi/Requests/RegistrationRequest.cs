namespace Forum.WebApi.Requests;

public class RegistrationRequest
{
    public string Email { get; init; } = default!;

    public string Password { get; init; } = default!;
}