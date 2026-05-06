using ChallengMe.Models.Usuario;
using ChallengMe.Repositories.DbConnectionFactory;
using Dapper;
using Microsoft.Extensions.Logging;

namespace ChallengMe.Repositories.UsuarioRepository
{
    public class UsuarioRepository : IUsuarioRepository
    {
        private readonly IDbConnectionFactory _dbConnectionFactory;
        private readonly ILogger<UsuarioRepository> _logger;

        public UsuarioRepository(IDbConnectionFactory dbConnectionFactory, ILogger<UsuarioRepository> logger)
        {
            _dbConnectionFactory = dbConnectionFactory;
            _logger = logger;
        }

        public async Task<Usuario?> ObtenerPorEmailAsync(string email)
        {
            using var connection = _dbConnectionFactory.CrearConexion();
            const string sql = """
                SELECT Id, Email, PasswordHash, NombreUsuario, AvatarUrl,
                       NivelDificultad, PuntosTotal, RachaActual, RachaMaxima,
                       UltimaActividad, FechaRegistro,
                       ProveedorAutenticacion, EmailVerificado
                FROM Usuarios
                WHERE Email = @Email
                """;
            _logger.LogInformation("Obtener Usuario por email: {email}", email);
            return await connection.QueryFirstOrDefaultAsync<Usuario>(sql, new { Email = email });
        }

        public async Task<Usuario?> ObtenerPorIdAsync(Guid id) 
        {
            using var connection = _dbConnectionFactory.CrearConexion();
            const string sql = """
                SELECT Id, Email, PasswordHash, NombreUsuario, AvatarUrl,
                       NivelDificultad, PuntosTotal, RachaActual, RachaMaxima,
                       UltimaActividad, FechaRegistro,
                       ProveedorAutenticacion, EmailVerificado
                FROM Usuarios
                WHERE Id = @Id
                """;
            _logger.LogInformation("Obtener Usuario por Id: {id}", id);
            return await connection.QueryFirstOrDefaultAsync<Usuario>(sql, new { Id = id });
        }

        public async Task CrearAsync(Usuario usuario)
        {
            using var connection = _dbConnectionFactory.CrearConexion();
            const string sql = """
                INSERT INTO Usuarios 
                    (Id, Email, PasswordHash, NombreUsuario, AvatarUrl,
                     NivelDificultad, PuntosTotal, RachaActual, RachaMaxima,
                     UltimaActividad, FechaRegistro,
                     ProveedorAutenticacion, EmailVerificado)
                VALUES 
                    (@Id, @Email, @PasswordHash, @NombreUsuario, @AvatarUrl,
                     @NivelDificultad, @PuntosTotal, @RachaActual, @RachaMaxima,
                     @UltimaActividad, @FechaRegistro,
                     @ProveedorAutenticacion, @EmailVerificado)
                """;
            _logger.LogInformation("Crear usuario con id:{id} ", usuario.Id);
            await connection.ExecuteAsync(sql, usuario);
        }

        public async Task ActualizarAsync(Usuario usuario) 
        {
            using var connection = _dbConnectionFactory.CrearConexion();
            const string sql = """
                UPDATE Usuarios SET
                    NombreUsuario   = @NombreUsuario,
                    AvatarUrl       = @AvatarUrl,
                    NivelDificultad = @NivelDificultad,
                    PuntosTotal     = @PuntosTotal,
                    RachaActual     = @RachaActual,
                    RachaMaxima     = @RachaMaxima,
                    UltimaActividad = @UltimaActividad
                WHERE Id = @Id
                """;
            _logger.LogInformation("Actualizando usuario con id:{id}", usuario.Id);
            await connection.ExecuteAsync(sql, usuario);
        }

    }
}
