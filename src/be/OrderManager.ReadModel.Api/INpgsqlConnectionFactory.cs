using Npgsql;

namespace OrderManager.ReadModel.Api;

public interface INpgsqlConnectionFactory
{
    NpgsqlConnection CreateConnection();
}