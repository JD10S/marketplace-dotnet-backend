using Npgsql;
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
            connection.Open();

            using var command = new NpgsqlCommand(
                @"SELECT 
                    id, 
                    name, 
                    description, 
                    price, 
                    stock, 
                    image_url, 
                    created_at 
                  FROM products 
                  ORDER BY id",
                connection
            );

            using var reader = command.ExecuteReader();

            while (reader.Read())
            {
                products.Add(new Product
                {
                    Id = reader.GetInt32(reader.GetOrdinal("id")),
                    Name = reader.GetString(reader.GetOrdinal("name")),
                    Description = reader.IsDBNull(reader.GetOrdinal("description"))
                        ? ""
                        : reader.GetString(reader.GetOrdinal("description")),
                    Price = reader.GetDecimal(reader.GetOrdinal("price")),
                    Stock = reader.GetInt32(reader.GetOrdinal("stock")),
                    ImageUrl = reader.GetString(reader.GetOrdinal("image_url")),
                    CreatedAt = reader.GetDateTime(reader.GetOrdinal("created_at"))
                });
            }

            return products;
        }

        public Product? GetById(int id)
        {
            using var connection = _db.GetConnection();
            connection.Open();

            using var command = new NpgsqlCommand(
                @"SELECT 
                    id, name, description, price, stock, image_url, created_at
                  FROM products 
                  WHERE id = @id",
                connection
            );

            command.Parameters.AddWithValue("id", id);

            using var reader = command.ExecuteReader();

            if (!reader.Read()) return null;

            return new Product
            {
                Id = reader.GetInt32(reader.GetOrdinal("id")),
                Name = reader.GetString(reader.GetOrdinal("name")),
                Description = reader.IsDBNull(reader.GetOrdinal("description"))
                    ? ""
                    : reader.GetString(reader.GetOrdinal("description")),
                Price = reader.GetDecimal(reader.GetOrdinal("price")),
                Stock = reader.GetInt32(reader.GetOrdinal("stock")),
                ImageUrl = reader.GetString(reader.GetOrdinal("image_url")),
                CreatedAt = reader.GetDateTime(reader.GetOrdinal("created_at"))
            };
        }

        public void Create(Product product)
        {
            using var connection = _db.GetConnection();
            connection.Open();

            using var command = new NpgsqlCommand(
                @"INSERT INTO products 
                  (name, description, price, stock, image_url, created_at)
                  VALUES 
                  (@name, @description, @price, @stock, @image_url, @created_at)",
                connection
            );

            command.Parameters.AddWithValue("name", product.Name);
            command.Parameters.AddWithValue("description", product.Description ?? "");
            command.Parameters.AddWithValue("price", product.Price);
            command.Parameters.AddWithValue("stock", product.Stock);
            command.Parameters.AddWithValue("image_url", product.ImageUrl);
            command.Parameters.AddWithValue("created_at", product.CreatedAt);

            command.ExecuteNonQuery();
        }

        public void Update(Product product)
        {
            using var connection = _db.GetConnection();
            connection.Open();

            using var command = new NpgsqlCommand(
                @"UPDATE products SET
                    name = @name,
                    description = @description,
                    price = @price,
                    stock = @stock,
                    image_url = @image_url
                  WHERE id = @id",
                connection
            );

            command.Parameters.AddWithValue("id", product.Id);
            command.Parameters.AddWithValue("name", product.Name);
            command.Parameters.AddWithValue("description", product.Description ?? "");
            command.Parameters.AddWithValue("price", product.Price);
            command.Parameters.AddWithValue("stock", product.Stock);
            command.Parameters.AddWithValue("image_url", product.ImageUrl);

            command.ExecuteNonQuery();
        }

        public void Delete(int id)
        {
            using var connection = _db.GetConnection();
            connection.Open();

            using var command = new NpgsqlCommand(
                "DELETE FROM products WHERE id = @id",
                connection
            );

            command.Parameters.AddWithValue("id", id);

            command.ExecuteNonQuery();
        }
    }
}
