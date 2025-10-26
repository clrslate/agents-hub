using ClrSlate.Modules.AgentsAppModule.Models;
using FluentValidation;

namespace ClrSlate.Modules.AgentsAppModule.Validators;

public sealed class UpdateAgentRequestValidator : AbstractValidator<UpdateAgentRequest>
{
    public UpdateAgentRequestValidator()
    {
        RuleFor(x => x.Model)
            .NotEmpty();

        RuleFor(x => x.Version)
            .GreaterThan(0);

        RuleFor(x => x.Instructions)
            .MaximumLength(16000)
            .When(x => x.Instructions is not null);

        RuleFor(x => x.DisplayName)
            .MaximumLength(128)
            .When(x => x.DisplayName is not null);

        RuleFor(x => x.Description)
            .MaximumLength(2048)
            .When(x => x.Description is not null);
    }
}
