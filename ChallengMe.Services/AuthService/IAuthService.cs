using ChallengMe.Models.Auth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChallengMe.Services.AuthService
{
    public interface IAuthService
    {
        Task<AuthResultado> LogingMicrosoftAsync(string tokenMicrosoft);
    }
}
