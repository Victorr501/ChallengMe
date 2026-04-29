using ChallengMe.Repositories.DbConnectionFactory;
using System.Data;

namespace ChallengMe.Tests.API.Helpers
{
    public class FakeDbConnectionFactory : IDbConnectionFactory
    {
        private readonly IDbConnection _conexion;

        public FakeDbConnectionFactory(IDbConnection conexion)
        {
            _conexion = conexion;
        }

        public IDbConnection CrearConexion()
        {
            return new NoCloseConnection(_conexion);
        }
    }

    public class NoCloseConnection : IDbConnection
    {
        private readonly IDbConnection _inner;

        public NoCloseConnection(IDbConnection inner)
        {
            _inner = inner;
        }

        public void Close() { }
        public void Dispose() { }

        public string ConnectionString
        {
            get => _inner.ConnectionString;
            set => _inner.ConnectionString = value;
        }

        public int ConnectionTimeout => _inner.ConnectionTimeout;
        public string Database => _inner.Database;
        public ConnectionState State => _inner.State;

        public IDbTransaction BeginTransaction() => _inner.BeginTransaction();
        public IDbTransaction BeginTransaction(IsolationLevel il) => _inner.BeginTransaction(il);
        public void ChangeDatabase(string databaseName) => _inner.ChangeDatabase(databaseName);
        public IDbCommand CreateCommand() => _inner.CreateCommand();
        public void Open() => _inner.Open();
    }
}