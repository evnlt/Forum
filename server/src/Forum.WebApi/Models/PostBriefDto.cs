namespace Forum.WebApi.Models;

public class PostBriefDto
{
    public Guid Id { get; init; }

    public string Title { get; init; } = default!;
}