using Dapper;

namespace OrderManager.ReadModel.Api;

public class DatabaseMigrator
{
    private readonly INpgsqlConnectionFactory _connectionFactory;

    public DatabaseMigrator(INpgsqlConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task Migrate()
    {
        await using var connection = _connectionFactory.CreateConnection();
        var sql = await File.ReadAllTextAsync("CreateDatabase.sql");
        await connection.ExecuteAsync(sql);
    }
}