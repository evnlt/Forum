using Forum.WebApi.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Forum.WebApi.Endpoints;

public static class AccountEndpoints
{
    public static async Task<IResult> Register(UserManager<ApplicationUser> userManager, [FromBody] RegisterRequest request)
    {
        var user = new ApplicationUser
        {
            UserName = request.Username,
            Email = request.Username,
        };
        
        await userManager.CreateAsync(user, request.Password);
        
        return Results.Ok(new { Message = "Registration successful" });
    }
    
    public class RegisterRequest
    {
        public string Username { get; init; } = default!;

        public string Password { get; init; } = default!;
    }
}