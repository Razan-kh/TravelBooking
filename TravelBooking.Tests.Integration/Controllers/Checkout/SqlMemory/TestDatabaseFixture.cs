public class TestDatabaseFixture : IAsyncLifetime
{
    public string ConnectionString { get; private set; } = default!;

    private readonly string _dbName = $"TravelBooking_Test_{Guid.NewGuid():N}";
    private readonly string _master = "Server=localhost;Database=master;Trusted_Connection=True;TrustServerCertificate=True;";

    public async Task InitializeAsync()
    {
        using var connection = new SqlConnection(_master);
        await connection.OpenAsync();

        using var cmd = connection.CreateCommand();
        cmd.CommandText = $"CREATE DATABASE [{_dbName}]";
        await cmd.ExecuteNonQueryAsync();

        ConnectionString = $"Server=localhost;Database={_dbName};Trusted_Connection=True;TrustServerCertificate=True;";
    }

    public async Task DisposeAsync()
    {
        using var connection = new SqlConnection(_master);
        await connection.OpenAsync();

        using var cmd = connection.CreateCommand();
        cmd.CommandText = $@"
            ALTER DATABASE [{_dbName}] SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
            DROP DATABASE [{_dbName}];
        ";
        await cmd.ExecuteNonQueryAsync();
    }
}