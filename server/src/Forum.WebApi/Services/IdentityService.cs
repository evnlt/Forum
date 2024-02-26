using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Forum.WebApi.Identity;
using Forum.WebApi.Options;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Forum.WebApi.Services;

public interface IIdentityService
{
    Task<AuthenticationResult> Register(string email, string password);
    
    Task<AuthenticationResult> Login(string email, string password);
}

public class IdentityService : IIdentityService
{
    private readonly UserManager<ApplicationUser> _userManager;
    
    private readonly JwtOptions _jwtOptions;

    public IdentityService(
        UserManager<ApplicationUser> userManager, 
        IOptions<JwtOptions> jwtOptions)
    {
        _userManager = userManager;
        _jwtOptions = jwtOptions.Value;
    }

    public async Task<AuthenticationResult> Register(string email, string password)
    {
        var user = await _userManager.FindByEmailAsync(email);

        if (user is not null)
        {
            return new AuthenticationResult
            {
                Errors = new[] { "User with this email already exists" }
            };
        }

        user = new ApplicationUser()
        {
            Email = email,
            UserName = email
        };

        var result = await _userManager.CreateAsync(user, password);

        if (!result.Succeeded)
        {
            return new AuthenticationResult
            {
                Errors = result.Errors.Select(x => x.Description)
            };
        }

        return GenerateAuthResult(user);
    }

    public async Task<AuthenticationResult> Login(string email, string password)
    {
        var user = await _userManager.FindByEmailAsync(email);

        if (user is null)
        {
            return new AuthenticationResult
            {
                Errors = new[] { "User with this email does not exist" }
            };
        }

        var passwordIsValid = await _userManager.CheckPasswordAsync(user, password);
        
        if (!passwordIsValid)
        {
            return new AuthenticationResult
            {
                Errors = new[] { "User password is not valid" }
            };
        }

        return GenerateAuthResult(user);
    }

    private AuthenticationResult GenerateAuthResult(ApplicationUser user)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(_jwtOptions.Secret);
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Email!),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email!),
                new Claim("Id", user.Id.ToString())
            }),
            Expires = DateTime.UtcNow.AddHours(1),
            SigningCredentials =
                new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);

        return new AuthenticationResult
        {
            Succeeded = true,
            Token = tokenHandler.WriteToken(token)
        };
    }
}

public class AuthenticationResult
{
    public bool Succeeded { get; init; }
    
    public string Token { get; init; } = default!;
    
    public IEnumerable<string> Errors { get; init; } = default!;
}