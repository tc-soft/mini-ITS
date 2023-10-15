using System;
using System.Collections.Generic;
using System.Linq;
using mini_ITS.Core.Database;
using mini_ITS.Core.Dto;
using mini_ITS.Core.Models;
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
        public static IEnumerable<EnrollmentsDto> CRUDCases
        {
            get
            {
                yield return new EnrollmentsDto
                {
                    DateEndEnrollment = null,
                    DateEndDeclareByUser = new DateTime(2023, 9, 8, 7, 0, 0),
                    DateEndDeclareByDepartment = null,
                    DateEndDeclareByDepartmentUser = new Guid(),
                    DateEndDeclareByDepartmentUserFullName = null,
                    Department = "Managers",
                    Description = "Test enrollment 11",
                    Group = "Alfa Avengers",
                    Priority = PriorityValues.Normal,
                    SMSToUserInfo = false,
                    SMSToAllInfo = false,
                    MailToUserInfo = false,
                    MailToAllInfo = false,
                    ReadyForClose = false,
                    UserAddEnrollment = new Guid("42E1156F-1F81-41E6-A5FB-1CBAB26F7F47"),
                    UserAddEnrollmentFullName = "Lana Raisic",
                    UserEndEnrollment = new Guid(),
                    UserEndEnrollmentFullName = null,
                    UserReeEnrollment = new Guid(),
                    UserReeEnrollmentFullName = null,
                    ActionRequest = 0,
                    ActionExecuted = 0,
                    ActionFinished = false
                };
                yield return new EnrollmentsDto
                {
                    DateEndEnrollment = null,
                    DateEndDeclareByUser = new DateTime(2023, 9, 9, 7, 0, 0),
                    DateEndDeclareByDepartment = null,
                    DateEndDeclareByDepartmentUser = new Guid(),
                    DateEndDeclareByDepartmentUserFullName = null,
                    Department = "Development",
                    Description = "Test enrollment 12",
                    Group = "DevOps Dynamos",
                    Priority = PriorityValues.High,
                    SMSToUserInfo = false,
                    SMSToAllInfo = false,
                    MailToUserInfo = false,
                    MailToAllInfo = false,
                    ReadyForClose = false,
                    UserAddEnrollment = new Guid("FFE8CAB9-C2A9-4173-B00B-509A4BA737C8"),
                    UserAddEnrollmentFullName = "Kathie Marsh",
                    UserEndEnrollment = new Guid(),
                    UserEndEnrollmentFullName = null,
                    UserReeEnrollment = new Guid(),
                    UserReeEnrollmentFullName = null,
                    ActionRequest = 0,
                    ActionExecuted = 0,
                    ActionFinished = false
                };
                yield return new EnrollmentsDto
                {
                    DateEndEnrollment = null,
                    DateEndDeclareByUser = new DateTime(2023, 9, 10, 7, 0, 0),
                    DateEndDeclareByDepartment = null,
                    DateEndDeclareByDepartmentUser = new Guid(),
                    DateEndDeclareByDepartmentUserFullName = null,
                    Department = "Research",
                    Description = "Test enrollment 13",
                    Group = "Maverick Testers",
                    Priority = PriorityValues.Critical,
                    SMSToUserInfo = false,
                    SMSToAllInfo = false,
                    MailToUserInfo = false,
                    MailToAllInfo = false,
                    ReadyForClose = false,
                    UserAddEnrollment = new Guid("61008777-3BCA-4470-817D-35CFBD465921"),
                    UserAddEnrollmentFullName = "Peter Cottle",
                    UserEndEnrollment = new Guid(),
                    UserEndEnrollmentFullName = null,
                    UserReeEnrollment = new Guid(),
                    UserReeEnrollmentFullName = null,
                    ActionRequest = 0,
                    ActionExecuted = 0,
                    ActionFinished = false
                };
                yield return new EnrollmentsDto
                {
                    DateEndEnrollment = null,
                    DateEndDeclareByUser = new DateTime(2023, 9, 11, 7, 0, 0),
                    DateEndDeclareByDepartment = null,
                    DateEndDeclareByDepartmentUser = new Guid(),
                    DateEndDeclareByDepartmentUserFullName = null,
                    Department = "IT",
                    Description = "Test enrollment 14",
                    Group = "Project Orion",
                    Priority = PriorityValues.Normal,
                    SMSToUserInfo = false,
                    SMSToAllInfo = false,
                    MailToUserInfo = false,
                    MailToAllInfo = false,
                    ReadyForClose = false,
                    UserAddEnrollment = new Guid("84249845-9D8E-48FD-898C-16BE0433BE66"),
                    UserAddEnrollmentFullName = "Michalina Yaveos",
                    UserEndEnrollment = new Guid(),
                    UserEndEnrollmentFullName = null,
                    UserReeEnrollment = new Guid(),
                    UserReeEnrollmentFullName = null,
                    ActionRequest = 0,
                    ActionExecuted = 0,
                    ActionFinished = false
                };
            }
        }
    }
}