using Dapper;

namespace OrderManager.ReadModel.Api.Orders;

public class OrderRepository : IOrderRepository
{
    private readonly INpgsqlConnectionFactory _connectionFactory;

    public OrderRepository(INpgsqlConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<Order?> Get(Guid orderId)
    {
        await using var connection = _connectionFactory.CreateConnection();

        var order = await connection.QuerySingleOrDefaultAsync<Order>(
            """
            SELECT id Id, 
                   product_name ProductName, 
                   delivery_address DeliveryAddress, 
                   status Status,
                   is_archived IsArchived,
                   created_at CreatedAt,
                   last_modified_at LastModifiedAt
            FROM orders 
            WHERE id = @id
            """,
            new { id = orderId });
        return order;
    }

    public async Task CreateOrUpdate(Order order)
    {
        await using var connection = _connectionFactory.CreateConnection();
        await connection.ExecuteAsync(
            """
            INSERT INTO orders (id, product_name, delivery_address, status, is_archived, created_at, last_modified_at) 
            VALUES (@Id, @ProductName, @DeliveryAddress, @Status, @IsArchived, @CreatedAt, @LastModifiedAt)
            ON CONFLICT (id) DO UPDATE SET
                product_name = @ProductName,
                delivery_address = @DeliveryAddress,
                status = @Status,
                is_archived = @IsArchived,
                last_modified_at = @LastModifiedAt
            """,
            order);
    }

    public async Task<Order[]> GetAll()
    {
        await using var connection = _connectionFactory.CreateConnection();
        var orders = await connection.QueryAsync<Order>(
            """
            SELECT id Id, 
                   product_name ProductName, 
                   delivery_address DeliveryAddress, 
                   status Status,
                   is_archived IsArchived,
                   created_at CreatedAt,
                   last_modified_at LastModifiedAt
            FROM orders
            """);
        return orders.ToArray();
    }
}