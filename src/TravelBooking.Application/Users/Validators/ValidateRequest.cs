using FluentValidation;
using TravelBooking.Application.Users.Commands;
using TravelBooking.Application.Users.Validators;

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
            .Must(BeAValidDomain).WithMessage("Email domain is not valid").WithErrorCode("EMAIL_DOMAIN_INVALID")
            .When(x => !string.IsNullOrWhiteSpace(x.Email));

        // Password validation
        RuleFor(x => x.Password)
            .Cascade(CascadeMode.Stop)
            .NotEmpty().WithMessage("Password is required").WithErrorCode("PASSWORD_REQUIRED")
            .MinimumLength(8).WithMessage("Password must be at least 8 characters").WithErrorCode("PASSWORD_TOO_SHORT")
            .MaximumLength(100).WithMessage("Password cannot exceed 100 characters").WithErrorCode("PASSWORD_TOO_LONG")
            .Matches(@"^(?=.*[a-z])").WithMessage("Password must contain at least one lowercase letter").WithErrorCode("PASSWORD_NO_LOWERCASE")
            .Matches(@"^(?=.*[A-Z])").WithMessage("Password must contain at least one uppercase letter").WithErrorCode("PASSWORD_NO_UPPERCASE")
            .Matches(@"^(?=.*\d)").WithMessage("Password must contain at least one number").WithErrorCode("PASSWORD_NO_NUMBER")
            .Matches(@"^(?=.*[^\da-zA-Z])").WithMessage("Password must contain at least one special character").WithErrorCode("PASSWORD_NO_SPECIAL")
            .When(x => !string.IsNullOrWhiteSpace(x.Password));
    }

    private bool BeAValidDomain(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
        {
            return false;
        }

        try
        {
            var domain = email.Split('@').Last();
            return !domain.Contains("example.com"); // Block example.com domains
        }
        catch
        {
            return false;
        }
    }
}