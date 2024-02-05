using Forum.WebApi;
using Forum.WebApi.Extentions;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddDbContext<ApplicationDbContext>(options => options.UseNpgsql(builder.Configuration.GetDbConnectionString()));
    
var app = builder.Build();

app.MapGet("api/posts",
    async (ApplicationDbContext applicationDbContext) => await applicationDbContext.Posts.AsNoTracking().ToListAsync());

app.Run();