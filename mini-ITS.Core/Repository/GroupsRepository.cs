using System.Collections.Generic;
using System.Data.SqlClient;
using System.Threading.Tasks;
using Dapper;
using mini_ITS.Core.Database;
using mini_ITS.Core.Models;

namespace mini_ITS.Core.Repository
{
    public class GroupsRepository : IGroupsRepository
    {
        private readonly string _connectionString;

        public GroupsRepository(ISqlConnectionString sqlConnectionString)
        {
            _connectionString = sqlConnectionString.ConnectionString;
        }

        public async Task<IEnumerable<Groups>> GetAsync()
        {
            using (var sqlConnection = new SqlConnection(_connectionString))
            {
                var sqlQueryBuilder = new SqlQueryBuilder<Groups>()
                    .WithSort(nameof(Groups.GroupName), "ASC");
                var groups = await sqlConnection.QueryAsync<Groups>(sqlQueryBuilder.GetSelectQuery());
                return groups;
            }
        }
        public async Task<SqlPagedResult<Groups>> GetAsync(SqlPagedQuery<Groups> query)
        {
            using (var sqlConnection = new SqlConnection(_connectionString))
            {
                var sqlQueryBuilder = new SqlQueryBuilder<Groups>(query);
                var groups = await sqlConnection.QueryAsync<Groups>(sqlQueryBuilder.GetSelectQuery());
                var count = await sqlConnection.QueryFirstAsync<int>(sqlQueryBuilder.GetCountQuery());
                return SqlPagedResult<Groups>.Create(groups, query, count);
            }
        }
    }
}