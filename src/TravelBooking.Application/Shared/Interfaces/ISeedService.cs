namespace TravelBooking.Application.Interfaces;

public interface ISeedService
{
    Task SeedAsync(CancellationToken cancellationToken = default);
}
