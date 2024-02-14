namespace Forum.WebApi.Requests;

public class CreateCommentRequest
{
    public string? Message { get; init; }

    public Guid? ParentId { get; init; }
}