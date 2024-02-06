using Forum.WebApi;
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
    
var app = builder.Build();

app.UseCors("client");

app.MapGet("api/posts", async (ApplicationDbContext applicationDbContext) => await applicationDbContext.Posts.AsNoTracking().ToListAsync());

app.Run();