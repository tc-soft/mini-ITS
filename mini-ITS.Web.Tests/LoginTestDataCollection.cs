using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using mini_ITS.Core.Tests;
using mini_ITS.Web.Models.UsersController;

namespace mini_ITS.Web.Tests
{
    public class LoginTestDataCollection
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
        public static IEnumerable<LoginData> LoginAuthorizedCases =>
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

        //Enrollments
        public static IEnumerable<LoginData> LoginAuthorizedIndexEnrollmentCases =>
            LoginAuthorizedCases;
        public static IEnumerable<LoginData> LoginAuthorizedCreateEnrollmentCases =>
            LoginAuthorizedCases;
        public static IEnumerable<LoginData> LoginAuthorizedEditEnrollmentCases =>
            LoginAuthorizedCases;
        public static IEnumerable<LoginData> LoginAuthorizedDeleteEnrollmentCases =>
            LoginAuthorizedADMCases
            .Concat(LoginAuthorizedMNGCases);
        public static IEnumerable<LoginData> LoginUnauthorizedIndexEnrollmentCases =>
            LoginUnauthorizedCases;
        public static IEnumerable<LoginData> LoginUnauthorizedCreateEnrollmentCases =>
            LoginUnauthorizedCases;
        public static IEnumerable<LoginData> LoginUnauthorizedEditEnrollmentCases =>
            LoginUnauthorizedCases;
        public static IEnumerable<TestCaseData> LoginUnauthorizedDeleteEnrollmentCases
        {
            get
            {
                foreach (var loginUnauthorizedCases in LoginUnauthorizedCases)
                {
                    foreach (var enrollmentDto in EnrollmentsTestsData.EnrollmentsCasesDto)
                    {
                        yield return new TestCaseData(loginUnauthorizedCases, null, enrollmentDto);
                    }
                }

                foreach (var loginAuthorizedUSRCases in LoginAuthorizedUSRCases)
                {
                    foreach (var enrollmentDto in EnrollmentsTestsData.EnrollmentsCasesDto)
                    {
                        yield return new TestCaseData(null, loginAuthorizedUSRCases, enrollmentDto);
                    }
                }
            }
        }

        //EnrollmentsDescription
        public static IEnumerable<LoginData> LoginAuthorizedDeleteEnrollmentDescriptionCases =>
            LoginAuthorizedADMCases
            .Concat(LoginAuthorizedMNGCases);
        public static IEnumerable<TestCaseData> LoginUnauthorizedDeleteEnrollmentDescriptionCases
        {
            get
            {
                foreach (var loginUnauthorizedCases in LoginUnauthorizedCases)
                {
                    foreach (var enrollmentsDescriptionDto in EnrollmentsDescriptionTestsData.EnrollmentsDescriptionCasesDto)
                    {
                        yield return new TestCaseData(loginUnauthorizedCases, null, enrollmentsDescriptionDto);
                    }
                }

                foreach (var loginAuthorizedUSRCases in LoginAuthorizedUSRCases)
                {
                    foreach (var enrollmentsDescriptionDto in EnrollmentsDescriptionTestsData.EnrollmentsDescriptionCasesDto)
                    {
                        yield return new TestCaseData(null, loginAuthorizedUSRCases, enrollmentsDescriptionDto);
                    }
                }
            }
        }

        //EnrollmentsPicture
        public static IEnumerable<LoginData> LoginAuthorizedDeleteEnrollmentPictureCases =>
            LoginAuthorizedADMCases
            .Concat(LoginAuthorizedMNGCases)
            .Concat(LoginAuthorizedUSRCases);
        public static IEnumerable<TestCaseData> LoginUnauthorizedDeleteEnrollmentPictureCases
        {
            get
            {
                foreach (var loginUnauthorizedCases in LoginUnauthorizedCases)
                {
                    foreach (var enrollmentsPictureDto in EnrollmentsPictureTestsData.EnrollmentsPictureCasesDto)
                    {
                        yield return new TestCaseData(loginUnauthorizedCases, null, enrollmentsPictureDto);
                    }
                }
            }
        }

