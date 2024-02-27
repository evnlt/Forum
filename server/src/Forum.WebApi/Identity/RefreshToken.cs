namespace Forum.WebApi.Identity;

public class RefreshToken
{
    public string? Value { get; set; }
    
    public DateTime CreatedAt { get; set; }
    
    public DateTime ExpiresAt { get; set; }

    public bool Used { get; set; }

    public bool Invalidated { get; set; }

    public Guid UserId { get; init; }

    public ApplicationUser User { get; set; } = default!;
}