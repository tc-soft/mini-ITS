using System;
using System.Collections.Generic;
using mini_ITS.Core.Database;
using mini_ITS.Core.Dto;
using mini_ITS.Web.Models.UsersController;

namespace mini_ITS.Web.Tests.Controllers
{
    public class UsersControllerTestsData
    {
        public static IEnumerable<LoginData> LoginAuthorizedCases
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
        public static IEnumerable<SqlPagedQuery<UsersDto>> SqlPagedQueryCases
        {
            get
            {
                yield return new SqlPagedQuery<UsersDto>
                {
                    Filter = new List<SqlQueryCondition>()
                    {
                        new SqlQueryCondition
                        {
                            Name = "Department",
                            Operator = SqlQueryOperator.Equal,
                            Value = "Sales"
                        },
                        new SqlQueryCondition
                        {
                            Name = "Role",
                            Operator = SqlQueryOperator.Equal,
                            Value = null
                        }
                    },
                    SortColumnName = "Login",
                    SortDirection = "DESC",
                    Page = 1,
                    ResultsPerPage = 5
                };
                yield return new SqlPagedQuery<UsersDto>
                {
                    Filter = new List<SqlQueryCondition>()
                    {
                        new SqlQueryCondition
                        {
                            Name = "Department",
                            Operator = SqlQueryOperator.Equal,
                            Value = "Development"
                        },
                        new SqlQueryCondition
                        {
                            Name = "Role",
                            Operator = SqlQueryOperator.Equal,
                            Value = "User"
                        }
                    },
                    SortColumnName = "FirstName",
                    SortDirection = "ASC",
                    Page = 1,
                    ResultsPerPage = 2
                };
                yield return new SqlPagedQuery<UsersDto>
                {
                    Filter = new List<SqlQueryCondition>()
                    {
                        new SqlQueryCondition
                        {
                            Name = "Department",
                            Operator = SqlQueryOperator.Equal,
                            Value = null
                        },
                        new SqlQueryCondition
                        {
                            Name = "Role",
                            Operator = SqlQueryOperator.Equal,
                            Value = null
                        }
                    },
                    SortColumnName = "Email",
                    SortDirection = "ASC",
                    Page = 1,
                    ResultsPerPage = 10
                };
                yield return new SqlPagedQuery<UsersDto>
                {
                    Filter = new List<SqlQueryCondition>()
                    {
                        new SqlQueryCondition
                        {
                            Name = "Department",
                            Operator = SqlQueryOperator.Equal,
                            Value = "Research"
                        },
                        new SqlQueryCondition
                        {
                            Name = "Role",
                            Operator = SqlQueryOperator.Equal,
                            Value = "Manager"
                        }
                    },
                    SortColumnName = "Login",
                    SortDirection = "ASC",
                    Page = 1,
                    ResultsPerPage = 3
                };
            }
        }
        public static IEnumerable<UsersDto> UsersCases
        {
            get
            {
                yield return new UsersDto
                {
                    Id = new Guid("DA30DCAC-27E8-4B53-B2C4-FE5220420106"),
                    Login = "bartlbri",
                    FirstName = "Brigita",
                    LastName = "Bartles",
                    Department = "Sales",
                    Email = "brigita.bartles@example.com",
                    Phone = "506XXX506",
                    Role = "Manager",
                    PasswordHash = "AQAAAAIAAYagAAAAECNMSp+6vCmYqr2MLQKmDVuIwLtvfxzC/rZ5fiDvoY05vDWxBuGmW7qC6BzMv+VZyA=="
                };
                yield return new UsersDto
                {
                    Id = new Guid("8532C003-D391-42AA-843A-21393B46C1A8"),
                    Login = "atkincol",
                    FirstName = "Colin",
                    LastName = "Atkins",
                    Department = "Marketing",
                    Email = "colin.atkins@example.com",
                    Phone = "505XXX505",
                    Role = "Manager",
                    PasswordHash = "AQAAAAIAAYagAAAAED+Au/PbiCL0SjMraBIwqPSgTjDpe13Xo8gCvN6iCvUFj4YNp4PihXhu2bzBgZl6Xg=="
                };
                yield return new UsersDto
                {
                    Id = new Guid("FAE887FB-ECAD-42E0-B9F3-7C0213BD1FF0"),
                    Login = "kirbyisa",
                    FirstName = "Isabella",
                    LastName = "Kirby",
                    Department = "Marketing",
                    Email = "isabella.kirby@example.com",
                    Phone = "505XXX505",
                    Role = "User",
                    PasswordHash = "AQAAAAIAAYagAAAAEHfxgcWZl1B++sf0Dnm7Un3vTER15OnDLOO+3qQCyC6DRB3zVLFhWS78Gil6UqL2Og=="
                };
                yield return new UsersDto
                {
                    Id = new Guid("9C94BCF2-00FA-4307-BF9B-AD70D8C8FC62"),
                    Login = "trevidor",
                    FirstName = "Dora",
                    LastName = "Trevino",
                    Department = "Sales",
                    Email = "dora.trevino@example.com",
                    Phone = "506XXX506",
                    Role = "User",
                    PasswordHash = "AQAAAAIAAYagAAAAEB5uzIxlCuMPub34FX9WmOdtVcLM4wWnNuqYTJiFkW2KTl8PPntmslcW+TFWgRmcIA=="
                };
            }
        }
        public static IEnumerable<UsersDto> CRUDCases
        {
            get
            {
                yield return new UsersDto
                {
                    Id = new Guid("9B62AEAC-A021-40E8-B865-3A0420B8AC77"),
                    Login = "gerbaemm",
                    FirstName = "Emma",
                    LastName = "Gerbani",
                    Department = "IT",
                    Email = "emma.gerbani@example.com",
                    Phone = "501XXX501",
                    Role = "Manager",
                    PasswordHash = "Gerbaemm2022@"
                };
                yield return new UsersDto
                {
                    Id = new Guid("036B4504-B062-4DEF-924A-C13741AFD686"),
                    Login = "trembays",
                    FirstName = "Ayse",
                    LastName = "Tremblay",
                    Department = "Research",
                    Email = "ayse.tremblay@example.com",
                    Phone = "502XXX502",
                    Role = "User",
                    PasswordHash = "Trembays2022@"
                };
                yield return new UsersDto
                {
                    Id = new Guid("725D991C-C332-4AAC-A319-3CE55295FE2E"),
                    Login = "gagnowil",
                    FirstName = "William",
                    LastName = "Gagnon",
                    Department = "Development",
                    Email = "william.gagnon@example.com",
                    Phone = "503XXX503",
                    Role = "Manager",
                    PasswordHash = "Gagnowil2022@"
                };
                yield return new UsersDto
                {
                    Id = new Guid("E5251D40-1402-4D25-8123-067DA24AEDAC"),
                    Login = "mortojac",
                    FirstName = "Jack",
                    LastName = "Morton",
                    Department = "Sales",
                    Email = "jack.morton@example.com",
                    Phone = "504XXX504",
                    Role = "Manager",
                    PasswordHash = "Mortojac2022@"
                };
            }
        }
    }
}