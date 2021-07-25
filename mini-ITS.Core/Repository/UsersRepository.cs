using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Threading.Tasks;
using Dapper;
using mini_ITS.Core.Database;
using mini_ITS.Core.Models;

namespace mini_ITS.Core.Repository
{
    public class UsersRepository : IUsersRepository
    {
        private readonly string _connectionString;

        public UsersRepository(ISqlConnectionString sqlConnectionString)
        {
            _connectionString = sqlConnectionString.ConnectionString;
        }
        public async Task<IEnumerable<Users>> GetAsync()
        {
            using (var sqlConnection = new SqlConnection(_connectionString))
            {
                var sqlQueryBuilder = new SqlQueryBuilder<Users>()
                    .WithSort(nameof(Users.Login), "ASC");
                var users = await sqlConnection.QueryAsync<Users>(sqlQueryBuilder.GetSelectQuery());
                return users;
            }
        }
        public async Task<IEnumerable<Users>> GetAsync(string department, string role)
        {
            using (var sqlConnection = new SqlConnection(_connectionString))
            {
                var sqlQueryBuilder = new SqlQueryBuilder<Users>()
                    .WithFilter(
                        new List<SqlQueryCondition>()
                        {
                            role == null ? null :
                            new SqlQueryCondition
                            {
                                Name = "Role",
                                Operator = SqlQueryOperator.Equal,
                                Value = new string(role)

                            },
                            department == null ? null :
                            new SqlQueryCondition
                            {
                                Name = "Department",
                                Operator = SqlQueryOperator.Equal,
                                Value = new string(department)
                            }
                        }
                    )
                    .WithSort(nameof(Users.Login), "ASC");
                var users = await sqlConnection.QueryAsync<Users>(sqlQueryBuilder.GetSelectQuery());
                return users;
            }
        }
        public async Task<IEnumerable<Users>> GetAsync(List<SqlQueryCondition> sqlQueryConditionList)
        {
            using (var sqlConnection = new SqlConnection(_connectionString))
            {
                var sqlQueryBuilder = new SqlQueryBuilder<Users>()
                    .WithFilter(
                        sqlQueryConditionList
                    )
                    .WithSort(nameof(Users.Login), "ASC");
                var users = await sqlConnection.QueryAsync<Users>(sqlQueryBuilder.GetSelectQuery());
                return users;
            }
        }
        public async Task<SqlPagedResult<Users>> GetAsync(SqlPagedQuery<Users> sqlPagedQuery)
        {
            using (var sqlConnection = new SqlConnection(_connectionString))
            {
                var sqlQueryBuilder = new SqlQueryBuilder<Users>(sqlPagedQuery);
                var users = await sqlConnection.QueryAsync<Users>(sqlQueryBuilder.GetSelectQuery());
                var count = await sqlConnection.QueryFirstAsync<int>(sqlQueryBuilder.GetCountQuery());
                return SqlPagedResult<Users>.Create(users, sqlPagedQuery, count);
            }
        }
        public async Task<Users> GetAsync(Guid id)
        {
            using (var sqlConnection = new SqlConnection(_connectionString))
            {
                var sqlQueryBuilder = new SqlQueryBuilder<Users>()
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
                var user = await sqlConnection.QueryFirstOrDefaultAsync<Users>(sqlQueryBuilder.GetSelectQuery());
                return user;
            }
        }
        public async Task<Users> GetAsync(string login)
        {
            using (var sqlConnection = new SqlConnection(_connectionString))
            {
                var sqlQueryBuilder = new SqlQueryBuilder<Users>()
                    .WithFilter(
                        new List<SqlQueryCondition>()
                        {
                            new SqlQueryCondition
                            {
                                Name = "Login",
                                Operator = SqlQueryOperator.Equal,
                                Value = new string(login)
                            }
                        }
                    );
                var user = await sqlConnection.QueryFirstOrDefaultAsync<Users>(sqlQueryBuilder.GetSelectQuery());
                return user;
            }
        }
        public async Task CreateAsync(Users user)
        {
            using (var sqlConnection = new SqlConnection(_connectionString))
            {
                var sqlQueryBuilder = new SqlQueryBuilder<Users>().GetInsertQuery();
                await sqlConnection.ExecuteAsync(sqlQueryBuilder, user);
            }
        }
        public async Task UpdateAsync(Users user)
        {
            using (var sqlConnection = new SqlConnection(_connectionString))
            {
                var sqlQueryBuilder = new SqlQueryBuilder<Users>().GetUpdateQuery();
                await sqlConnection.ExecuteAsync(sqlQueryBuilder, user);
            }
        }
        public async Task DeleteAsync(Guid id)
        {
            using (var sqlConnection = new SqlConnection(_connectionString))
            {
                var sqlQueryBuilder = new SqlQueryBuilder<Users>().GetDeleteQuery();
                await sqlConnection.ExecuteAsync(sqlQueryBuilder, new { Id = id });
            }
        }
        public async Task SetPasswordAsync(Guid id, string passwordHash)
        {
            using (var sqlConnection = new SqlConnection(_connectionString))
            {
                var sqlQueryBuilder = new SqlQueryBuilder<Users>().GetUpdateItemQuery("PasswordHash");
                await sqlConnection.ExecuteAsync(sqlQueryBuilder, new { PasswordHash = passwordHash, Id = id });
            }
        }
    }
}