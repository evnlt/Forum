using Forum.WebApi.Requests;
using Forum.WebApi.Responses;
using Forum.WebApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace Forum.WebApi.Endpoints;

public static class AccountEndpoints
{
    public static async Task<IResult> Register(IIdentityService identityService, [FromBody] RegistrationRequest request)
    {
        var response = await identityService.Register(request.Email, request.Password);

        if (!response.Succeeded)
        {
            return Results.BadRequest(new AuthFailedResponse
            {
                Errors = response.Errors
            });
        }
        
        return Results.Ok(new AuthSuccessResponse
        {
            Token = response.Token
        });
    }
}