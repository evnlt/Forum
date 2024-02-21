using Forum.WebApi.Identity;

namespace Forum.WebApi.Entities;

public class Like
{
    public Guid UserId { get; set; }

    public ApplicationUser User { get; set; } = default!;
    
    public Guid CommentId { get; set; }

    public Comment Comment { get; set; } = default!;
}