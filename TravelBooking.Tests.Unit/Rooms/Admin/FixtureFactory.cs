using AutoFixture;
using AutoFixture.AutoMoq;
using AutoFixture.Kernel;

namespace TravelBooking.Tests.Rooms;

public static class FixtureFactory
{
    public static IFixture Create()
    {
        var fixture = new Fixture();

        fixture.Customize(new AutoMoqCustomization { ConfigureMembers = false });

        // Avoid recursion exceptions 
        fixture.Behaviors.OfType<ThrowingRecursionBehavior>().ToList()
            .ForEach(b => fixture.Behaviors.Remove(b));
        fixture.Behaviors.Add(new OmitOnRecursionBehavior());
        fixture.Customizations.Add(new TypeRelay(typeof(ICollection<>), typeof(List<>)));

        return fixture;
    }
}