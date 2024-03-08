using Forum.WebApi.Models;
using Microsoft.EntityFrameworkCore;

namespace Forum.WebApi.Repositories;

public interface IPostRepository
{
    Task<object> Get(Guid id, Guid userId, CancellationToken cancellationToken);
    
    Task<ICollection<PostBriefDto>> GetAll(CancellationToken cancellationToken);
}

public class PostRepository(ApplicationDbContext applicationDbContext) : IPostRepository
{
    // TODO - remove object 
    public async Task<object> Get(Guid id, Guid userId, CancellationToken cancellationToken)
    {
        var post = await applicationDbContext.Posts
            .AsNoTracking()
            .Include(x => x.Comments)
            .ThenInclude(c => c.User)
            .Include(x => x.Comments)
            .ThenInclude(c => c.Likes)
            .Select(x => new
            {
                x.Id,
                x.Title,
                x.Body,
                Comments = x.Comments.Select(comment  => new
                {
                    comment.Id, 
                    comment.CreatedAt, 
                    comment.ParentId, 
                    comment.Message,  
                    comment.User,
                    LikeCount = comment.Likes.Count,
                    LikedByMe = comment.Likes.Any(like => like.UserId == userId)
                })
            })
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

        return post!;
    }

    public async Task<ICollection<PostBriefDto>> GetAll(CancellationToken cancellationToken)
    {
        return await applicationDbContext.Posts
            .AsNoTracking()
            .Select(x => new PostBriefDto { Id = x.Id, Title = x.Title })
            .ToListAsync(cancellationToken);
    }
}