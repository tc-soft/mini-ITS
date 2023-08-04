using System.Collections.Generic;
using mini_ITS.Core.Database;
using mini_ITS.Core.Models;

namespace mini_ITS.Core.Tests.Repository
{
    public class GroupsRepositoryTestsData
    {
        public static IEnumerable<SqlPagedQuery<Groups>> SqlPagedQueryCases
        {
            get
            {
                yield return new SqlPagedQuery<Groups>
                {
                    Filter = new List<SqlQueryCondition>()
                    {
                        new SqlQueryCondition
                        {
                            Name = "UserAddGroupFullName",
                            Operator = SqlQueryOperator.Equal,
                            Value = "Admin Administrator"
                        }
                    },
                    SortColumnName = "GroupName",
                    SortDirection = "ASC",
                    Page = 1,
                    ResultsPerPage = 3
                };
                yield return new SqlPagedQuery<Groups>
                {
                    Filter = new List<SqlQueryCondition>()
                    {
                        new SqlQueryCondition
                        {
                            Name = "UserModGroupFullName",
                            Operator = SqlQueryOperator.Equal,
                            Value = "Demi Balode"
                        }
                    },
                    SortColumnName = "GroupName",
                    SortDirection = "DESC",
                    Page = 1,
                    ResultsPerPage = 3
                };
                yield return new SqlPagedQuery<Groups>
                {
                    Filter = new List<SqlQueryCondition>()
                    {
                        new SqlQueryCondition
                        {
                            Name = "UserAddGroupFullName",
                            Operator = SqlQueryOperator.Equal,
                            Value = null
                        }
                    },
                    SortColumnName = "GroupName",
                    SortDirection = "ASC",
                    Page = 1,
                    ResultsPerPage = 3
                };
                yield return new SqlPagedQuery<Groups>
                {
                    Filter = new List<SqlQueryCondition>()
                    {
                        new SqlQueryCondition
                        {
                            Name = "UserModGroupFullName",
                            Operator = SqlQueryOperator.Equal,
                            Value = null
                        }
                    },
                    SortColumnName = "GroupName",
                    SortDirection = "DESC",
                    Page = 1,
                    ResultsPerPage = 3
                };
            }
        }
    }
}