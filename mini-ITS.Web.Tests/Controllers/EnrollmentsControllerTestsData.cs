using System.Collections.Generic;
using System.Linq;
using mini_ITS.Core.Database;
using mini_ITS.Core.Dto;
using mini_ITS.Web.Models.UsersController;

namespace mini_ITS.Web.Tests.Controllers
{
    public class EnrollmentsControllerTestsData
    {
        public static IEnumerable<LoginData> LoginAuthorizedADMCases
        {
            get
            {
                yield return new LoginData
                {
                    Login = "admin",
                    Password = "admin"
                };
                yield return new LoginData
                {
                    Login = "baloddem",
                    Password = "Balo1234$%"
                };
                yield return new LoginData
                {
                    Login = "beukeida",
                    Password = "Beuk1234$%"
                };
                yield return new LoginData
                {
                    Login = "huntewil",
                    Password = "Hunt1234$%"
                };
            }
        }
        public static IEnumerable<LoginData> LoginAuthorizedMNGCases
        {
            get
            {
                yield return new LoginData
                {
                    Login = "atkincol",
                    Password = "Atki1234$%"
                };
                yield return new LoginData
                {
                    Login = "butchlau",
                    Password = "Butc1234$%"
                };
                yield return new LoginData
                {
                    Login = "cantrjov",
                    Password = "Cant1234$%"
                };
                yield return new LoginData
                {
                    Login = "pedroeva",
                    Password = "Pedr1234$%"
                };
            }
        }
        public static IEnumerable<LoginData> LoginAuthorizedUSRCases
        {
            get
            {
                yield return new LoginData
                {
                    Login = "farleeva",
                    Password = "Farl1234$%"
                };
                yield return new LoginData
                {
                    Login = "horbsaly",
                    Password = "Horb1234$%"
                };
                yield return new LoginData
                {
                    Login = "kirbyisa",
                    Password = "Kirb1234$%"
                };
                yield return new LoginData
                {
                    Login = "vissemar",
                    Password = "Viss1234$%"
                };
            }
        }
        public static IEnumerable<LoginData> LoginAuthorizedAllCases =>
            LoginAuthorizedADMCases
            .Concat(LoginAuthorizedMNGCases)
            .Concat(LoginAuthorizedUSRCases);
        public static IEnumerable<LoginData> LoginUnauthorizedCases
        {
            get
            {
                yield return new LoginData
                {
                    Login = "admin",
                    Password = ""
                };
                yield return new LoginData
                {
                    Login = "baloddem",
                    Password = "baloddem"
                };
                yield return new LoginData
                {
                    Login = "beukeida",
                    Password = "Beukeida2022#"
                };
                yield return new LoginData
                {
                    Login = "huntewil",
                    Password = "Xuntewil2022@"
                };
            }
        }
        public static IEnumerable<SqlPagedQuery<EnrollmentsDto>> SqlPagedQueryCases
        {
            get
            {
                yield return new SqlPagedQuery<EnrollmentsDto>
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
                yield return new SqlPagedQuery<EnrollmentsDto>
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
                yield return new SqlPagedQuery<EnrollmentsDto>
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
                yield return new SqlPagedQuery<EnrollmentsDto>
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