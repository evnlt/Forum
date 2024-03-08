using Forum.WebApi.Entities;
using Forum.WebApi.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace Forum.WebApi.Repositories;

public interface ICommentRepository
{
    Task Add(Comment comment, CancellationToken cancellationToken);
    
    Task<Comment> Update(Guid id, string message, CancellationToken cancellationToken);

    Task Delete(Guid id, CancellationToken cancellationToken);

    Task<bool> ToggleLike(Guid commentId, Guid userId, CancellationToken cancellationToken);
}

public class CommentRepository(ApplicationDbContext applicationDbContext) : ICommentRepository
{
    public async Task Add(Comment comment, CancellationToken cancellationToken)
    {
        applicationDbContext.Comments.Add(comment);
        await applicationDbContext.SaveChangesAsync(cancellationToken);
        
        comment.User = (await applicationDbContext.Users
            .FirstOrDefaultAsync(x => x.Id == comment.UserId, cancellationToken))!;
    }

    public async Task<Comment> Update(Guid id, string message, CancellationToken cancellationToken)
    {
        var comment = await applicationDbContext.Comments
            .Where(x => x.Id == id)
            .FirstOrDefaultAsync(cancellationToken);

        if (comment is null)
        {
            throw new NotFoundException();
        }

        comment.Message = message;
        comment.UpdatedAt = DateTime.UtcNow;

        await applicationDbContext.SaveChangesAsync(cancellationToken);

        return comment;
    }

    public async Task Delete(Guid id, CancellationToken cancellationToken)
    {
        var comment = await applicationDbContext.Comments
            .Where(x => x.Id == id)
            .FirstOrDefaultAsync(cancellationToken);

        if (comment is null)
        {
            throw new NotFoundException();
        }

        applicationDbContext.Comments.Remove(comment);
        await applicationDbContext.SaveChangesAsync(cancellationToken);
    }

    // TODO - refactor return value
    public async Task<bool> ToggleLike(Guid commentId, Guid userId, CancellationToken cancellationToken)
    {
        var comment = await applicationDbContext.Comments
            .Where(x => x.Id == commentId)
            .Include(x => x.Likes)
            .FirstOrDefaultAsync(cancellationToken);

        if (comment is null)
        {
            throw new NotFoundException();
        }

        var like = comment.Likes.FirstOrDefault(x => x.UserId == userId);
        var value = false;
        if (like is null)
        {
            like = new Like
            {
                UserId = userId,
                CommentId = commentId
            };
            applicationDbContext.Likes.Add(like);
            value = true;
        }
        else
        {
            applicationDbContext.Likes.Remove(like);
        }

        await applicationDbContext.SaveChangesAsync(cancellationToken);

        return value;
    }
}