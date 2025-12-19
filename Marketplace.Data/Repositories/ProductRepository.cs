using Microsoft.Data.SqlClient;
using Marketplace.Data.Database;
using Marketplace.Data.Interfaces;
using Marketplace.Entities.Entities;

namespace Marketplace.Data.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private readonly DatabaseConnection _db;

        public ProductRepository(DatabaseConnection db)
        {
            _db = db;
        }

        public IEnumerable<Product> GetAll()
        {
            var products = new List<Product>();

            using var connection = _db.GetConnection();
            var command = new SqlCommand("SELECT * FROM Products", connection);

            connection.Open();
            using var reader = command.ExecuteReader();

            while (reader.Read())
            {
                products.Add(new Product
                {
                    Id = (int)reader["Id"],
                    Name = reader["Name"].ToString()!,
                    Description = reader["Description"].ToString()!,
                    Price = (decimal)reader["Price"],
                    Stock = (int)reader["Stock"],
                    ImageUrl = reader["ImageUrl"].ToString()!,
                    CreatedAt = (DateTime)reader["CreatedAt"]
                });
            }

            return products;
        }

        public Product? GetById(int id)
        {
            using var connection = _db.GetConnection();
            var command = new SqlCommand(
                "SELECT * FROM Products WHERE Id = @Id",
                connection
            );

            command.Parameters.AddWithValue("@Id", id);

            connection.Open();
            using var reader = command.ExecuteReader();

            if (!reader.Read()) return null;

            return new Product
            {
                Id = (int)reader["Id"],
                Name = reader["Name"].ToString()!,
                Description = reader["Description"].ToString()!,
                Price = (decimal)reader["Price"],
                Stock = (int)reader["Stock"],
                ImageUrl = reader["ImageUrl"].ToString()!,
                CreatedAt = (DateTime)reader["CreatedAt"]
            };
        }

        public void Create(Product product)
        {
            using var connection = _db.GetConnection();
            var command = new SqlCommand(
                @"INSERT INTO Products (Name, Description, Price, Stock, ImageUrl, CreatedAt)
                  VALUES (@Name, @Description, @Price, @Stock, @ImageUrl, @CreatedAt)",
                connection
            );

            command.Parameters.AddWithValue("@Name", product.Name);
            command.Parameters.AddWithValue("@Description", product.Description);
            command.Parameters.AddWithValue("@Price", product.Price);
            command.Parameters.AddWithValue("@Stock", product.Stock);
            command.Parameters.AddWithValue("@ImageUrl", product.ImageUrl);
            command.Parameters.AddWithValue("@CreatedAt", product.CreatedAt);

            connection.Open();
            command.ExecuteNonQuery();
        }

        public void Update(Product product)
        {
            using var connection = _db.GetConnection();
            var command = new SqlCommand(
                @"UPDATE Products SET
                    Name = @Name,
                    Description = @Description,
                    Price = @Price,
                    Stock = @Stock,
                    ImageUrl = @ImageUrl
                  WHERE Id = @Id",
                connection
            );

            command.Parameters.AddWithValue("@Id", product.Id);
            command.Parameters.AddWithValue("@Name", product.Name);
            command.Parameters.AddWithValue("@Description", product.Description);
            command.Parameters.AddWithValue("@Price", product.Price);
            command.Parameters.AddWithValue("@Stock", product.Stock);
            command.Parameters.AddWithValue("@ImageUrl", product.ImageUrl);

            connection.Open();
            command.ExecuteNonQuery();
        }

        public void Delete(int id)
        {
            using var connection = _db.GetConnection();
            var command = new SqlCommand(
                "DELETE FROM Products WHERE Id = @Id",
                connection
            );

            command.Parameters.AddWithValue("@Id", id);

            connection.Open();
            command.ExecuteNonQuery();
        }
    }
}
