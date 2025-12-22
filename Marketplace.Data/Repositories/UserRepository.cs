using Npgsql;
using Marketplace.Data.Database;
using Marketplace.Data.Interfaces;
using Marketplace.Entities.Entities;

namespace Marketplace.Data.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly DatabaseConnection _db;

        public UserRepository(DatabaseConnection db)
        {
            _db = db;
        }

        public User? GetByEmail(string email)
        {
            using var connection = _db.GetConnection();
            using var command = new NpgsqlCommand(
                "SELECT * FROM users WHERE email = @email",
                connection
            );

            command.Parameters.AddWithValue("@email", email);

            connection.Open();
            using var reader = command.ExecuteReader();

            if (!reader.Read()) return null;

            return new User
            {
                Id = reader.GetInt32(reader.GetOrdinal("id")),
                FullName = reader.GetString(reader.GetOrdinal("full_name")),
                Email = reader.GetString(reader.GetOrdinal("email")),
                Password = reader.GetString(reader.GetOrdinal("password_hash")),
                CreatedAt = reader.GetDateTime(reader.GetOrdinal("created_at"))
            };
        }

        public int Create(User user)
        {
            using var connection = _db.GetConnection();
            connection.Open();

            using var cmd = new NpgsqlCommand(
                @"INSERT INTO users (name, email, password)
          VALUES (@name, @email, @password)
          RETURNING id",
                connection
            );

            cmd.Parameters.AddWithValue("name", user.FullName);
            cmd.Parameters.AddWithValue("email", user.Email);
            cmd.Parameters.AddWithValue("password", user.Password);

            return (int)cmd.ExecuteScalar()!;
        }
    }
}
