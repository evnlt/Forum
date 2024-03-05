using System.Security.Authentication;
using System.Security.Claims;

namespace Forum.WebApi.Extentions;

public static class ClaimsPrincipalExtensions
{
    public static Guid GetCurrentUserId(this HttpContext httpContext)
    {
        var claim = httpContext.User.Claims.FirstOrDefault(x => x.Type == "id");
        if (claim is not null && Guid.TryParse(claim.Value, out var currentUserId))
        {
            return currentUserId;
        }

        throw new AuthenticationException("User is not authenticated.");
    }
    
    public static Guid? TryGetCurrentUserId(this HttpContext httpContext)
    {
        var claim = httpContext.User.Claims.FirstOrDefault(x => x.Type == "id");
        if (claim is not null && Guid.TryParse(claim.Value, out var currentUserId))
        {
            return currentUserId;
        }

        return null;
    }
}