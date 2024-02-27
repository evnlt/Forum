﻿namespace Forum.WebApi.Responses;

public class AuthSuccessResponse
{
    public string AccessToken { get; init; } = default!;

    public string RefreshToken { get; init; } = default!;
}