using Microsoft.Extensions.DependencyInjection;
using TravelBooking.Application.Cheackout.Servicies.Interfaces;
using TravelBooking.Infrastructure.Persistence;
using TravelBooking.Tests.Integration.Factories;
using Xunit;
using TravelBooking.Application.Shared.Interfaces;

namespace BookingSystem.IntegrationTests.Checkout.Utils;

public class CheckoutTestFixture : IAsyncLifetime
{
    public ApiTestFactory Factory { get; private set; }
    public Guid TestUserId { get; private set; }
    public TestEmailService EmailService { get; private set; } = null!;
    public TestPaymentService PaymentService { get; private set; } = null!;
    public TestPdfService PdfService { get; private set; } = null!;
    public CheckoutTestFixture()
    {
        TestUserId = Guid.NewGuid();
    }

    public async Task InitializeAsync()
    {
        EmailService = new TestEmailService();
        PaymentService = new TestPaymentService();
        PdfService = new TestPdfService();

        Factory = new ApiTestFactory();
        Factory.SetInMemoryDbName($"CheckoutTests_{Guid.NewGuid()}");

        // Override external services with test implementations
        Factory.AddServiceConfiguration(services =>
        {
            // Remove existing registrations if they exist
            var emailDescriptor = services.FirstOrDefault(d => d.ServiceType == typeof(IEmailService));
            if (emailDescriptor != null) services.Remove(emailDescriptor);
            
            var paymentDescriptor = services.FirstOrDefault(d => d.ServiceType == typeof(IPaymentService));
            if (paymentDescriptor != null) services.Remove(paymentDescriptor);
            
            var pdfDescriptor = services.FirstOrDefault(d => d.ServiceType == typeof(IPdfService));
            if (pdfDescriptor != null) services.Remove(pdfDescriptor);

            // Register test implementations
            services.AddSingleton<IEmailService, TestEmailService>();
            services.AddSingleton<IPaymentService, TestPaymentService>();
            services.AddSingleton<IPdfService, TestPdfService>();
            
            // Register InMemoryUnitOfWork
            services.AddScoped<IUnitOfWork>(provider =>
            {
                var context = provider.GetRequiredService<AppDbContext>();
                return new InMemoryUnitOfWork(context);
            });
        });
    }

    public async Task DisposeAsync()
    {
        if (Factory != null)
        {
            await Factory.DisposeAsync();
        }
    }
}