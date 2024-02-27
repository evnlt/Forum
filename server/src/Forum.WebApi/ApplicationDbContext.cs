using Forum.WebApi.Configuration;
using Forum.WebApi.Entities;
using Forum.WebApi.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Forum.WebApi;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, Guid>
{
    public DbSet<RefreshToken> RefreshTokens { get; init; } = default!;
    
    public DbSet<Post> Posts { get; init; } = default!;

    public DbSet<Comment> Comments { get; init; } = default!;

    public DbSet<Like> Likes { get; init; } = default!;

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        
        builder.ApplyConfiguration(new RefreshTokenConfiguration());

        builder.ApplyConfiguration(new CommentConfiguration());
        builder.ApplyConfiguration(new LikeConfiguration());
        builder.ApplyConfiguration(new PostConfiguration());
    }

    public ApplicationDbContext(DbContextOptions dbContextOptions)
        : base(dbContextOptions)
    {
    }
}