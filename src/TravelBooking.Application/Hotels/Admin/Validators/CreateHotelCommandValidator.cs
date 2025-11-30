using FluentValidation;
using TravelBooking.Application.Hotels.Commands;

public class CreateHotelCommandValidator : AbstractValidator<CreateHotelCommand>
{
    public CreateHotelCommandValidator()
    {
        RuleFor(x => x.Dto).NotNull();

        RuleFor(x => x.Dto.Name)
            .NotEmpty().WithMessage("Hotel name is required.")
            .MaximumLength(150);

        RuleFor(x => x.Dto.StarRating)
            .InclusiveBetween(1, 5)
            .WithMessage("Star rating must be between 1 and 5.");

        RuleFor(x => x.Dto.CityId)
            .NotEmpty().WithMessage("CityId is required.");

        RuleFor(x => x.Dto.OwnerId)
            .NotEmpty().WithMessage("OwnerId is required.");

        RuleFor(x => x.Dto.TotalRooms)
            .GreaterThan(0).WithMessage("Total rooms must be greater than zero.");

        RuleFor(x => x.Dto.Email)
            .NotEmpty().WithMessage("Email is required.")
            .EmailAddress().WithMessage("Invalid email format.");

        RuleFor(x => x.Dto.Location)
            .NotEmpty().WithMessage("Location is required.");

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