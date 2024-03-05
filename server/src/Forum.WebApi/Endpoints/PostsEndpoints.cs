using Forum.WebApi.Entities;
using Forum.WebApi.Extentions;
using Forum.WebApi.Models;
using Forum.WebApi.Requests;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Forum.WebApi.Endpoints;

public static class PostsEndpoints
{
    public static async Task<IEnumerable<PostBriefDto>> GetPosts(HttpContext httpContext, ApplicationDbContext applicationDbContext, CancellationToken cancellationToken)
    {
        var testUserId = httpContext.TryGetCurrentUserId();
        
        return await applicationDbContext.Posts
            .AsNoTracking()
            .Select(x => new PostBriefDto { Id = x.Id, Title = x.Title })
            .ToListAsync(cancellationToken);
    }
    
    public static async Task<IResult?> GetPost(HttpContext httpContext, ApplicationDbContext applicationDbContext, CancellationToken cancellationToken, Guid id)
    {
        var userId = httpContext.TryGetCurrentUserId();
        
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

        if (post is null)
        {
            return Results.NotFound();
        }

        return Results.Ok(post);
    }
    
    [Authorize]
    public static async Task<IResult> CreateComment(HttpContext httpContext, ApplicationDbContext applicationDbContext, CancellationToken cancellationToken, Guid postId, [FromBody] CreateCommentRequest request)
    {
        if (request.Message is null or "")
        {
            return Results.BadRequest();
        }
        
        var userId = httpContext.GetCurrentUserId();

        var comment = new Comment
        {
            Message = request.Message,
            ParentId = request.ParentId,
            PostId = postId,
            UserId = userId
        };

        applicationDbContext.Comments.Add(comment);
        await applicationDbContext.SaveChangesAsync(cancellationToken);

        comment.User = (await applicationDbContext.Users
            .FirstOrDefaultAsync(x => x.Id == comment.UserId, cancellationToken))!;

        return Results.Ok(comment);
    }
    
    [Authorize]
    public static async Task<IResult> UpdateComment(HttpContext httpContext, ApplicationDbContext applicationDbContext, CancellationToken cancellationToken, Guid postId, Guid id, [FromBody] UpdateCommentRequest request)
    {
        if (request.Message is null or "")
        {
            return Results.BadRequest();
        }

        var comment = await applicationDbContext.Comments
            .Where(x => x.Id == id)
            .FirstOrDefaultAsync(cancellationToken);

        if (comment is null)
        {
            return Results.NotFound();
        }

        comment.Message = request.Message;
        comment.UpdatedAt = DateTime.UtcNow;

        await applicationDbContext.SaveChangesAsync(cancellationToken);

        return Results.Ok(comment);
    }
    
    [Authorize]
    public static async Task<IResult> DeleteComment(HttpContext httpContext, ApplicationDbContext applicationDbContext, CancellationToken cancellationToken, Guid postId, Guid id)
    {
        var comment = await applicationDbContext.Comments
            .Where(x => x.Id == id)
            .FirstOrDefaultAsync(cancellationToken);

        if (comment is null)
        {
            return Results.NotFound();
        }

        applicationDbContext.Comments.Remove(comment);
        await applicationDbContext.SaveChangesAsync(cancellationToken);

        return Results.Ok();
    }
    
    [Authorize]
    public static async Task<IResult> ToggleCommentLike(HttpContext httpContext, ApplicationDbContext applicationDbContext, CancellationToken cancellationToken, Guid postId, Guid commentId)
    {
        var userId = httpContext.TryGetCurrentUserId();
        
        var comment = await applicationDbContext.Comments
            .Where(x => x.Id == commentId)
            .Include(x => x.Likes)
            .FirstOrDefaultAsync(cancellationToken);

        if (comment is null)
        {
            return Results.NotFound();
        }

        var like = comment.Likes.FirstOrDefault(x => x.UserId == userId);
        var value = false;
        if (like is null)
        {
            like = new Like
            {
                UserId = userId!.Value,
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

        return Results.Ok(value);
    }
}