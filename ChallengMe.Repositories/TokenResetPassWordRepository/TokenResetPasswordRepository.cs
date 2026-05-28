using ChallengMe.Models.TokenResetPassword;
using ChallengMe.Repositories.DbConnectionFactory;
using Dapper;
using Microsoft.Extensions.Logging;

namespace ChallengMe.Repositories.TokenResetPasswordRepository
{
    public class TokenResetPasswordRepository : ITokenResetPasswordRepository
    {
        private readonly IDbConnectionFactory _dbConnectionFactory;
        private readonly ILogger<TokenResetPasswordRepository> _logger;

        public TokenResetPasswordRepository(IDbConnectionFactory dbConnectionFactory, ILogger<TokenResetPasswordRepository> logger)
        {
            _dbConnectionFactory = dbConnectionFactory;
            _logger = logger;
        }

        public async Task<TokenResetPassword?> ObtenerPorTokenAsync(string token)
        {
            using var connection = _dbConnectionFactory.CrearConexion();
            const string sql = """
                SELECT Id, UsuarioId, Token, Expiracion, Usado, FechaCreacion
                FROM TokensResetPassword
                WHERE Token = @Token
                """;
            _logger.LogInformation("Obteniendo token de reset: {token}", token);
            return await connection.QueryFirstOrDefaultAsync<TokenResetPassword>(sql, new { Token = token });
        }

        public async Task CrearAsync(TokenResetPassword tokenReset)
        {
            using var connection = _dbConnectionFactory.CrearConexion();
            const string sql = """
                INSERT INTO TokensResetPassword
                    (Id, UsuarioId, Token, Expiracion, Usado, FechaCreacion)
                VALUES
                    (@Id, @UsuarioId, @Token, @Expiracion, @Usado, @FechaCreacion)
                """;
            _logger.LogInformation("Creando token de reset para usuario: {usuarioId}", tokenReset.UsuarioId);
            await connection.ExecuteAsync(sql, tokenReset);
        }

        public async Task MarcarComoUsadoAsync(Guid id)
        {
            using var connection = _dbConnectionFactory.CrearConexion();
            const string sql = """
                UPDATE TokensResetPassword
                SET Usado = 1
                WHERE Id = @Id
                """;
            _logger.LogInformation("Marcando token como usado: {id}", id);
            await connection.ExecuteAsync(sql, new { Id = id });
        }

        public async Task EliminarExpiradosAsync()
        {
            using var connection = _dbConnectionFactory.CrearConexion();
            const string sql = """
                DELETE FROM TokensResetPassword
                WHERE Expiracion < @Ahora
                OR Usado = 1
                """;
            await connection.ExecuteAsync(sql, new { Ahora = DateTime.UtcNow }); // ← parámetro en vez de función SQL
        }
    }
}