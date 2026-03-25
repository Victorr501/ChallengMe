using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChallengMe.AzureAD.AzureAd
{
    public interface IAzureAdService
    {
        Task<string> ValidarTokenYObtenerEmailAsync(string tokenMicrosoft);
    }
}
