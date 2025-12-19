using Microsoft.Extensions.Configuration;
using Npgsql;
using System;

namespace Marketplace.Data.Database
{
    public class DatabaseConnection
    {
        private readonly string _connectionString = string.Empty;

        public DatabaseConnection(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public NpgsqlConnection GetConnection()
        {
            return new NpgsqlConnection(_connectionString);
        }
    }
}
