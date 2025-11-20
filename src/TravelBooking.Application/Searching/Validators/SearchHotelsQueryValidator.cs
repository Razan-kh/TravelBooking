using FluentValidation;
using TravelBooking.Application.Queries;

namespace TravelBooking.Application.Searching.Validators;

public class SearchHotelsQueryValidator : AbstractValidator<SearchHotelsQuery>
{
    public SearchHotelsQueryValidator()
    {
        RuleFor(x => x.Adults).GreaterThan(0);
        RuleFor(x => x.PageSize).InclusiveBetween(1, 100);

        RuleFor(x => x.CheckOut)
            .Must((query, checkOut) =>
                checkOut == null || query.CheckIn == null || checkOut > query.CheckIn)
            .WithMessage("CheckOut must be after CheckIn.");
    }
}