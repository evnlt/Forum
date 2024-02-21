using System.Security.Authentication;
using System.Security.Claims;

namespace Forum.WebApi.Extentions;

public static class ClaimsPrincipalExtensions
{
    public static Guid GetCurrentUserId(this ClaimsPrincipal principal)
    {
        var claim = principal.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier);
        if (claim is not null && Guid.TryParse(claim.Value, out var currentUserId))
        {
            return currentUserId;
        }

        throw new AuthenticationException("User is not authenticated.");
    }
}