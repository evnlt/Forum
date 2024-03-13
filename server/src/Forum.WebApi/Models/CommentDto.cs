using Forum.WebApi.Identity;

namespace Forum.WebApi.Models;

public class CommentDto
{
    public Guid Id { get; init; }
    
    public string Message { get; init; } = default!;

    public DateTime CreatedAt { get; init; }

    public ApplicationUser User { get; init; } = default!;

    public Guid? ParentId { get; init; }
    
    public int LikeCount { get; init; }

    public bool LikedByMe { get; init; }
}