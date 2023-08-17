using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using mini_ITS.Core.Database;
using mini_ITS.Core.Dto;
using mini_ITS.Web.Models.UsersController;

namespace mini_ITS.Web.Tests.Controllers
{
    public class GroupsControllerTestsData
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
        public static IEnumerable<LoginData> LoginAuthorizedCreateCases =>
            LoginAuthorizedADMCases
            .Concat(LoginAuthorizedMNGCases);
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
        public static IEnumerable<TestCaseData> LoginUnauthorizedCreateCases
        {
            get
            {
                foreach (var loginUnauthorizedCases in GroupsControllerTestsData.LoginUnauthorizedCases)
                {
                    foreach (var groupsDto in GroupsControllerTestsData.CRUDCases)
                    {
                        yield return new TestCaseData(loginUnauthorizedCases, null, groupsDto);
                    }
                }

                foreach (var loginAuthorizedUSRCases in GroupsControllerTestsData.LoginAuthorizedUSRCases)
                {
                    foreach (var groupsDto in GroupsControllerTestsData.CRUDCases)
                    {
                        yield return new TestCaseData(null, loginAuthorizedUSRCases, groupsDto);
                    }
                }
            }
        }
        public static IEnumerable<SqlPagedQuery<GroupsDto>> SqlPagedQueryCases
        {
            get
            {
                yield return new SqlPagedQuery<GroupsDto>
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
                yield return new SqlPagedQuery<GroupsDto>
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
                yield return new SqlPagedQuery<GroupsDto>
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
                yield return new SqlPagedQuery<GroupsDto>
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
        public static IEnumerable<GroupsDto> CRUDCases
        {
            get
            {
                yield return new GroupsDto
                {
                    DateAddGroup = new DateTime(2023, 8, 1, 0, 0, 0),
                    DateModGroup = new DateTime(2023, 8, 1, 0, 0, 0),
                    UserAddGroup = new Guid("FCC06ACA-BE27-46FA-9142-BB1BA1322EB3"),
                    UserAddGroupFullName = "Admin Administrator",
                    UserModGroup = new Guid("FCC06ACA-BE27-46FA-9142-BB1BA1322EB3"),
                    UserModGroupFullName = "Admin Administrator",
                    GroupName = "Testing Titans"
                };
                yield return new GroupsDto
                {
                    DateAddGroup = new DateTime(2023, 8, 1, 0, 0, 0),
                    DateModGroup = new DateTime(2023, 8, 1, 0, 0, 0),
                    UserAddGroup = new Guid("FBE24C52-15AE-4C92-9C24-2C735D81EAE7"),
                    UserAddGroupFullName = "Demi Balode",
                    UserModGroup = new Guid("FBE24C52-15AE-4C92-9C24-2C735D81EAE7"),
                    UserModGroupFullName = "Demi Balode",
                    GroupName = "Beta Breakers"
                };
                yield return new GroupsDto
                {
                    DateAddGroup = new DateTime(2023, 8, 1, 0, 0, 0),
                    DateModGroup = new DateTime(2023, 8, 1, 0, 0, 0),
                    UserAddGroup = new Guid("FCC06ACA-BE27-46FA-9142-BB1BA1322EB3"),
                    UserAddGroupFullName = "Admin Administrator",
                    UserModGroup = new Guid("FCC06ACA-BE27-46FA-9142-BB1BA1322EB3"),
                    UserModGroupFullName = "Admin Administrator",
                    GroupName = "Quality Questers"
                };
                yield return new GroupsDto
                {
                    DateAddGroup = new DateTime(2023, 8, 1, 0, 0, 0),
                    DateModGroup = new DateTime(2023, 8, 1, 0, 0, 0),
                    UserAddGroup = new Guid("FBE24C52-15AE-4C92-9C24-2C735D81EAE7"),
                    UserAddGroupFullName = "Demi Balode",
                    UserModGroup = new Guid("FBE24C52-15AE-4C92-9C24-2C735D81EAE7"),
                    UserModGroupFullName = "Demi Balode",
                    GroupName = "Test Pilots United"
                };
            }
        }
    }
}