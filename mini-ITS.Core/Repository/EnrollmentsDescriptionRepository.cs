using mini_ITS.Core.Database;

namespace mini_ITS.Core.Repository
{
    public class EnrollmentsDescriptionRepository : IEnrollmentsDescriptionRepository
    {
        private readonly string _connectionString;

        public EnrollmentsDescriptionRepository(ISqlConnectionString sqlConnectionString)
        {
            _connectionString = sqlConnectionString.ConnectionString;
        }
    }
}