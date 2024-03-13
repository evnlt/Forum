namespace Forum.WebApi.Models;

public class PostDto
{
    public Guid Id { get; init; }

    public string Title { get; init; } = default!;

    public string Body { get; set; } = default!;

    public IEnumerable<CommentDto> Comments { get; init; } = default!;
}