namespace Forum.WebApi.Entities;

public class Like
{
    public Guid UserId { get; set; }

    public User User { get; set; } = default!;
    
    public Guid CommentId { get; set; }

    public Comment Comment { get; set; } = default!;
}