using FluentValidation;
using Hirehub.Application.DTOs.Proposals;

namespace Hirehub.Application.Validators;

public class ProposalCreateRequestValidator : AbstractValidator<ProposalCreateRequest>
{
    public ProposalCreateRequestValidator()
    {
        RuleFor(x => x.JobId)
            .GreaterThan(0).WithMessage("A valid job must be specified.");

        RuleFor(x => x.CoverLetter)
            .NotEmpty().WithMessage("A cover letter is required.")
            .MaximumLength(4000);

        RuleFor(x => x.ProposedPrice)
            .GreaterThan(0).WithMessage("Proposed price must be greater than zero.");

        RuleFor(x => x.DeliveryDays)
            .GreaterThan(0).WithMessage("Delivery days must be at least 1.");
    }
}