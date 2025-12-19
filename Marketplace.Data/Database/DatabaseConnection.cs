using Npgsql;
using Microsoft.Extensions.Configuration;

namespace Marketplace.Data.Database
{
    public class DatabaseConnection
    {
        private readonly string _connectionString;

        public DatabaseConnection(IConfiguration configuration)
        {
            _connectionString = configuration["CONNECTION_STRING"]
                ?? throw new Exception("CONNECTION_STRING no definida");
        }

        public NpgsqlConnection GetConnection()
        {
            return new NpgsqlConnection(_connectionString);
        }
    }
}
