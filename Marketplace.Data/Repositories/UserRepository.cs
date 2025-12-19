using Marketplace.Data.Database;
using Marketplace.Data.Interfaces;
using Marketplace.Entities.Entities;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            var command = new SqlCommand(
                "SELECT * FROM Users WHERE Email = @Email",
                connection
            );

            command.Parameters.AddWithValue("@Email", email);

            connection.Open();
            using var reader = command.ExecuteReader();

            if (!reader.Read()) return null;

            return new User
            {
                Id = (int)reader["Id"],
                FullName = reader["FullName"].ToString()!,
                Email = reader["Email"].ToString()!,
                Password = reader["PasswordHash"].ToString()!,
                CreatedAt = (DateTime)reader["CreatedAt"]
            };
        }

        public void Create(User user)
        {
            using var connection = _db.GetConnection();
            var command = new SqlCommand(
                @"INSERT INTO Users (FullName, Email, PasswordHash, CreatedAt)
                  VALUES (@FullName, @Email, @PasswordHash, @CreatedAt)",
                connection
            );

            command.Parameters.AddWithValue("@FullName", user.FullName);
            command.Parameters.AddWithValue("@Email", user.Email);
            command.Parameters.AddWithValue("@PasswordHash", user.Password);
            command.Parameters.AddWithValue("@CreatedAt", user.CreatedAt);

            connection.Open();
            command.ExecuteNonQuery();
        }
    }
}
