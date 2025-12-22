using Marketplace.Data.Database;
using Marketplace.Data.Interfaces;
using Marketplace.Entities.Entities;
using Npgsql;
using System.Data;

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
                "SELECT id, full_name, email, password_hash, created_at FROM users WHERE email = @email",
                connection
            );

            command.Parameters.AddWithValue("@email", email);
            connection.Open();
            using var reader = command.ExecuteReader();

            if (!reader.Read()) return null;

            return new User
            {
                Id = reader.GetInt32("id"),
                Name = reader.GetString("full_name"),        
                Email = reader.GetString("email"),
                Password = reader.GetString("password_hash"), 
                CreatedAt = reader.GetDateTime("created_at")
            };
        }

        public int Create(User user)
        {
            using var connection = _db.GetConnection();
            connection.Open();

            using var cmd = new NpgsqlCommand(
                @"INSERT INTO users (full_name, email, password_hash)
          VALUES (@full_name, @email, @password_hash)
          RETURNING id",
                connection
            );

            cmd.Parameters.AddWithValue("full_name", user.Name);
            cmd.Parameters.AddWithValue("email", user.Email);
            cmd.Parameters.AddWithValue("password_hash", user.Password);

            return (int)cmd.ExecuteScalar()!;
        }

        public int CreateUserWithCart(User user)
        {
            using var connection = _db.GetConnection();
            connection.Open();
            using var transaction = connection.BeginTransaction();

            try
            {
                int userId;
                using (var userCmd = new NpgsqlCommand(
                    @"INSERT INTO users (full_name, email, password_hash, created_at)
              VALUES (@full_name, @email, @password_hash, NOW())
              RETURNING id", connection, transaction))
                {
                    userCmd.Parameters.AddWithValue("full_name", user.Name);
                    userCmd.Parameters.AddWithValue("email", user.Email);
                    userCmd.Parameters.AddWithValue("password_hash", user.Password);
                    userId = (int)userCmd.ExecuteScalar()!;
                }

                using (var cartCmd = new NpgsqlCommand(
                    @"INSERT INTO carts (user_id, created_at)
              VALUES (@userId, NOW())
              RETURNING id", connection, transaction))
                {
                    cartCmd.Parameters.AddWithValue("userId", userId);
                    cartCmd.ExecuteScalar(); 
                }

                transaction.Commit();
                return userId;
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }
    }
}
