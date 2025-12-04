public class TestApplicationFactory : WebApplicationFactory<Program>
{
    private readonly string _testConnection;

    public TestApplicationFactory(string testConnection)
    {
        _testConnection = testConnection;
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            var descriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(DbContextOptions<AppDbContext>));

            if (descriptor != null)
                services.Remove(descriptor);

            services.AddDbContext<AppDbContext>(options =>
            {
                options.UseSqlServer(_testConnection);
            });
        });
    }
}
