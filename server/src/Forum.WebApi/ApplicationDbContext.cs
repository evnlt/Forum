using Forum.WebApi.Configuration;
using Forum.WebApi.Entities;
using Microsoft.EntityFrameworkCore;

namespace Forum.WebApi;

public class ApplicationDbContext : DbContext
{
    public DbSet<Post> Posts { get; init; } = default!;

    public DbSet<Comment> Comments { get; init; } = default!;

    public DbSet<Like> Likes { get; init; } = default!;
    
    // TODO - switch to microsoft identity?
    public DbSet<User> Users { get; init; } = default!;

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.ApplyConfiguration(new CommentConfiguration());
        builder.ApplyConfiguration(new LikeConfiguration());
        builder.ApplyConfiguration(new PostConfiguration());
        builder.ApplyConfiguration(new UserConfiguration());
    }

    public ApplicationDbContext(DbContextOptions dbContextOptions)
        : base(dbContextOptions)
    {
    }
}