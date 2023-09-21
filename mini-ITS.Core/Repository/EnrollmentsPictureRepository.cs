using mini_ITS.Core.Database;

namespace mini_ITS.Core.Repository
{
    public class EnrollmentsPictureRepository : IEnrollmentsPictureRepository
    {
        private readonly string _connectionString;

        public EnrollmentsPictureRepository(ISqlConnectionString sqlConnectionString)
        {
            _connectionString = sqlConnectionString.ConnectionString;
        }
    }
}