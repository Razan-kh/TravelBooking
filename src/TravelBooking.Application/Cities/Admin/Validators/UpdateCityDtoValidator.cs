using FluentValidation;
using TravelBooking.Application.Cities.Dtos;

public class UpdateCityDtoValidator : AbstractValidator<UpdateCityDto>
{
    public UpdateCityDtoValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("City ID is required.");

        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("City name is required.")
            .MaximumLength(100);

        RuleFor(x => x.Country)
            .NotEmpty().WithMessage("Country is required.")
            .MaximumLength(100);

        RuleFor(x => x.PostalCode)
            .NotEmpty().WithMessage("Postal code is required.")
            .Matches(@"^[A-Za-z0-9\- ]+$")
            .WithMessage("Postal code contains invalid characters.");

        RuleFor(x => x.ThumbnailUrl)
            .Must(BeValidUrl)
            .When(x => !string.IsNullOrWhiteSpace(x.ThumbnailUrl))
            .WithMessage("Thumbnail URL must be a valid URL.");
    }

    private bool BeValidUrl(string? url)
    {
        return Uri.TryCreate(url, UriKind.Absolute, out _);
    }
}