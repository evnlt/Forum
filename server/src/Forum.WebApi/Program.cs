using Forum.WebApi;
using Forum.WebApi.Endpoints;
using Forum.WebApi.Extentions;
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

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
    
var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseCors("client");

app.MapGet("api/posts", PostsEndpoints.GetPosts);
app.MapGet("api/posts/{id}", PostsEndpoints.GetPost);
app.MapPost("api/posts/{postId}/comments", PostsEndpoints.CreateComment);
app.MapPut("api/posts/{postId}/comments/{id}", PostsEndpoints.UpdateComment);
    
app.Run();