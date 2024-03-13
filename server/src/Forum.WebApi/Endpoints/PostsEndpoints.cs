using Forum.WebApi.Entities;
using Forum.WebApi.Extentions;
using Forum.WebApi.Repositories;
using Forum.WebApi.Requests;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Forum.WebApi.Endpoints;

public static class PostsEndpoints
{
    public static async Task<IResult> GetPosts(IPostRepository postRepository, CancellationToken cancellationToken)
    {
        var posts = await postRepository.GetAll(cancellationToken);
        return Results.Ok(posts);
    }
    
    public static async Task<IResult> GetPost(HttpContext httpContext, IPostRepository postRepository, CancellationToken cancellationToken, Guid id)
    {
        var userId = httpContext.GetCurrentUserId();

        var post = await postRepository.Get(id, userId, cancellationToken);

        if (post is null)
        {
            return Results.NotFound();
        }

        return Results.Ok(post);
    }
    
    [Authorize]
    public static async Task<IResult> CreateComment(HttpContext httpContext, ICommentRepository commentRepository, CancellationToken cancellationToken, Guid postId, [FromBody] CreateCommentRequest request)
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

        await commentRepository.Add(comment, cancellationToken);

        return Results.Ok(comment);
    }
    
    [Authorize]
    public static async Task<IResult> UpdateComment(ICommentRepository commentRepository, CancellationToken cancellationToken, Guid postId, Guid id, [FromBody] UpdateCommentRequest request)
    {
        if (request.Message is null or "")
        {
            return Results.BadRequest();
        }

        var comment = await commentRepository.Update(id, request.Message, cancellationToken);

        return Results.Ok(comment);
    }
    
    [Authorize]
    public static async Task<IResult> DeleteComment(HttpContext httpContext, ICommentRepository commentRepository, CancellationToken cancellationToken, Guid postId, Guid id)
    {
        await commentRepository.Delete(id, cancellationToken);

        return Results.NoContent();
    }
    
    [Authorize]
    public static async Task<IResult> ToggleCommentLike(HttpContext httpContext, ICommentRepository commentRepository, CancellationToken cancellationToken, Guid postId, Guid commentId)
    {
        var userId = httpContext.GetCurrentUserId();

        var liked = await commentRepository.ToggleLike(commentId, userId, cancellationToken);

        return Results.Ok(liked);
    }
}