using ChallengMe.Models.TokenResetPassword;
using ChallengMe.Repositories.TokenResetPasswordRepository;
using ChallengMe.Tests.API.Helpers.Unit;
using Dapper;
using FluentAssertions;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Logging.Abstractions;

namespace ChallengMe.Tests.API.Unit.Repositories;

public class TokenResetPasswordRepositoryTests : IDisposable
{
    private readonly SqliteConnection _conexion;
    private readonly TokenResetPasswordRepository _sut;

    public TokenResetPasswordRepositoryTests()
    {
        _conexion = new SqliteConnection("Data Source=:memory:");
        _conexion.Open();

        SqlMapper.AddTypeHandler(new GuidTypeHandler());

        _conexion.Execute("""
            CREATE TABLE TokensResetPassword (
                Id            TEXT     NOT NULL PRIMARY KEY,
                UsuarioId     TEXT     NOT NULL,
                Token         TEXT     NOT NULL,
                Expiracion    TEXT     NOT NULL,
                Usado         INTEGER  NOT NULL DEFAULT 0,
                FechaCreacion TEXT     NOT NULL
            )
            """);

        var factory = new FakeDbConnectionFactory(_conexion);
        _sut = new TokenResetPasswordRepository(factory, NullLogger<TokenResetPasswordRepository>.Instance);
    }

    public void Dispose()
    {
        _conexion.Close();
        _conexion.Dispose();
    }

    private TokenResetPassword CrearTokenPrueba(
        Guid? usuarioId = null,
        string? token = null,
        DateTime? expiracion = null,
        bool usado = false)
    {
        return new TokenResetPassword
        {
            Id = Guid.NewGuid(),
            UsuarioId = usuarioId ?? Guid.NewGuid(),
            Token = token ?? Guid.NewGuid().ToString("N"),
            Expiracion = expiracion ?? DateTime.UtcNow.AddHours(1),
            Usado = usado,
            FechaCreacion = DateTime.UtcNow
        };
    }


    [Fact]
    public async Task CrearAsync_TokenValido_SeInsertaConTodosLosCampos()
    {
        var tokenReset = CrearTokenPrueba();

        await _sut.CrearAsync(tokenReset);

        var guardado = await _sut.ObtenerPorTokenAsync(tokenReset.Token);

        guardado.Should().NotBeNull();
        guardado!.Id.Should().Be(tokenReset.Id);
        guardado.UsuarioId.Should().Be(tokenReset.UsuarioId);
        guardado.Token.Should().Be(tokenReset.Token);
        guardado.Usado.Should().BeFalse();
        guardado.FechaCreacion.Should().BeCloseTo(tokenReset.FechaCreacion, precision: TimeSpan.FromSeconds(1));
    }


    [Fact]
    public async Task ObtenerPorTokenAsync_TokenExiste_DevuelveTokenCompleto()
    {
        var tokenReset = CrearTokenPrueba();
        await _sut.CrearAsync(tokenReset);

        var resultado = await _sut.ObtenerPorTokenAsync(tokenReset.Token);

        resultado.Should().NotBeNull();
        resultado!.Id.Should().Be(tokenReset.Id);
        resultado.UsuarioId.Should().Be(tokenReset.UsuarioId);
        resultado.Token.Should().Be(tokenReset.Token);
        resultado.Usado.Should().BeFalse();
    }

    [Fact]
    public async Task ObtenerPorTokenAsync_TokenInexistente_DevuelveNull()
    {
        var resultado = await _sut.ObtenerPorTokenAsync("token-que-no-existe");

        resultado.Should().BeNull();
    }


    [Fact]
    public async Task MarcarComoUsadoAsync_TokenMarcado_UsadoPasaATrue()
    {
        var tokenReset = CrearTokenPrueba(usado: false);
        await _sut.CrearAsync(tokenReset);

        await _sut.MarcarComoUsadoAsync(tokenReset.Id);

        var actualizado = await _sut.ObtenerPorTokenAsync(tokenReset.Token);
        actualizado!.Usado.Should().BeTrue();
    }

    [Fact]
    public async Task MarcarComoUsadoAsync_MarcaUno_OtrosTokensNoAfectados()
    {
        var tokenObjetivo = CrearTokenPrueba();
        var tokenOtro = CrearTokenPrueba();
        await _sut.CrearAsync(tokenObjetivo);
        await _sut.CrearAsync(tokenOtro);

        await _sut.MarcarComoUsadoAsync(tokenObjetivo.Id);

        var otroSinCambios = await _sut.ObtenerPorTokenAsync(tokenOtro.Token);
        otroSinCambios!.Usado.Should().BeFalse();
    }


    [Fact]
    public async Task EliminarExpiradosAsync_TokenExpirado_SeElimina()
    {
        var tokenExpirado = CrearTokenPrueba(expiracion: DateTime.UtcNow.AddHours(-2));
        await _sut.CrearAsync(tokenExpirado);

        await _sut.EliminarExpiradosAsync();

        var resultado = await _sut.ObtenerPorTokenAsync(tokenExpirado.Token);
        resultado.Should().BeNull();
    }

    [Fact]
    public async Task EliminarExpiradosAsync_TokenUsado_SeElimina()
    {
        var tokenUsado = CrearTokenPrueba(usado: true);
        await _sut.CrearAsync(tokenUsado);

        await _sut.EliminarExpiradosAsync();

        var resultado = await _sut.ObtenerPorTokenAsync(tokenUsado.Token);
        resultado.Should().BeNull();
    }

    [Fact]
    public async Task EliminarExpiradosAsync_TokenVigenteYNoUsado_NoSeElimina()
    {
        var tokenVigente = CrearTokenPrueba(expiracion: DateTime.UtcNow.AddHours(1), usado: false);
        await _sut.CrearAsync(tokenVigente);

        await _sut.EliminarExpiradosAsync();

        var resultado = await _sut.ObtenerPorTokenAsync(tokenVigente.Token);
        resultado.Should().NotBeNull();
    }

    [Fact]
    public async Task EliminarExpiradosAsync_MixDeTokens_SoloEliminaLosCorrectos()
    {
        // Arrange — tres tipos distintos en la misma base de datos
        var tokenExpirado = CrearTokenPrueba(expiracion: DateTime.UtcNow.AddHours(-1));
        var tokenUsado = CrearTokenPrueba(usado: true);
        var tokenVigente = CrearTokenPrueba(expiracion: DateTime.UtcNow.AddHours(2), usado: false);

        await _sut.CrearAsync(tokenExpirado);
        await _sut.CrearAsync(tokenUsado);
        await _sut.CrearAsync(tokenVigente);

        // Act
        await _sut.EliminarExpiradosAsync();

        // Assert — solo el vigente sobrevive
        (await _sut.ObtenerPorTokenAsync(tokenExpirado.Token)).Should().BeNull();
        (await _sut.ObtenerPorTokenAsync(tokenUsado.Token)).Should().BeNull();
        (await _sut.ObtenerPorTokenAsync(tokenVigente.Token)).Should().NotBeNull();
    }
}