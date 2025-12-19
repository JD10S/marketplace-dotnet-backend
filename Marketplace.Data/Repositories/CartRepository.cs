using Microsoft.Data.SqlClient;
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

            var command = new SqlCommand(@"
        UPDATE CartItems
        SET Quantity = @Quantity
        WHERE Id = @Id
    ", connection);

            command.Parameters.AddWithValue("@Quantity", item.Quantity);
            command.Parameters.AddWithValue("@Id", item.Id);

            command.ExecuteNonQuery();
        }

        public IEnumerable<CartItem> GetByUser(int userId)
        {
            var items = new List<CartItem>();

            using var connection = _db.GetConnection();
            var command = new SqlCommand(
                "SELECT * FROM CartItems WHERE CartId = @UserId",
                connection
            );

            command.Parameters.AddWithValue("@UserId", userId);

            connection.Open();
            using var reader = command.ExecuteReader();

            while (reader.Read())
            {
                items.Add(new CartItem
                {
                    Id = (int)reader["Id"],
                    CartId = (int)reader["CartId"],
                    ProductId = (int)reader["ProductId"],
                    Quantity = (int)reader["Quantity"],
                    UnitPrice = (decimal)reader["UnitPrice"]
                });
            }

            return items;
        }

        public void Add(CartItem item)
        {
            using var connection = _db.GetConnection();
            connection.Open();

            
            var checkCmd = new SqlCommand(
                @"SELECT Id, Quantity 
          FROM CartItems 
          WHERE CartId = @CartId AND ProductId = @ProductId",
                connection
            );

            checkCmd.Parameters.AddWithValue("@CartId", item.CartId);
            checkCmd.Parameters.AddWithValue("@ProductId", item.ProductId);

            using var reader = checkCmd.ExecuteReader();

            if (reader.Read())
            {
                var id = reader.GetInt32(0);
                var qty = reader.GetInt32(1);
                reader.Close();

                
                var updateCmd = new SqlCommand(
                    @"UPDATE CartItems 
              SET Quantity = @Qty 
              WHERE Id = @Id",
                    connection
                );

                updateCmd.Parameters.AddWithValue("@Qty", qty + item.Quantity);
                updateCmd.Parameters.AddWithValue("@Id", id);

                updateCmd.ExecuteNonQuery();
            }
            else
            {
                reader.Close();

                
                var insertCmd = new SqlCommand(
                    @"INSERT INTO CartItems (CartId, ProductId, Quantity, UnitPrice)
              VALUES (@CartId, @ProductId, @Quantity, @UnitPrice)",
                    connection
                );

                insertCmd.Parameters.AddWithValue("@CartId", item.CartId);
                insertCmd.Parameters.AddWithValue("@ProductId", item.ProductId);
                insertCmd.Parameters.AddWithValue("@Quantity", item.Quantity);
                insertCmd.Parameters.AddWithValue("@UnitPrice", item.UnitPrice);

                insertCmd.ExecuteNonQuery();
            }
        }

        public void UpdateQuantity(int id, int quantity)
        {
            using var connection = _db.GetConnection();
            var command = new SqlCommand(
                "UPDATE CartItems SET Quantity = @Quantity WHERE Id = @Id",
                connection
            );

            command.Parameters.AddWithValue("@Id", id);
            command.Parameters.AddWithValue("@Quantity", quantity);

            connection.Open();
            command.ExecuteNonQuery();
        }

        public void Remove(int id)
        {
            using var connection = _db.GetConnection();
            var command = new SqlCommand(
                "DELETE FROM CartItems WHERE Id = @Id",
                connection
            );

            command.Parameters.AddWithValue("@Id", id);

            connection.Open();
            command.ExecuteNonQuery();
        }
    }
}
