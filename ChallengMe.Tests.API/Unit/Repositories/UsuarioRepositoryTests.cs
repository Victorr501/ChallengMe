using ChallengMe.Models.Usuario;
using ChallengMe.Repositories.UsuarioRepository;
using ChallengMe.Tests.API.Helpers;
using Dapper;
using FluentAssertions;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Logging.Abstractions;

namespace ChallengMe.Tests.API.Unit.Repositories
{
    public class UsuarioRepositoryTests : IDisposable
    {
        private readonly SqliteConnection _conexion;
        private readonly UsuarioRepository _sut;

        public UsuarioRepositoryTests()
        {
            _conexion = new SqliteConnection("Data Source=:memory:");
            _conexion.Open();

            SqlMapper.AddTypeHandler(new GuidTypeHandler());

            _conexion.Execute("""
                CREATE TABLE Usuarios (
                    Id                TEXT     NOT NULL PRIMARY KEY,
                    Email             TEXT     NOT NULL UNIQUE,
                    PasswordHash      TEXT     NULL,
                    NombreUsuario     TEXT     NOT NULL,
                    AvatarUrl         TEXT     NULL,
                    NivelDificultad   INTEGER  NOT NULL DEFAULT 1,
                    PuntosTotal       INTEGER  NOT NULL DEFAULT 0,
                    RachaActual       INTEGER  NOT NULL DEFAULT 0,
                    RachaMaxima       INTEGER  NOT NULL DEFAULT 0,
                    UltimaActividad   TEXT     NULL,
                    FechaRegistro     TEXT     NOT NULL
                )
                """);

            var factory = new FakeDbConnectionFactory(_conexion);

            _sut = new UsuarioRepository(
                factory,
                NullLogger<UsuarioRepository>.Instance);
        }

        public void Dispose()
        {
            _conexion.Close();
            _conexion.Dispose();
        }

        private Usuario CrearUsuarioPrueba(
            string email = "test@test.com",
            string nombreUsuario = "TestUser",
            string? passwordHash = "hash_de_prueba")
        {
            return new Usuario
            {
                Id = Guid.NewGuid(),
                Email = email,
                NombreUsuario = nombreUsuario,
                PasswordHash = passwordHash,
                FechaRegistro = DateTime.UtcNow
            };
        }

        // CREAR

        [Fact]
        public async Task CrearAsync_UsuarioValido_SeGuardaEnBaseDeDatos()
        {
            var usuario = CrearUsuarioPrueba();

            await _sut.CrearAsync(usuario);

            var guardado = await _sut.ObtenerPorIdAsync(usuario.Id);

            guardado.Should().NotBeNull();
            guardado!.Email.Should().Be(usuario.Email);
            guardado.NombreUsuario.Should().Be(usuario.NombreUsuario);
        }

        [Fact]
        public async Task CrearAsync_EmailDuplicado_LanzaExcepcion()
        {
            var usuario1 = CrearUsuarioPrueba(email: "duplicado@test.com");
            var usuario2 = CrearUsuarioPrueba(email: "duplicado@test.com");
            await _sut.CrearAsync(usuario1);

            await _sut.Invoking(r => r.CrearAsync(usuario2))
                      .Should().ThrowAsync<Exception>();
        }

        // OBTENER POR EMAIL

        [Fact]
        public async Task ObtenerPorEmailAsync_UsuarioExiste_DevuelveUsuario()
        {
            var usuario = CrearUsuarioPrueba(email: "buscar@test.com");
            await _sut.CrearAsync(usuario);

            var resultado = await _sut.ObtenerPorEmailAsync("buscar@test.com");

            resultado.Should().NotBeNull();
            resultado!.Email.Should().Be("buscar@test.com");
            resultado.NombreUsuario.Should().Be(usuario.NombreUsuario);
        }

        [Fact]
        public async Task ObtenerPorEmailAsync_UsuarioNoExiste_DevuelveNull()
        {
            var resultado = await _sut.ObtenerPorEmailAsync("noexiste@test.com");

            resultado.Should().BeNull();
        }

        // OBTENER POR ID

        [Fact]
        public async Task ObtenerPorIdAsync_UsuarioExiste_DevuelveUsuario()
        {
            var usuario = CrearUsuarioPrueba();
            await _sut.CrearAsync(usuario);

            var resultado = await _sut.ObtenerPorIdAsync(usuario.Id);

            resultado.Should().NotBeNull();
            resultado!.Id.Should().Be(usuario.Id);
            resultado.Email.Should().Be(usuario.Email);
        }

        [Fact]
        public async Task ObtenerPorIdAsync_UsuarioNoExiste_DevuelveNull()
        {

            var resultado = await _sut.ObtenerPorIdAsync(Guid.NewGuid());

            resultado.Should().BeNull();
        }

        // ACTUALIZAR

        [Fact]
        public async Task ActualizarAsync_CambiaNombreUsuario_SeGuardaEnBaseDeDatos()
        {

            var usuario = CrearUsuarioPrueba(nombreUsuario: "NombreOriginal");
            await _sut.CrearAsync(usuario);

            usuario.NombreUsuario = "NombreNuevo";


            await _sut.ActualizarAsync(usuario);


            var actualizado = await _sut.ObtenerPorIdAsync(usuario.Id);
            actualizado!.NombreUsuario.Should().Be("NombreNuevo");
        }

        [Fact]
        public async Task ActualizarAsync_ActualizaPuntosYRacha_SeGuardaEnBaseDeDatos()
        {
            var usuario = CrearUsuarioPrueba();
            await _sut.CrearAsync(usuario);

            usuario.PuntosTotal = 100;
            usuario.RachaActual = 5;
            usuario.RachaMaxima = 10;

            await _sut.ActualizarAsync(usuario);

            var actualizado = await _sut.ObtenerPorIdAsync(usuario.Id);
            actualizado!.PuntosTotal.Should().Be(100);
            actualizado.RachaActual.Should().Be(5);
            actualizado.RachaMaxima.Should().Be(10);
        }
    }
}