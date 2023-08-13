﻿using System;
using System.Collections.Generic;
using mini_ITS.Core.Database;
using mini_ITS.Core.Dto;
using mini_ITS.Core.Models;

namespace mini_ITS.Core.Tests.Services
{
    public class GroupsServicesTestsData
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
        public static IEnumerable<GroupsDto> GroupsCases
        {
            get
            {
                yield return new GroupsDto
                {
                    Id = new Guid("B936FBEB-A52D-40F4-92FC-1258214CF6E8"),
                    DateAddGroup = new DateTime(2023, 8, 1, 0, 0, 0),
                    DateModGroup = new DateTime(2023, 8, 1, 0, 0, 0),
                    UserAddGroup = new Guid("FCC06ACA-BE27-46FA-9142-BB1BA1322EB3"),
                    UserAddGroupFullName = "Admin Administrator",
                    UserModGroup = new Guid("FCC06ACA-BE27-46FA-9142-BB1BA1322EB3"),
                    UserModGroupFullName = "Admin Administrator",
                    GroupName = "Alfa Avengers"
                };
                yield return new GroupsDto
                {
                    Id = new Guid("9E8569C5-B198-43C7-93C4-260D185028BF"),
                    DateAddGroup = new DateTime(2023, 8, 1, 0, 0, 0),
                    DateModGroup = new DateTime(2023, 8, 1, 0, 0, 0),
                    UserAddGroup = new Guid("FBE24C52-15AE-4C92-9C24-2C735D81EAE7"),
                    UserAddGroupFullName = "Demi Balode",
                    UserModGroup = new Guid("FBE24C52-15AE-4C92-9C24-2C735D81EAE7"),
                    UserModGroupFullName = "Demi Balode",
                    GroupName = "Group Phoenix"
                };
                yield return new GroupsDto
                {
                    Id = new Guid("5987D667-6072-41DC-A822-460ECD4C9DA6"),
                    DateAddGroup = new DateTime(2023, 8, 1, 0, 0, 0),
                    DateModGroup = new DateTime(2023, 8, 1, 0, 0, 0),
                    UserAddGroup = new Guid("FCC06ACA-BE27-46FA-9142-BB1BA1322EB3"),
                    UserAddGroupFullName = "Admin Administrator",
                    UserModGroup = new Guid("FCC06ACA-BE27-46FA-9142-BB1BA1322EB3"),
                    UserModGroupFullName = "Admin Administrator",
                    GroupName = "Precision Probers"
                };
                yield return new GroupsDto
                {
                    Id = new Guid("F3773676-24E4-413D-A600-60CB032E67DD"),
                    DateAddGroup = new DateTime(2023, 8, 1, 0, 0, 0),
                    DateModGroup = new DateTime(2023, 8, 1, 0, 0, 0),
                    UserAddGroup = new Guid("FBE24C52-15AE-4C92-9C24-2C735D81EAE7"),
                    UserAddGroupFullName = "Demi Balode",
                    UserModGroup = new Guid("FBE24C52-15AE-4C92-9C24-2C735D81EAE7"),
                    UserModGroupFullName = "Demi Balode",
                    GroupName = "Team Mercury"
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