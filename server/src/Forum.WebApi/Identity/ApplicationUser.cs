using Microsoft.AspNetCore.Identity;

namespace Forum.WebApi.Identity;

public class ApplicationUser : IdentityUser<Guid>
{
    public RefreshToken RefreshToken { get; set; } = default!;
}