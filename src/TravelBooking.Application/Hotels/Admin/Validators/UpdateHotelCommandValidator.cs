using FluentValidation;
using TravelBooking.Application.Hotels.Commands;

public class UpdateHotelCommandValidator : AbstractValidator<UpdateHotelCommand>
{
    public UpdateHotelCommandValidator()
    {
        RuleFor(x => x.Dto).NotNull();

        RuleFor(x => x.Dto.Id)
            .NotEmpty().WithMessage("Hotel Id is required.");

        RuleFor(x => x.Dto.Name)
            .NotEmpty().WithMessage("Hotel name is required.")
            .MaximumLength(150);

        RuleFor(x => x.Dto.StarRating)
            .InclusiveBetween(1, 5);

        RuleFor(x => x.Dto.CityId)
            .NotEmpty().WithMessage("CityId is required.");

        RuleFor(x => x.Dto.OwnerId)
            .NotEmpty().WithMessage("OwnerId is required.");

        RuleFor(x => x.Dto.TotalRooms)
            .GreaterThan(0).WithMessage("Total rooms must be greater than zero.");

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