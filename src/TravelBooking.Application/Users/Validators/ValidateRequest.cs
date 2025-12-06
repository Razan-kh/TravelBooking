using FluentValidation;
using TravelBooking.Application.Users.Commands;

namespace TravelBooking.Application.Users.Validators;

public class LoginCommandValidator : AbstractValidator<LoginCommand>
{
    public LoginCommandValidator()
    {
        // Email validation
        RuleFor(x => x.Email)
            .Cascade(CascadeMode.Stop)
            .NotEmpty().WithMessage("Email is required").WithErrorCode("EMAIL_REQUIRED")
            .EmailAddress().WithMessage("A valid email address is required").WithErrorCode("EMAIL_INVALID")
            .MaximumLength(200).WithMessage("Email cannot exceed 200 characters").WithErrorCode("EMAIL_TOO_LONG")
            .When(x => !string.IsNullOrWhiteSpace(x.Email));

        // Password validation
        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Password is required")
            .MinimumLength(8).WithMessage("Password must be at least 8 characters")
            .MaximumLength(100).WithMessage("Password is too long");
    }
}