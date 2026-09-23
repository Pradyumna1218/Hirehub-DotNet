using FluentValidation;
using Hirehub.Application.DTOs.Jobs;

namespace Hirehub.Application.Validators;

public class JobCreateRequestValidator : AbstractValidator<JobCreateRequest>
{
    public JobCreateRequestValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Title is required.")
            .MaximumLength(150);

        RuleFor(x => x.Description)
            .NotEmpty().WithMessage("Description is required.")
            .MaximumLength(4000);

        RuleFor(x => x.Budget)
            .GreaterThan(0).WithMessage("Budget must be greater than zero.");

        RuleFor(x => x.Deadline)
            .GreaterThan(DateTime.UtcNow).WithMessage("Deadline must be in the future.");

        RuleFor(x => x.CategoryId)
            .GreaterThan(0).WithMessage("A valid category must be selected.");
    }
}