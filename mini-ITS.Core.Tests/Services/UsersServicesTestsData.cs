using System;
using System.Collections.Generic;
using mini_ITS.Core.Database;
using mini_ITS.Core.Dto;
using mini_ITS.Core.Models;

namespace mini_ITS.Core.Tests.Services
{
    public class UsersServicesTestsData
    {
        public static IEnumerable<string> TestDepartment
        {
            get
            {
                yield return null;
                yield return "Sales";
                yield return "Research";
            }
        }
        public static IEnumerable<string> TestRole
        {
            get
            {
                yield return null;
                yield return "User";
                yield return "Manager";
            }
        }
        public static IEnumerable<SqlPagedQuery<Users>> SqlPagedQueryCases
        {
            get
            {
                yield return new SqlPagedQuery<Users>
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
                yield return new SqlPagedQuery<Users>
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
                yield return new SqlPagedQuery<Users>
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
                yield return new SqlPagedQuery<Users>
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
        public static IEnumerable<Users> UsersCases
        {
            get
            {
                yield return new Users
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
                yield return new Users
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
                yield return new Users
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
                yield return new Users
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
                    Login = "gerbaemm",
                    FirstName = "Emma",
                    LastName = "Gerbani",
                    Department = "IT",
                    Email = "emma.gerbani@example.com",
                    Phone = "501XXX501",
                    Role = "Manager",
                };
                yield return new UsersDto
                {
                    Login = "trembays",
                    FirstName = "Ayse",
                    LastName = "Tremblay",
                    Department = "Research",
                    Email = "ayse.tremblay@example.com",
                    Phone = "502XXX502",
                    Role = "User",
                };
                yield return new UsersDto
                {
                    Login = "gagnowil",
                    FirstName = "William",
                    LastName = "Gagnon",
                    Department = "Development",
                    Email = "william.gagnon@example.com",
                    Phone = "503XXX503",
                    Role = "Manager",
                };
                yield return new UsersDto
                {
                    Login = "mortojac",
                    FirstName = "Jack",
                    LastName = "Morton",
                    Department = "Sales",
                    Email = "jack.morton@example.com",
                    Phone = "504XXX504",
                    Role = "Manager",
                };
            }
        }
    }
}