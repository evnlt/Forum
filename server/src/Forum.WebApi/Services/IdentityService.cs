using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Forum.WebApi.Identity;
using Forum.WebApi.Options;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Forum.WebApi.Services;

public interface IIdentityService
{
    Task<AuthenticationResult> Register(string email, string password, CancellationToken cancellationToken);
    
    Task<AuthenticationResult> Login(string email, string password, CancellationToken cancellationToken);
    
    Task<AuthenticationResult> RefreshAccessToken(string accessToken, string refreshToken, CancellationToken cancellationToken);
}

public class IdentityService : IIdentityService
{
    private readonly UserManager<ApplicationUser> _userManager;
    
    private readonly ApplicationDbContext _applicationDbContext;

    private readonly JwtOptions _jwtOptions;
    
    private readonly TokenValidationParameters _tokenValidationParameters;

    public IdentityService(
        UserManager<ApplicationUser> userManager,
        ApplicationDbContext applicationDbContext,
        IOptions<JwtOptions> jwtOptions,
        TokenValidationParameters tokenValidationParameters)
    {
        _userManager = userManager;
        _applicationDbContext = applicationDbContext;
        _jwtOptions = jwtOptions.Value;
        _tokenValidationParameters = tokenValidationParameters;
    }

    public async Task<AuthenticationResult> Register(string email, string password, CancellationToken cancellationToken)
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

        return await GenerateAuthResult(user, cancellationToken);
    }

    public async Task<AuthenticationResult> Login(string email, string password, CancellationToken cancellationToken)
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

        return await GenerateAuthResult(user, cancellationToken);
    }

    public async Task<AuthenticationResult> RefreshAccessToken(string accessToken, string refreshToken, CancellationToken cancellationToken)
    {
        var validatedToken = GetPrincipalFromAccessToken(accessToken);

        if (validatedToken is null)
        {
            return new AuthenticationResult { Errors = new[] { "Invalid token" } };
        }

        var expiryDateUnix = 
            long.Parse(validatedToken.Claims.Single(x => x.Type == JwtRegisteredClaimNames.Exp).Value);

        var expiryDateUtc = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            .AddSeconds(expiryDateUnix)
            .Add(_jwtOptions.AccessTokenLifetime);

        var jti = validatedToken.Claims.Single(x => x.Type == JwtRegisteredClaimNames.Jti).Value;

        var storedRefreshToken = await _applicationDbContext.RefreshTokens
            .FirstOrDefaultAsync(x => x.Value == refreshToken, cancellationToken);

        if (storedRefreshToken is null)
        {
            return new AuthenticationResult { Errors = new[] { "This refresh token does not exist" } };
        }

        if (DateTime.UtcNow > storedRefreshToken.ExpiresAt)
        {
            return new AuthenticationResult { Errors = new[] { "This refresh token has expired" } };
        }
        
        if (storedRefreshToken.Invalidated)
        {
            return new AuthenticationResult { Errors = new[] { "This refresh token has been invalidated" } };
        }
        
        if (storedRefreshToken.Used)
        {
            return new AuthenticationResult { Errors = new[] { "This refresh token has been used" } };
        }
        
        if (storedRefreshToken.JwtId != jti)
        {
            return new AuthenticationResult { Errors = new[] { "This refresh token does not match this JWT" } };
        }

        storedRefreshToken.Used = true;
        await _applicationDbContext.SaveChangesAsync(cancellationToken);

        var user = await _userManager.FindByIdAsync(validatedToken.Claims.Single(x => x.Type == "id").Value);

        return await GenerateAuthResult(user!, cancellationToken);
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
    

    private async Task<AuthenticationResult> GenerateAuthResult(ApplicationUser user, CancellationToken cancellationToken)
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
                new Claim("id", user.Id.ToString())
            }),
            Expires = DateTime.UtcNow.AddHours(1),
            SigningCredentials =
                new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };

        var accessToken = tokenHandler.CreateToken(tokenDescriptor);

        var refreshToken = new RefreshToken
        {
            JwtId = accessToken.Id,
            UserId = user.Id,
            ExpiresAt = DateTime.UtcNow.AddMonths(6)
        };

        _applicationDbContext.RefreshTokens.Add(refreshToken);
        await _applicationDbContext.SaveChangesAsync(cancellationToken);

        return new AuthenticationResult
        {
            Succeeded = true,
            AccessToken = tokenHandler.WriteToken(accessToken),
            RefreshToken = refreshToken
        };
    }
}

public class AuthenticationResult
{
    public bool Succeeded { get; init; }
    
    public string AccessToken { get; init; } = default!;

    public RefreshToken RefreshToken { get; init; } = default!;
    
    public IEnumerable<string> Errors { get; init; } = default!;
}