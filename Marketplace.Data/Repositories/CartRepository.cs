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

          
            int? cartId = null;

            using (var cartCmd = new NpgsqlCommand(
                "SELECT id FROM carts WHERE user_id = @user_id",
                connection))
            {
                cartCmd.Parameters.AddWithValue("@user_id", userId);
                cartId = cartCmd.ExecuteScalar() as int?;
            }

            if (cartId is null)
                return items; 

            
            using var command = new NpgsqlCommand(
                @"SELECT id, product_id, quantity, unit_price
          FROM cart_items
          WHERE cart_id = @cart_id",
                connection
            );

            command.Parameters.AddWithValue("@cart_id", cartId.Value);

            using var reader = command.ExecuteReader();

            while (reader.Read())
            {
                items.Add(new CartItem
                {
                    Id = reader.GetInt32(0),
                    ProductId = reader.GetInt32(1),
                    Quantity = reader.GetInt32(2),
                    UnitPrice = reader.GetDecimal(3)
                });
            }

            return items;
        }

        public void Add(int cartId, CartItem item)
        {
            using var connection = _db.GetConnection();
            connection.Open();

            using var checkCmd = new NpgsqlCommand(
                @"SELECT id, quantity
          FROM cart_items
          WHERE cart_id = @cart_id AND product_id = @product_id",
                connection
            );

            checkCmd.Parameters.AddWithValue("cart_id", cartId);
            checkCmd.Parameters.AddWithValue("product_id", item.ProductId);

            using var reader = checkCmd.ExecuteReader();

            if (reader.Read())
            {
                var id = reader.GetInt32(0);
                var currentQty = reader.GetInt32(1);
                reader.Close();

                using var updateCmd = new NpgsqlCommand(
                    @"UPDATE cart_items
              SET quantity = @quantity
              WHERE id = @id",
                    connection
                );

                updateCmd.Parameters.AddWithValue("quantity", currentQty + item.Quantity);
                updateCmd.Parameters.AddWithValue("id", id);

                updateCmd.ExecuteNonQuery();
            }
            else
            {
                reader.Close();

                if (item.UnitPrice == null)
                    throw new Exception("UnitPrice is required");

                using var insertCmd = new NpgsqlCommand(
                    @"INSERT INTO cart_items
              (cart_id, product_id, quantity, unit_price)
              VALUES (@cart_id, @product_id, @quantity, @unit_price)",
                    connection
                );

                insertCmd.Parameters.AddWithValue("cart_id", cartId);
                insertCmd.Parameters.AddWithValue("product_id", item.ProductId);
                insertCmd.Parameters.AddWithValue("quantity", item.Quantity);
                insertCmd.Parameters.AddWithValue("unit_price", item.UnitPrice.Value);

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

        public Cart? GetCartByUser(int userId)
        {
            using var connection = _db.GetConnection();
            connection.Open();

            using var command = new NpgsqlCommand(
                "SELECT id, user_id FROM carts WHERE user_id = @userId",
                connection
            );

            command.Parameters.AddWithValue("userId", userId);

            using var reader = command.ExecuteReader();

            if (!reader.Read()) return null;

            return new Cart
            {
                Id = reader.GetInt32(0),
                UserId = reader.GetInt32(1)
            };
        }

        public void CreateCart(int userId)
        {
            using var connection = _db.GetConnection();
            connection.Open();

            using var command = new NpgsqlCommand(
                @"INSERT INTO carts (user_id, created_at)
          VALUES (@user_id, NOW())",
                connection
            );

            command.Parameters.AddWithValue("user_id", userId);
            command.ExecuteNonQuery();
        }
    }
}
