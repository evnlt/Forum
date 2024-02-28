using Forum.WebApi.Entities;
using Forum.WebApi.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Forum.WebApi.Configuration;

public class RefreshTokenConfiguration : IEntityTypeConfiguration<RefreshToken>
{
    public void Configure(EntityTypeBuilder<RefreshToken> builder)
    {
        builder.ToTable("RefreshTokens");

        builder.HasKey(x => x.Value);
        
        builder.Property(x => x.CreatedAt).HasConversion(x => x, x => new DateTime(x.Ticks, DateTimeKind.Utc));
        builder.Property(x => x.ExpiresAt).HasConversion(x => x, x => new DateTime(x.Ticks, DateTimeKind.Utc));
    }
}