using System.Collections.Generic;
using mini_ITS.Core.Database;
using mini_ITS.Core.Models;

namespace mini_ITS.Core.Tests.Repository
{
    public class EnrollmentsRepositoryTestsData
    {
        public static IEnumerable<SqlPagedQuery<Enrollments>> SqlPagedQueryCases
        {
            get
            {
                yield return new SqlPagedQuery<Enrollments>
                {
                    Filter = new List<SqlQueryCondition>()
                    {
                        new SqlQueryCondition
                        {
                            Name = "State",
                            Operator = SqlQueryOperator.Equal,
                            Value = "New"
                        }
                    },
                    SortColumnName = "DateAddEnrollment",
                    SortDirection = "ASC",
                    Page = 1,
                    ResultsPerPage = 3
                };
                yield return new SqlPagedQuery<Enrollments>
                {
                    Filter = new List<SqlQueryCondition>()
                    {
                        new SqlQueryCondition
                        {
                            Name = "State",
                            Operator = SqlQueryOperator.Equal,
                            Value = "Assigned"
                        }
                    },
                    SortColumnName = "DateAddEnrollment",
                    SortDirection = "DESC",
                    Page = 1,
                    ResultsPerPage = 3
                };
                yield return new SqlPagedQuery<Enrollments>
                {
                    Filter = new List<SqlQueryCondition>()
                    {
                        new SqlQueryCondition
                        {
                            Name = "DateEndEnrollment",
                            Operator = SqlQueryOperator.Is,
                            Value = "NULL"
                        }
                    },
                    SortColumnName = "DateAddEnrollment",
                    SortDirection = "ASC",
                    Page = 1,
                    ResultsPerPage = 3
                };
                yield return new SqlPagedQuery<Enrollments>
                {
                    Filter = new List<SqlQueryCondition>()
                    {
                        new SqlQueryCondition
                        {
                            Name = "Department",
                            Operator = SqlQueryOperator.Equal,
                            Value = "Managers"
                        }
                    },
                    SortColumnName = "DateAddEnrollment",
                    SortDirection = "DESC",
                    Page = 1,
                    ResultsPerPage = 3
                };
            }
        }
    }
}