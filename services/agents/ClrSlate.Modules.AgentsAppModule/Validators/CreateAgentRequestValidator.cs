using System.Text.RegularExpressions;
using ClrSlate.Modules.AgentsAppModule.Models;
using FluentValidation;

namespace ClrSlate.Modules.AgentsAppModule.Validators;

public sealed class CreateAgentRequestValidator : AbstractValidator<CreateAgentRequest>
{
    private static readonly Regex SlugPattern = new("^[a-z0-9]+(?:-[a-z0-9]+)*$", RegexOptions.Compiled);

    public CreateAgentRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(64)
            .Must(v => SlugPattern.IsMatch(v))
            .WithMessage("Name must be a lowercase kebab-case slug");

        RuleFor(x => x.Model)
            .NotEmpty();

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
