using mini_ITS.Core.Database;

namespace mini_ITS.Core.Repository
{
    public class EnrollmentsRepository : IEnrollmentsRepository
    {
        private readonly string _connectionString;

        public EnrollmentsRepository(ISqlConnectionString sqlConnectionString)
        {
            _connectionString = sqlConnectionString.ConnectionString;
        }
    }
}