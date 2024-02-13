using Forum.WebApi;
using Forum.WebApi.Entities;
using Forum.WebApi.Extentions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddDbContext<ApplicationDbContext>(options => options.UseNpgsql(builder.Configuration.GetDbConnectionString()));

builder.Services.AddCors(options =>
{
    options.AddPolicy("client",
        policy =>
        {
            // TODO - fix env variables
            policy.WithOrigins("http://localhost:5173")
                .AllowCredentials()
                .AllowAnyHeader()
                .AllowAnyMethod();
        });
});
    
var app = builder.Build();

app.UseCors("client");

app.MapGet("api/posts", 
    async (ApplicationDbContext applicationDbContext, CancellationToken cancellationToken) 
        => await applicationDbContext.Posts
            .AsNoTracking()
            .Select(x => new { x.Id, x.Title})
            .ToListAsync(cancellationToken));

app.MapGet("api/posts/{id}", 
    async (ApplicationDbContext applicationDbContext, CancellationToken cancellationToken, Guid id) 
        => await applicationDbContext.Posts
            .AsNoTracking()
            .Include(x => x.Comments)
            .Where(x=> x.Id == id)
            .FirstOrDefaultAsync(cancellationToken));

// TODO - create DTOs
// TODO - fix this endpoint

app.MapPost("api/posts/{postId}/comments", async (ApplicationDbContext applicationDbContext, CancellationToken cancellationToken, Guid postId, [FromBody] CreateCommentRequest request) =>
{
    if (request.Message is null || request.Message == "")
    {
        return Results.BadRequest();
    }

    var comment = new Comment
    {
        Message = request.Message,
        ParentId = request.ParentId,
        PostId = postId,
        UserId = Guid.Parse("75fbde02-faf8-4fea-8611-b01372bdd9b8")
    };

    applicationDbContext.Comments.Add(comment);
    await applicationDbContext.SaveChangesAsync(cancellationToken);

    return Results.Ok();
});
    
app.Run();

public class CreateCommentRequest
{
    public string? Message { get; init; }

    public Guid? ParentId { get; init; }
}