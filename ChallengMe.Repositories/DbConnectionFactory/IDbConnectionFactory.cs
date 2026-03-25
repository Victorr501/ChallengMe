using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChallengMe.Repositories.DbConnectionFactory
{
    public interface IDbConnectionFactory
    {
        IDbConnection CrearConexion();
    }
}
