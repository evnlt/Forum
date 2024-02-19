using System.Text.Json.Serialization;
using Forum.WebApi;
using Forum.WebApi.Endpoints;
using Forum.WebApi.Extentions;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddDbContext<ApplicationDbContext>(options => options.UseNpgsql(builder.Configuration.GetDbConnectionString()));

builder.Services.ConfigureHttpJsonOptions(options =>
    {
        options.SerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
    });

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

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
    
var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseCors("client");

app.Use((context, next) =>
{
    context.Response.Cookies.Append("userId", "75fbde02-faf8-4fea-8611-b01372bdd9b8");
    return next(context);
});

app.MapGet("api/posts", PostsEndpoints.GetPosts);
app.MapGet("api/posts/{id}", PostsEndpoints.GetPost);
app.MapPost("api/posts/{postId}/comments", PostsEndpoints.CreateComment);
app.MapPut("api/posts/{postId}/comments/{id}", PostsEndpoints.UpdateComment);
app.MapDelete("api/posts/{postId}/comments/{id}", PostsEndpoints.DeleteComment);
app.MapPost("api/posts/{postId}/comments/{commentId}/toggleLike", PostsEndpoints.ToggleCommentLike);
    
app.Run();