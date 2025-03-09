using Microsoft.Extensions.Options;
using Microsoft.Data.SqlClient;
using mini_ITS.Core.Options;

namespace mini_ITS.Core.Database
{
    public class SqlConnectionString : ISqlConnectionString
    {
        private readonly DatabaseOptions _databaseOptions;
        private string _connectionString;

        public SqlConnectionString(IOptions<DatabaseOptions> databaseOptions)
        {
            _databaseOptions = databaseOptions.Value;
        }

        private string Build()
        {
            var builder = new SqlConnectionStringBuilder
            {
                DataSource = _databaseOptions.DataSource,
                InitialCatalog = _databaseOptions.InitialCatalog,
                PersistSecurityInfo = _databaseOptions.PersistSecurityInfo,
                IntegratedSecurity = _databaseOptions.IntegratedSecurity,
                UserID = _databaseOptions.UserId,
                Password = _databaseOptions.Password,
                ConnectTimeout = _databaseOptions.ConnectTimeout,
                Encrypt = true,
                TrustServerCertificate = true
            };

            _connectionString = builder.ToString();

            return _connectionString;
        }

        public string ConnectionString => string.IsNullOrWhiteSpace(_connectionString) ?
            Build() : _connectionString;
    }
}