        //Groups
        public static IEnumerable<LoginData> LoginAuthorizedIndexGroupCases =>
            LoginAuthorizedCases;
        public static IEnumerable<LoginData> LoginAuthorizedCreateGroupCases =>
            LoginAuthorizedADMCases
            .Concat(LoginAuthorizedMNGCases);
        public static IEnumerable<LoginData> LoginAuthorizedEditGroupCases =>
            LoginAuthorizedADMCases
            .Concat(LoginAuthorizedMNGCases);
        public static IEnumerable<LoginData> LoginAuthorizedDeleteGroupCases =>
            LoginAuthorizedADMCases
            .Concat(LoginAuthorizedMNGCases);
        public static IEnumerable<LoginData> LoginUnauthorizedIndexGroupCases =>
            LoginUnauthorizedCases;
        public static IEnumerable<TestCaseData> LoginUnauthorizedCreateGroupCases
        {
            get
            {
                foreach (var loginUnauthorizedCases in LoginUnauthorizedCases)
                {
                    foreach (var groupsDto in GroupsTestsData.CRUDCasesDto)
                    {
                        yield return new TestCaseData(loginUnauthorizedCases, null, groupsDto);
                    }
                }

                foreach (var loginAuthorizedUSRCases in LoginAuthorizedUSRCases)
                {
                    foreach (var groupsDto in GroupsTestsData.CRUDCasesDto)
                    {
                        yield return new TestCaseData(null, loginAuthorizedUSRCases, groupsDto);
                    }
                }
            }
        }
        public static IEnumerable<TestCaseData> LoginUnauthorizedEditGroupCases
        {
            get
            {
                foreach (var loginUnauthorizedCases in LoginUnauthorizedCases)
                {
                    foreach (var groupsDto in GroupsTestsData.GroupsCasesDto)
                    {
                        yield return new TestCaseData(loginUnauthorizedCases, null, groupsDto);
                    }
                }

                foreach (var loginAuthorizedUSRCases in LoginAuthorizedUSRCases)
                {
                    foreach (var groupsDto in GroupsTestsData.GroupsCasesDto)
                    {
                        yield return new TestCaseData(null, loginAuthorizedUSRCases, groupsDto);
                    }
                }
            }
        }
        public static IEnumerable<TestCaseData> LoginUnauthorizedDeleteGroupCases
        {
            get
            {
                foreach (var loginUnauthorizedCases in LoginUnauthorizedCases)
                {
                    foreach (var groupsDto in GroupsTestsData.GroupsCasesDto)
                    {
                        yield return new TestCaseData(loginUnauthorizedCases, null, groupsDto);
                    }
                }

                foreach (var loginAuthorizedUSRCases in LoginAuthorizedUSRCases)
                {
                    foreach (var groupsDto in GroupsTestsData.GroupsCasesDto)
                    {
                        yield return new TestCaseData(null, loginAuthorizedUSRCases, groupsDto);
                    }
                }
            }
        }

