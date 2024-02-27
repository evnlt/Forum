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
    
    Task<AuthenticationResult> RefreshAccessToken(string accessToken, string refreshToken);
}

public class IdentityService : IIdentityService
{
    private readonly UserManager<ApplicationUser> _userManager;

    private readonly JwtOptions _jwtOptions;
    
    private readonly TokenValidationParameters _tokenValidationParameters;

    public IdentityService(
        UserManager<ApplicationUser> userManager,
        IOptions<JwtOptions> jwtOptions,
        TokenValidationParameters tokenValidationParameters)
    {
        _userManager = userManager;
        _jwtOptions = jwtOptions.Value;
        _tokenValidationParameters = tokenValidationParameters;
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

    public Task<AuthenticationResult> RefreshAccessToken(string accessToken, string refreshToken)
    {
        throw new NotImplementedException();
    }

    private ClaimsPrincipal? GetPrincipalFromAccessToken(string token)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        try
        {
            var principal = tokenHandler.ValidateToken(token, _tokenValidationParameters, out var validatedToken);

            if (!IsJwtWithValidSecurityAlgorithm(validatedToken))
            {
                return null;
            }

            return principal;

        } catch
        {
            return null;
        }
    }
    
    private bool IsJwtWithValidSecurityAlgorithm(SecurityToken validatedToken)
    {
        return (validatedToken is JwtSecurityToken jwtSecurityToken) &&
            jwtSecurityToken.Header.Alg.Equals(value: SecurityAlgorithms.HmacSha256, 
                StringComparison.InvariantCultureIgnoreCase);
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
            AccessToken = tokenHandler.WriteToken(token)
        };
    }
}

public class AuthenticationResult
{
    public bool Succeeded { get; init; }
    
    public string AccessToken { get; init; } = default!;

    public string RefreshToken { get; init; } = default!;
    
    public IEnumerable<string> Errors { get; init; } = default!;
}