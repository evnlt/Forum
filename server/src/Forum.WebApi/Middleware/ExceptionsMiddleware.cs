using Forum.WebApi.Exceptions;

namespace Forum.WebApi.Middleware;

public class ExceptionsMiddleware
{
    private readonly RequestDelegate _next;

    public ExceptionsMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task Invoke(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception exception)
        {
            (int statusCode, string errorMessage) = GetStatusCodeAndErrorMessage(exception);

            context.Response.StatusCode = statusCode;
            await context.Response.WriteAsJsonAsync(new ErrorDetailsResponse { Message = errorMessage });
        }
    }

    private static (int StatusCode, string ErrorMessage) GetStatusCodeAndErrorMessage(Exception exception)
    {
        return exception switch
        {
            NotFoundException => (StatusCodes.Status404NotFound, exception.Message),
            _ => (StatusCodes.Status500InternalServerError, "Internal Server Error")
        };
    }
}

public class ErrorDetailsResponse
{
    public string Message { get; init; } = default!;
}
