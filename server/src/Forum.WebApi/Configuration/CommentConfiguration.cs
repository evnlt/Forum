using Forum.WebApi.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Forum.WebApi.Configuration;
 
public class CommentConfiguration : IEntityTypeConfiguration<Comment>
{
    public void Configure(EntityTypeBuilder<Comment> builder)
    {
        builder.ToTable("Comments");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.CreatedAt).HasConversion(x => x, x => new DateTime(x.Ticks, DateTimeKind.Utc));
        builder.Property(x => x.UpdatedAt).HasConversion(x => x, x => new DateTime(x.Ticks, DateTimeKind.Utc));

        builder.HasOne(x => x.User).WithMany().HasForeignKey(x => x.UserId);
        builder.HasOne<Post>().WithMany( x=> x.Comments).HasForeignKey(x => x.PostId);
        builder.HasOne( x=> x.Parent).WithMany().HasForeignKey(x => x.ParentId);
    }
}