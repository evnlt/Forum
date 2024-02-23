using System.Text;
using System.Text.Json.Serialization;
using Forum.WebApi;
using Forum.WebApi.Endpoints;
using Forum.WebApi.Extentions;
using Forum.WebApi.Identity;
using Forum.WebApi.Options;
using Forum.WebApi.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Swashbuckle.AspNetCore.SwaggerGen;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOptions<JwtOptions>().Bind(builder.Configuration.GetSection(JwtOptions.Section));
var jwtOptions = builder.Configuration.GetSection(JwtOptions.Section).Get<JwtOptions>();

builder.Services.AddScoped<IIdentityService, IdentityService>();

builder.Services
    .AddAuthentication(x =>
    {
        x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        x.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
        x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(x =>
    {
        x.SaveToken = true;
        x.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(jwtOptions!.Secret)),
            ValidateIssuer = false,
            ValidateAudience = false,
            RequireExpirationTime = false,
            ValidateLifetime = true
        };
    });  

builder.Services.AddAuthorization();

builder.Services
    .AddDbContext<ApplicationDbContext>(options => options.UseNpgsql(builder.Configuration.GetDbConnectionString()))
    .AddIdentityCore<ApplicationUser>(options =>
    {
        options.Password.RequireDigit = false;
        options.Password.RequireLowercase = false;
        options.Password.RequireNonAlphanumeric = false;
        options.Password.RequireUppercase = false;
        options.Password.RequiredLength = 6;
    })
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

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
builder.Services.AddTransient<IConfigureOptions<SwaggerGenOptions>, SwaggerOptions>();
    
var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseCors("client");

app.Use((context, next) =>
{
    context.Response.Cookies.Append("userId", "75fbde02-faf8-4fea-8611-b01372bdd9b8");
    return next(context);
});

app.UseHttpsRedirection();
app.UseAuthorization();

app.MapPost("api/account/register", AccountEndpoints.Register);

app.MapGet("api/posts", PostsEndpoints.GetPosts);
app.MapGet("api/posts/{id}", PostsEndpoints.GetPost);
app.MapPost("api/posts/{postId}/comments", PostsEndpoints.CreateComment);
app.MapPut("api/posts/{postId}/comments/{id}", PostsEndpoints.UpdateComment);
app.MapDelete("api/posts/{postId}/comments/{id}", PostsEndpoints.DeleteComment);
app.MapPost("api/posts/{postId}/comments/{commentId}/toggleLike", PostsEndpoints.ToggleCommentLike);
    
app.Run();