using Npgsql;
using Marketplace.Data.Database;
using Marketplace.Data.Interfaces;
using Marketplace.Entities.Entities;

namespace Marketplace.Data.Repositories
{
    public class CartRepository : ICartRepository
    {
        private readonly DatabaseConnection _db;

        public CartRepository(DatabaseConnection db)
        {
            _db = db;
        }

        public void Update(CartItem item)
        {
            using var connection = _db.GetConnection();
            connection.Open();

            using var command = new NpgsqlCommand(@"
                UPDATE cartitems
                SET quantity = @quantity
                WHERE id = @id
            ", connection);

            command.Parameters.AddWithValue("@quantity", item.Quantity);
            command.Parameters.AddWithValue("@id", item.Id);

            command.ExecuteNonQuery();
        }

        public IEnumerable<CartItem> GetByUser(int userId)
        {
            var items = new List<CartItem>();

            using var connection = _db.GetConnection();
            using var command = new NpgsqlCommand(
                "SELECT * FROM cartitems WHERE cartid = @userid",
                connection
            );

            command.Parameters.AddWithValue("@userid", userId);

            connection.Open();
            using var reader = command.ExecuteReader();

            while (reader.Read())
            {
                items.Add(new CartItem
                {
                    Id = reader.GetInt32(reader.GetOrdinal("id")),
                    CartId = reader.GetInt32(reader.GetOrdinal("cartid")),
                    ProductId = reader.GetInt32(reader.GetOrdinal("productid")),
                    Quantity = reader.GetInt32(reader.GetOrdinal("quantity")),
                    UnitPrice = reader.GetDecimal(reader.GetOrdinal("unitprice"))
                });
            }

            return items;
        }

        public void Add(CartItem item)
        {
            using var connection = _db.GetConnection();
            connection.Open();

            using var checkCmd = new NpgsqlCommand(@"
                SELECT id, quantity
                FROM cartitems
                WHERE cartid = @cartid AND productid = @productid
            ", connection);

            checkCmd.Parameters.AddWithValue("@cartid", item.CartId);
            checkCmd.Parameters.AddWithValue("@productid", item.ProductId);

            using var reader = checkCmd.ExecuteReader();

            if (reader.Read())
            {
                var id = reader.GetInt32(0);
                var qty = reader.GetInt32(1);
                reader.Close();

                using var updateCmd = new NpgsqlCommand(@"
                    UPDATE cartitems
                    SET quantity = @qty
                    WHERE id = @id
                ", connection);

                updateCmd.Parameters.AddWithValue("@qty", qty + item.Quantity);
                updateCmd.Parameters.AddWithValue("@id", id);

                updateCmd.ExecuteNonQuery();
            }
            else
            {
                reader.Close();

                using var insertCmd = new NpgsqlCommand(@"
                    INSERT INTO cartitems (cartid, productid, quantity, unitprice)
                    VALUES (@cartid, @productid, @quantity, @unitprice)
                ", connection);

                insertCmd.Parameters.AddWithValue("@cartid", item.CartId);
                insertCmd.Parameters.AddWithValue("@productid", item.ProductId);
                insertCmd.Parameters.AddWithValue("@quantity", item.Quantity);
                insertCmd.Parameters.AddWithValue("@unitprice", item.UnitPrice);

                insertCmd.ExecuteNonQuery();
            }
        }

        public void UpdateQuantity(int id, int quantity)
        {
            using var connection = _db.GetConnection();
            connection.Open();

            using var command = new NpgsqlCommand(
                "UPDATE cartitems SET quantity = @quantity WHERE id = @id",
                connection
            );

            command.Parameters.AddWithValue("@id", id);
            command.Parameters.AddWithValue("@quantity", quantity);

            command.ExecuteNonQuery();
        }

        public void Remove(int id)
        {
            using var connection = _db.GetConnection();
            connection.Open();

            using var command = new NpgsqlCommand(
                "DELETE FROM cartitems WHERE id = @id",
                connection
            );

            command.Parameters.AddWithValue("@id", id);

            command.ExecuteNonQuery();
        }
    }
}
