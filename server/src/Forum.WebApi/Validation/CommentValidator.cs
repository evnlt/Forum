using FluentValidation;
using Forum.WebApi.Requests;

namespace Forum.WebApi.Validation;

public class CreateCommentRequestValidator : AbstractValidator<CreateCommentRequest>
{
    public CreateCommentRequestValidator()
    {
        RuleFor(x => x.Message)
            .NotNull()
            .NotEmpty();
    }
}

public class UpdateCommentRequestValidator : AbstractValidator<UpdateCommentRequest>
{
    public UpdateCommentRequestValidator()
    {
        RuleFor(x => x.Message)
            .NotNull()
            .NotEmpty();
    }
}