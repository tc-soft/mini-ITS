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
                    Password = "Baloddem2022@"
                };
                yield return new LoginData
                {
                    Login = "beukeida",
                    Password = "Beukeida2022@"
                };
                yield return new LoginData
                {
                    Login = "huntewil",
                    Password = "Huntewil2022@"
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
                    Id = new Guid("5ee56913-7441-4305-8b31-bc86584fff47"),
                    Login = "bartlbri",
                    FirstName = "Brigita",
                    LastName = "Bartles",
                    Department = "Sales",
                    Email = "brigita.bartles@example.com",
                    Phone = "505XXX505",
                    Role = "User",
                    PasswordHash = "AQAAAAEAACcQAAAAEA/sCjbUn+XYqOX2IvkfPOVDuk55tm6+dAvF3tRKOZtSXFFlog18TLlgf+qZYbz0Eg=="
                };
                yield return new UsersDto
                {
                    Id = new Guid("3131c3ea-5607-4fa0-b9d7-712ff41baa4e"),
                    Login = "atkincol",
                    FirstName = "Colin",
                    LastName = "Atkins",
                    Department = "Marketing",
                    Email = "colin.atkins@example.com",
                    Phone = "507XXX507",
                    Role = "Manager",
                    PasswordHash = "AQAAAAEAACcQAAAAECI5D9hIHXvaVpudtHvyvLqa+p0SwNhPQ0R53T8jrkVUEQ83Teaqb7c8Vkv9O+KtZw=="
                };
                yield return new UsersDto
                {
                    Id = new Guid("dfe4d2bf-08ea-4d86-9ccd-4e1ce3459c48"),
                    Login = "kirbyisa",
                    FirstName = "Isabella",
                    LastName = "Kirby",
                    Department = "Marketing",
                    Email = "isabella.kirby@example.com",
                    Phone = "507XXX507",
                    Role = "Manager",
                    PasswordHash = "AQAAAAEAACcQAAAAELS+ZaS667+SzIjNLuFk9YJV6yvLK85cs2A6wZd9chyjgsHk0gPt2FXbZVxFSASfuw=="
                };
                yield return new UsersDto
                {
                    Id = new Guid("99fcf2cf-9080-4c61-bd3d-66f78ce4e39f"),
                    Login = "trevidor",
                    FirstName = "Dora",
                    LastName = "Trevino",
                    Department = "Sales",
                    Email = "dora.trevino@example.com",
                    Phone = "509XXX509",
                    Role = "User",
                    PasswordHash = "AQAAAAEAACcQAAAAEB6m7DEIjclUTy6LMf7lGSelKRBp0ZstF9Fm+YW/Yq/Ta4NcZCaRKiZla3w7EjcuGg=="
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