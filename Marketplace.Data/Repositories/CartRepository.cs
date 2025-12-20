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

        public IEnumerable<CartItem> GetByUser(int userId)
        {
            var items = new List<CartItem>();

            using var connection = _db.GetConnection();
            connection.Open();

            using var command = new NpgsqlCommand(
                @"SELECT id, cart_id, product_id, quantity, unit_price
                  FROM cart_items
                  WHERE cart_id = @cart_id",
                connection
            );

            command.Parameters.AddWithValue("cart_id", userId);

            using var reader = command.ExecuteReader();

            while (reader.Read())
            {
                items.Add(new CartItem
                {
                    Id = reader.GetInt32(0),
                    CartId = reader.GetInt32(1),
                    ProductId = reader.GetInt32(2),
                    Quantity = reader.GetInt32(3),
                    UnitPrice = reader.GetDecimal(4)
                });
            }

            return items;
        }

        public void Add(CartItem item)
        {
            using var connection = _db.GetConnection();
            connection.Open();

            using var checkCmd = new NpgsqlCommand(
                @"SELECT id, quantity
                  FROM cart_items
                  WHERE cart_id = @cart_id AND product_id = @product_id",
                connection
            );

            checkCmd.Parameters.AddWithValue("cart_id", item.CartId);
            checkCmd.Parameters.AddWithValue("product_id", item.ProductId);

            using var reader = checkCmd.ExecuteReader();

            if (reader.Read())
            {
                var id = reader.GetInt32(0);
                var qty = reader.GetInt32(1);
                reader.Close();

                using var updateCmd = new NpgsqlCommand(
                    @"UPDATE cart_items
                      SET quantity = @quantity
                      WHERE id = @id",
                    connection
                );

                updateCmd.Parameters.AddWithValue("quantity", qty + item.Quantity);
                updateCmd.Parameters.AddWithValue("id", id);

                updateCmd.ExecuteNonQuery();
            }
            else
            {
                reader.Close();

                using var insertCmd = new NpgsqlCommand(
                    @"INSERT INTO cart_items 
                      (cart_id, product_id, quantity, unit_price)
                      VALUES (@cart_id, @product_id, @quantity, @unit_price)",
                    connection
                );

                insertCmd.Parameters.AddWithValue("cart_id", item.CartId);
                insertCmd.Parameters.AddWithValue("product_id", item.ProductId);
                insertCmd.Parameters.AddWithValue("quantity", item.Quantity);
                insertCmd.Parameters.AddWithValue("unit_price", item.UnitPrice);

                insertCmd.ExecuteNonQuery();
            }
        }

        public void UpdateQuantity(int id, int quantity)
        {
            using var connection = _db.GetConnection();
            connection.Open();

            using var command = new NpgsqlCommand(
                @"UPDATE cart_items
                  SET quantity = @quantity
                  WHERE id = @id",
                connection
            );

            command.Parameters.AddWithValue("id", id);
            command.Parameters.AddWithValue("quantity", quantity);

            command.ExecuteNonQuery();
        }

        public void Update(CartItem item)
        {
            using var connection = _db.GetConnection();
            connection.Open();

            using var command = new NpgsqlCommand(
                @"UPDATE cart_items
          SET quantity = @quantity,
              unit_price = @unit_price
          WHERE id = @id",
                connection
            );

            command.Parameters.AddWithValue("id", item.Id);
            command.Parameters.AddWithValue("quantity", item.Quantity);
            command.Parameters.AddWithValue("unit_price", item.UnitPrice);

            command.ExecuteNonQuery();
        }

        public void Remove(int id)
        {
            using var connection = _db.GetConnection();
            connection.Open();

            using var command = new NpgsqlCommand(
                "DELETE FROM cart_items WHERE id = @id",
                connection
            );

            command.Parameters.AddWithValue("id", id);

            command.ExecuteNonQuery();
        }
    }
}
