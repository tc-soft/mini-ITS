using System;
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
        public async Task<EnrollmentsDescription> GetAsync(Guid id)
        {
            using (var sqlConnection = new SqlConnection(_connectionString))
            {
                var sqlQueryBuilder = new SqlQueryBuilder<EnrollmentsDescription>()
                    .WithFilter(
                        new List<SqlQueryCondition>()
                        {
                            new SqlQueryCondition
                            {
                                Name = "Id",
                                Operator = SqlQueryOperator.Equal,
                                Value = new string(id.ToString())
                            }
                        }
                    );
                var enrollmentsDescription = await sqlConnection.QueryFirstOrDefaultAsync<EnrollmentsDescription>(sqlQueryBuilder.GetSelectQuery());
                return enrollmentsDescription;
            }
        }
        public async Task<IEnumerable<EnrollmentsDescription>> GetEnrollmentDescriptionsAsync(Guid id)
        {
            using (var sqlConnection = new SqlConnection(_connectionString))
            {
                var sqlQueryBuilder = new SqlQueryBuilder<EnrollmentsDescription>()
                    .WithFilter(
                        new List<SqlQueryCondition>()
                        {
                            new SqlQueryCondition
                            {
                                Name = "EnrollmentId",
                                Operator = SqlQueryOperator.Equal,
                                Value = new string(id.ToString())
                            }
                        }
                    );
                var enrollmentsDescription = await sqlConnection.QueryAsync<EnrollmentsDescription>(sqlQueryBuilder.GetSelectQuery());
                return enrollmentsDescription;
            }
        }
        public async Task CreateAsync(EnrollmentsDescription enrollmentsDescription)
        {
            using (var sqlConnection = new SqlConnection(_connectionString))
            {
                var sqlQueryBuilder = new SqlQueryBuilder<EnrollmentsDescription>().GetInsertQuery();
                await sqlConnection.ExecuteAsync(sqlQueryBuilder, enrollmentsDescription);
            }
        }
        public async Task UpdateAsync(EnrollmentsDescription enrollmentsDescription)
        {
            using (var sqlConnection = new SqlConnection(_connectionString))
            {
                var sqlQueryBuilder = new SqlQueryBuilder<EnrollmentsDescription>().GetUpdateQuery();
                await sqlConnection.ExecuteAsync(sqlQueryBuilder, enrollmentsDescription);
            }
        }
        public async Task DeleteAsync(Guid id)
        {
            using (var sqlConnection = new SqlConnection(_connectionString))
            {
                var sqlQueryBuilder = new SqlQueryBuilder<EnrollmentsDescription>().GetDeleteQuery();
                await sqlConnection.ExecuteAsync(sqlQueryBuilder, new { Id = id });
            }
        }
    }
}