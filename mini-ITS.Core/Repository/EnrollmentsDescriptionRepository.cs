using System.Collections.Generic;
using System.Data.SqlClient;
using System.Threading.Tasks;
using Dapper;
using mini_ITS.Core.Database;
using mini_ITS.Core.Models;

namespace mini_ITS.Core.Repository
{
    public class EnrollmentsDescriptionRepository : IEnrollmentsDescriptionRepository
    {
        private readonly string _connectionString;

        public EnrollmentsDescriptionRepository(ISqlConnectionString sqlConnectionString)
        {
            _connectionString = sqlConnectionString.ConnectionString;
        }
        public async Task<IEnumerable<EnrollmentsDescription>> GetAsync()
        {
            using (var sqlConnection = new SqlConnection(_connectionString))
            {
                var sqlQueryBuilder = new SqlQueryBuilder<EnrollmentsDescription>()
                    .WithSort(nameof(EnrollmentsDescription.DateAddDescription), "ASC");
                var enrollmentsDescription = await sqlConnection.QueryAsync<EnrollmentsDescription>(sqlQueryBuilder.GetSelectQuery());
                return enrollmentsDescription;
            }
        }
    }
}