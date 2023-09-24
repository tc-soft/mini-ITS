using System;
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
        public async Task<EnrollmentsPicture> GetAsync(Guid id)
        {
            using (var sqlConnection = new SqlConnection(_connectionString))
            {
                var sqlQueryBuilder = new SqlQueryBuilder<EnrollmentsPicture>()
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
                var enrollmentsPicture = await sqlConnection.QueryFirstOrDefaultAsync<EnrollmentsPicture>(sqlQueryBuilder.GetSelectQuery());
                return enrollmentsPicture;
            }
        }
        public async Task<IEnumerable<EnrollmentsPicture>> GetEnrollmentPicturesAsync(Guid id)
        {
            using (var sqlConnection = new SqlConnection(_connectionString))
            {
                var sqlQueryBuilder = new SqlQueryBuilder<EnrollmentsPicture>()
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
                    )
                    .WithSort(nameof(EnrollmentsPicture.DateAddPicture), "ASC"); ;
                var enrollmentsPicture = await sqlConnection.QueryAsync<EnrollmentsPicture>(sqlQueryBuilder.GetSelectQuery());
                return enrollmentsPicture;
            }
        }
        public async Task CreateAsync(EnrollmentsPicture enrollmentsPicture)
        {
            using (var sqlConnection = new SqlConnection(_connectionString))
            {
                var sqlQueryBuilder = new SqlQueryBuilder<EnrollmentsPicture>().GetInsertQuery();
                await sqlConnection.ExecuteAsync(sqlQueryBuilder, enrollmentsPicture);
            }
        }
        public async Task UpdateAsync(EnrollmentsPicture enrollmentsPicture)
        {
            using (var sqlConnection = new SqlConnection(_connectionString))
            {
                var sqlQueryBuilder = new SqlQueryBuilder<EnrollmentsPicture>().GetUpdateQuery();
                await sqlConnection.ExecuteAsync(sqlQueryBuilder, enrollmentsPicture);
            }
        }
        public async Task DeleteAsync(Guid id)
        {
            using (var sqlConnection = new SqlConnection(_connectionString))
            {
                var sqlQueryBuilder = new SqlQueryBuilder<EnrollmentsPicture>().GetDeleteQuery();
                await sqlConnection.ExecuteAsync(sqlQueryBuilder, new { Id = id });
            }
        }
    }
}