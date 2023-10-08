using System;
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
        public async Task<SqlPagedResult<Enrollments>> GetAsync(SqlPagedQuery<Enrollments> query)
        {
            using (var sqlConnection = new SqlConnection(_connectionString))
            {
                var sqlQueryBuilder = new SqlQueryBuilder<Enrollments>(query);
                var enrollments = await sqlConnection.QueryAsync<Enrollments>(sqlQueryBuilder.GetSelectQuery());
                var count = await sqlConnection.QueryFirstAsync<int>(sqlQueryBuilder.GetCountQuery());
                return SqlPagedResult<Enrollments>.Create(enrollments, query, count);
            }
        }
        public async Task<Enrollments> GetAsync(Guid id)
        {
            using (var sqlConnection = new SqlConnection(_connectionString))
            {
                var sqlQueryBuilder = new SqlQueryBuilder<Enrollments>()
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
                var enrollments = await sqlConnection.QueryFirstOrDefaultAsync<Enrollments>(sqlQueryBuilder.GetSelectQuery());
                return enrollments;
            }
        }
        public async Task<int> GetMaxNumberAsync(int year)
        {
            using (var sqlConnection = new SqlConnection(_connectionString))
            {
                var query = @"SELECT COALESCE(MAX(Nr),0) FROM Enrollments WHERE Year = @Year";
                var nr = await sqlConnection.QueryFirstOrDefaultAsync<int>(query, new { Year = year });
                return nr;
            }
        }
        public async Task CreateAsync(Enrollments enrollments)
        {
            using (var sqlConnection = new SqlConnection(_connectionString))
            {
                var sqlQueryBuilder = new SqlQueryBuilder<Enrollments>().GetInsertQuery();
                await sqlConnection.ExecuteAsync(sqlQueryBuilder, enrollments);
            }
        }
    }
}