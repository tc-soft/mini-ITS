using System.Collections.Generic;
using System.Data.SqlClient;
using System.Threading.Tasks;
using Dapper;
using mini_ITS.Core.Database;
using mini_ITS.Core.Models;

namespace mini_ITS.Core.Repository
{
    public class EnrollmentsRepository : IEnrollmentsRepository
    {
        private readonly string _connectionString;

        public EnrollmentsRepository(ISqlConnectionString sqlConnectionString)
        {
            _connectionString = sqlConnectionString.ConnectionString;
        }
        public async Task<IEnumerable<Enrollments>> GetAsync()
        {
            using (var sqlConnection = new SqlConnection(_connectionString))
            {
                var sqlQueryBuilder = new SqlQueryBuilder<Enrollments>()
                    .WithSort(nameof(Enrollments.DateAddEnrollment), "ASC");
                var enrollments = await sqlConnection.QueryAsync<Enrollments>(sqlQueryBuilder.GetSelectQuery());
                return enrollments;
            }
        }
    }
}