        //Users
        public static IEnumerable<LoginData> LoginAuthorizedIndexUserCases =>
            LoginAuthorizedADMCases;
        public static IEnumerable<LoginData> LoginAuthorizedCreateUserCases =>
            LoginAuthorizedADMCases;
        public static IEnumerable<LoginData> LoginAuthorizedEditUserCases =>
            LoginAuthorizedADMCases;
        public static IEnumerable<LoginData> LoginAuthorizedDeleteUserCases =>
            LoginAuthorizedADMCases;
        public static IEnumerable<LoginData> LoginAuthorizedChangePasswordUserCases =>
            LoginAuthorizedADMCases;
        public static IEnumerable<LoginData> LoginAuthorizedSetPasswordUserCases =>
            LoginAuthorizedADMCases;
        public static IEnumerable<TestCaseData> LoginUnauthorizedIndexUserCases
        {
            get
            {
                foreach (var loginUnauthorizedCases in LoginUnauthorizedCases)
                {
                    foreach (var sqlPagedQueryDto in UsersTestsData.SqlPagedQueryCasesDto)
                    {
                        yield return new TestCaseData(loginUnauthorizedCases, null, sqlPagedQueryDto);
                    }
                }

                foreach (var loginAuthorizedUSRCases in LoginAuthorizedUSRCases)
                {
                    foreach (var sqlPagedQueryDto in UsersTestsData.SqlPagedQueryCasesDto)
                    {
                        yield return new TestCaseData(null, loginAuthorizedUSRCases, sqlPagedQueryDto);
                    }
                }

                foreach (var loginAuthorizedMGRCases in LoginAuthorizedMNGCases)
                {
                    foreach (var sqlPagedQueryDto in UsersTestsData.SqlPagedQueryCasesDto)
                    {
                        yield return new TestCaseData(null, loginAuthorizedMGRCases, sqlPagedQueryDto);
                    }
                }
            }
        }
        public static IEnumerable<TestCaseData> LoginUnauthorizedCreateUserCases
        {
            get
            {
                foreach (var loginUnauthorizedCases in LoginUnauthorizedCases)
                {
                    foreach (var usersDto in UsersTestsData.CRUDCasesDto)
                    {
                        yield return new TestCaseData(loginUnauthorizedCases, null, usersDto);
                    }
                }

                foreach (var loginAuthorizedUSRCases in LoginAuthorizedUSRCases)
                {
                    foreach (var usersDto in UsersTestsData.CRUDCasesDto)
                    {
                        yield return new TestCaseData(null, loginAuthorizedUSRCases, usersDto);
                    }
                }

                foreach (var loginAuthorizedMGRCases in LoginAuthorizedMNGCases)
                {
                    foreach (var usersDto in UsersTestsData.CRUDCasesDto)
                    {
                        yield return new TestCaseData(null, loginAuthorizedMGRCases, usersDto);
                    }
                }
            }
        }
        public static IEnumerable<TestCaseData> LoginUnauthorizedEditUserCases
        {
            get
            {
                foreach (var loginUnauthorizedCases in LoginUnauthorizedCases)
                {
                    foreach (var usersDto in UsersTestsData.UsersCasesDto)
                    {
                        yield return new TestCaseData(loginUnauthorizedCases, null, usersDto);
                    }
                }

                foreach (var loginAuthorizedUSRCases in LoginAuthorizedUSRCases)
                {
                    foreach (var usersDto in UsersTestsData.UsersCasesDto)
                    {
                        yield return new TestCaseData(null, loginAuthorizedUSRCases, usersDto);
                    }
                }

                foreach (var loginAuthorizedMGRCases in LoginAuthorizedMNGCases)
                {
                    foreach (var usersDto in UsersTestsData.UsersCasesDto)
                    {
                        yield return new TestCaseData(null, loginAuthorizedMGRCases, usersDto);
                    }
                }
            }
        }
        public static IEnumerable<TestCaseData> LoginUnauthorizedDeleteUserCases
        {
            get
            {
                foreach (var loginUnauthorizedCases in LoginUnauthorizedCases)
                {
                    foreach (var usersDto in UsersTestsData.UsersCasesDto)
                    {
                        yield return new TestCaseData(loginUnauthorizedCases, null, usersDto);
                    }
                }

                foreach (var loginAuthorizedUSRCases in LoginAuthorizedUSRCases)
                {
                    foreach (var usersDto in UsersTestsData.UsersCasesDto)
                    {
                        yield return new TestCaseData(null, loginAuthorizedUSRCases, usersDto);
                    }
                }

                foreach (var loginAuthorizedMGRCases in LoginAuthorizedMNGCases)
                {
                    foreach (var usersDto in UsersTestsData.UsersCasesDto)
                    {
                        yield return new TestCaseData(null, loginAuthorizedMGRCases, usersDto);
                    }
                }
            }
        }
        public static IEnumerable<TestCaseData> LoginUnauthorizedChangePasswordUserCases
        {
            get
            {
                foreach (var loginUnauthorizedCases in LoginUnauthorizedCases)
                {
                    foreach (var usersDto in UsersTestsData.UsersCasesDto)
                    {
                        yield return new TestCaseData(loginUnauthorizedCases, null, usersDto);
                    }
                }

                foreach (var loginAuthorizedUSRCases in LoginAuthorizedUSRCases)
                {
                    foreach (var usersDto in UsersTestsData.UsersCasesDto)
                    {
                        yield return new TestCaseData(null, loginAuthorizedUSRCases, usersDto);
                    }
                }

                foreach (var loginAuthorizedMGRCases in LoginAuthorizedMNGCases)
                {
                    foreach (var usersDto in UsersTestsData.UsersCasesDto)
                    {
                        yield return new TestCaseData(null, loginAuthorizedMGRCases, usersDto);
                    }
                }
            }
        }
        public static IEnumerable<TestCaseData> LoginUnauthorizedSetPasswordUserCases
        {
            get
            {
                foreach (var loginUnauthorizedCases in LoginUnauthorizedCases)
                {
                    foreach (var usersDto in UsersTestsData.CRUDCasesDto)
                    {
                        yield return new TestCaseData(loginUnauthorizedCases, null, usersDto);
                    }
                }

                foreach (var loginAuthorizedUSRCases in LoginAuthorizedUSRCases)
                {
                    foreach (var usersDto in UsersTestsData.CRUDCasesDto)
                    {
                        yield return new TestCaseData(null, loginAuthorizedUSRCases, usersDto);
                    }
                }

                foreach (var loginAuthorizedMGRCases in LoginAuthorizedMNGCases)
                {
                    foreach (var usersDto in UsersTestsData.CRUDCasesDto)
                    {
                        yield return new TestCaseData(null, loginAuthorizedMGRCases, usersDto);
                    }
                }
            }
        }
    }
}