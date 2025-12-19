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

        public void Create(User user)
        {
            using var connection = _db.GetConnection();
            connection.Open();

            using var command = new NpgsqlCommand(@"
                INSERT INTO users (full_name, email, password_hash, created_at)
                VALUES (@full_name, @email, @password_hash, @created_at)
            ", connection);

            command.Parameters.AddWithValue("@full_name", user.FullName);
            command.Parameters.AddWithValue("@email", user.Email);
            command.Parameters.AddWithValue("@password_hash", user.Password);
            command.Parameters.AddWithValue("@created_at", user.CreatedAt);

            command.ExecuteNonQuery();
        }
    }
}
