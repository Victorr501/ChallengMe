using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChallengMe.Services.AuthService
{
    public interface IAuthService
    {
        Task<String> LogingMicrosoftAsync(string tokenMicrosoft);
        Task<String> RegistroEmailAsync(string email, string password, string nombreUsuario);
        Task<String> LoginEmailAsync(string email, string password);
    }
}
