using FluentValidation;
using TravelBooking.Application.Cities.Commands;

namespace TravelBooking.Application.Cities.Validators;

public class CreateCityCommandValidator : AbstractValidator<CreateCityCommand>
{
    public CreateCityCommandValidator()
    {
        RuleFor(x => x.Dto).NotNull();

        RuleFor(x => x.Dto.Name)
            .NotEmpty().WithMessage("City name is required.")
            .MaximumLength(100).WithMessage("City name cannot exceed 100 characters.");

        RuleFor(x => x.Dto.Country)
            .NotEmpty().WithMessage("Country is required.")
            .MaximumLength(100);

        RuleFor(x => x.Dto.PostalCode)
            .NotEmpty().WithMessage("Postal code is required.")
            .Matches(@"^[A-Za-z0-9\- ]+$")
            .WithMessage("Postal code contains invalid characters.");

        RuleFor(x => x.Dto.ThumbnailUrl)
            .Must(BeValidUrl)
            .When(x => !string.IsNullOrWhiteSpace(x.Dto.ThumbnailUrl))
            .WithMessage("Thumbnail URL must be a valid URL.");
    }

    private bool BeValidUrl(string? url)
    {
        return Uri.TryCreate(url, UriKind.Absolute, out _);
    }
}
