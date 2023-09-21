using System.Collections.Generic;
using System.Data.SqlClient;
using System.Threading.Tasks;
using Dapper;
using mini_ITS.Core.Database;
using mini_ITS.Core.Models;

namespace mini_ITS.Core.Repository
{
    public class EnrollmentsPictureRepository : IEnrollmentsPictureRepository
    {
        private readonly string _connectionString;

        public EnrollmentsPictureRepository(ISqlConnectionString sqlConnectionString)
        {
            _connectionString = sqlConnectionString.ConnectionString;
        }
        public async Task<IEnumerable<EnrollmentsPicture>> GetAsync()
        {
            using (var sqlConnection = new SqlConnection(_connectionString))
            {
                var sqlQueryBuilder = new SqlQueryBuilder<EnrollmentsPicture>()
                    .WithSort(nameof(EnrollmentsPicture.DateAddPicture), "ASC");
                var enrollmentsPicture = await sqlConnection.QueryAsync<EnrollmentsPicture>(sqlQueryBuilder.GetSelectQuery());
                return enrollmentsPicture;
            }
        }
    }
}