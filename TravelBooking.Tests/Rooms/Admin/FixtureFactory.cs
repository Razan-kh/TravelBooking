using AutoFixture;
using AutoFixture.AutoMoq;
using AutoFixture.Kernel;

namespace TravelBooking.Tests.Rooms;

public static class FixtureFactory
{
    public static IFixture Create()
    {
        var fixture = new Fixture();

        // Use AutoMoq but don't auto-configure mock internals (safer)
        fixture.Customize(new AutoMoqCustomization { ConfigureMembers = false });

        // Avoid recursion exceptions (circular references from EF entities)
        fixture.Behaviors.OfType<ThrowingRecursionBehavior>().ToList()
            .ForEach(b => fixture.Behaviors.Remove(b));
        fixture.Behaviors.Add(new OmitOnRecursionBehavior());

        // Replace ICollection<T> with List<T> so AutoFixture can populate navigation props safely
        fixture.Customizations.Add(new TypeRelay(typeof(ICollection<>), typeof(List<>)));

        // Example stable defaults for DTOs or other types can be registered here if needed
        return fixture;
    }
}