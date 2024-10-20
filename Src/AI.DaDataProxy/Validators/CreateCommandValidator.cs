using AI.DaDataProxy.Http.Contracts;
using FluentValidation;

namespace AI.DaDataProxy.Validators;

/// <summary>
/// Validator for the CreateDaDataCommand.
/// </summary>
public class CreateCommandValidator : AbstractValidator<CreateDaDataCommand>
{
    /// <summary>
    /// Initializes a new instance of the CreateCommandValidator class.
    /// Sets up validation rules for the CreateDaDataCommand.
    /// </summary>
    public CreateCommandValidator()
    {
        RuleFor(command => command.Name)
            .NotEmpty().WithMessage("Name is required.")
            .Length(5, 150).WithMessage("Name must be between 5 and 150 characters.");
    }
}