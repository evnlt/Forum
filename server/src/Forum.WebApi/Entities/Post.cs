namespace Forum.WebApi.Entities;

public class Post
{
    public Guid Id { get; init; } = Guid.NewGuid();

    public string Title { get; set; } = default!;

    public string Body { get; set; } = default!;

    public ICollection<Comment> Comments { get; set; } = default!;
}