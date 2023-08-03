using mini_ITS.Core.Database;

namespace mini_ITS.Core.Repository
{
    public class GroupsRepository : IGroupsRepository
    {
        private readonly string _connectionString;

        public GroupsRepository(ISqlConnectionString sqlConnectionString)
        {
            _connectionString = sqlConnectionString.ConnectionString;
        }
    }
}