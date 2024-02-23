﻿using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Forum.WebApi.Identity;
using Forum.WebApi.Options;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;

namespace Forum.WebApi.Services;

public class IdentityService
{
    private readonly UserManager<ApplicationUser> _userManager;
    
    private readonly JwtOptions _jwtOptions;

    public IdentityService(UserManager<ApplicationUser> userManager, JwtOptions jwtOptions)
    {
        _userManager = userManager;
        _jwtOptions = jwtOptions;
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

        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(_jwtOptions.Secret);
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim("Id", user.Id.ToString())
            }),
            Expires = DateTime.UtcNow.AddHours(1),
            SigningCredentials =
                new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);

        return new AuthenticationResult
        {
            Token = tokenHandler.WriteToken(token)
        };
    }
}

public class AuthenticationResult
{
    public string Token { get; init; } = default!;
    
    public IEnumerable<string> Errors { get; init; } = default!;
}