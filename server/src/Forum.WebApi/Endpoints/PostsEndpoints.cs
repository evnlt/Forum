using Forum.WebApi.Entities;
using Forum.WebApi.Extentions;
using Forum.WebApi.Filters;
using Forum.WebApi.Repositories;
using Forum.WebApi.Requests;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Forum.WebApi.Endpoints;

public static class PostsEndpoints
{
    public static void MapPostsEndpoints(this WebApplication app)
    {
        app.MapGet("api/posts", GetPosts);
        app.MapGet("api/posts/{id}", GetPost);
        app.MapPost("api/posts/{postId}/comments", CreateComment).AddEndpointFilter<ValidationFilter<CreateCommentRequest>>();
        app.MapPut("api/posts/{postId}/comments/{id}", UpdateComment).AddEndpointFilter<ValidationFilter<UpdateCommentRequest>>();
        app.MapDelete("api/posts/{postId}/comments/{id}", DeleteComment);
        app.MapPost("api/posts/{postId}/comments/{commentId}/toggleLike", ToggleCommentLike);
    }
    
    public static async Task<IResult> GetPosts(IPostRepository postRepository, CancellationToken cancellationToken)
    {
        var posts = await postRepository.GetAll(cancellationToken);
        return Results.Ok(posts);
    }
    
    public static async Task<IResult> GetPost(
        HttpContext httpContext, 
        IPostRepository postRepository, 
        Guid id,
        CancellationToken cancellationToken)
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
    public static async Task<IResult> CreateComment(
        HttpContext httpContext, 
        ICommentRepository commentRepository, 
        Guid postId, 
        [FromBody] CreateCommentRequest request, 
        CancellationToken cancellationToken)
    {
        var userId = httpContext.GetCurrentUserId();

        var comment = new Comment
        {
            Message = request.Message!,
            ParentId = request.ParentId,
            PostId = postId,
            UserId = userId
        };

        await commentRepository.Add(comment, cancellationToken);

        return Results.Ok(comment);
    }
    
    [Authorize]
    public static async Task<IResult> UpdateComment(
        ICommentRepository commentRepository, 
        Guid postId, 
        Guid id, 
        [FromBody] UpdateCommentRequest request,
        CancellationToken cancellationToken)
    {
        var comment = await commentRepository.Update(id, request.Message!, cancellationToken);

        return Results.Ok(comment);
    }
    
    [Authorize]
    public static async Task<IResult> DeleteComment(
        HttpContext httpContext, 
        ICommentRepository commentRepository,
        Guid postId, 
        Guid id,
        CancellationToken cancellationToken)
    {
        await commentRepository.Delete(id, cancellationToken);

        return Results.NoContent();
    }
    
    [Authorize]
    public static async Task<IResult> ToggleCommentLike(
        HttpContext httpContext, 
        ICommentRepository commentRepository, 
        Guid postId, 
        Guid commentId, 
        CancellationToken cancellationToken)
    {
        var userId = httpContext.GetCurrentUserId();

        var liked = await commentRepository.ToggleLike(commentId, userId, cancellationToken);

        return Results.Ok(liked);
    }
}