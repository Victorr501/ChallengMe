using ChallengMe.Models.Usuario;

namespace ChallengMe.Repositories.UsuarioRepository
{
    public interface IUsuarioRepository
    {
        Task<Usuario?> ObtenerPorEmailAsync(string email);
        Task<Usuario?> ObtenerPorIdAsync(Guid id);
        Task CrearAsync(Usuario usuario);
        Task ActualizarAsync(Usuario usuario);
    }
}
