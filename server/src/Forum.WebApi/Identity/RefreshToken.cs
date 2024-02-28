namespace Forum.WebApi.Identity;

public class RefreshToken
{
    public string Value { get; set; } = Guid.NewGuid().ToString();

    public string JwtId { get; set; } = default!;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime ExpiresAt { get; set; } = DateTime.UtcNow.AddMonths(6);

    public bool Used { get; set; }

    public bool Invalidated { get; set; }

    public Guid UserId { get; init; }

    public ApplicationUser User { get; set; } = default!;
}