using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChallengMe.Repositories.DbConnectionFactory
{
    public class DbConnectionFactory : IDbConnectionFactory
    {
        private readonly string _connectionString;
        private readonly ILogger<DbConnectionFactory> _logger;

        public DbConnectionFactory(IConfiguration configuration, ILogger<DbConnectionFactory> logger)
        {
            _connectionString = configuration.GetConnectionString("SqlDb");
            _logger = logger;
        }

        public IDbConnection CrearConexion()
        {
            var sql = new SqlConnection(_connectionString);
            if (sql == null)
            {
                _logger.LogError("El sqlConnection se ha creado mal");
                throw new InvalidOperationException("No se encontró la connection string SqlDb.");
            }

            return sql;
        }
    }
}
