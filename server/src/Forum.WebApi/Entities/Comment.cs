using System.Collections;
using Forum.WebApi.Identity;

namespace Forum.WebApi.Entities;

public class Comment
{
    public Guid Id { get; init; } = Guid.NewGuid();

    public string Message { get; set; } = default!;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    public Guid UserId { get; set; }

    public ApplicationUser User { get; set; } = default!;
    
    public Guid PostId { get; set; }

    public Guid? ParentId { get; set; }
    
    public Comment? Parent { get; set; }

    public ICollection<Like> Likes { get; set; } = default!;
}