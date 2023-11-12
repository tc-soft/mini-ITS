using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using mini_ITS.Core.Database;
using mini_ITS.Core.Dto;
using mini_ITS.Core.Models;

namespace mini_ITS.Core.Tests
{
    public class EnrollmentsTestsData
    {
        private static IMapper _mapper;

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
        public static IEnumerable<SqlPagedQuery<EnrollmentsDto>> SqlPagedQueryCasesDto
        {
            get
            {
                var config = new MapperConfiguration(cfg => cfg.CreateMap<SqlPagedQuery<Enrollments>, SqlPagedQuery<EnrollmentsDto>>());
                _mapper = config.CreateMapper();

                return SqlPagedQueryCases.Select(item => _mapper.Map<SqlPagedQuery<EnrollmentsDto>>(item));
            }
        }
        public static IEnumerable<Enrollments> EnrollmentsCases
        {
            get
            {
                yield return new Enrollments
                {
                    Id = new Guid("A7615813-A47F-4E1A-9318-0EA1904BE6E0"),
                    Nr = 1,
                    Year = 2023,
                    DateAddEnrollment = new DateTime(2023, 8, 1, 7, 0, 0),
                    DateModEnrollment = new DateTime(2023, 8, 1, 7, 0, 0),
                    DateEndEnrollment = null,
                    DateReeEnrollment = null,
                    DateEndDeclareByUser = new DateTime(2023, 8, 8, 7, 0, 0),
                    DateEndDeclareByDepartment = null,
                    DateEndDeclareByDepartmentUser = new Guid(),
                    DateEndDeclareByDepartmentUserFullName = null,
                    Department = "Research",
                    Description = "Test enrollment 1",
                    Group = "Maverick Testers",
                    Priority = PriorityValues.High,
                    SMSToUserInfo = false,
                    SMSToAllInfo = false,
                    MailToUserInfo = false,
                    MailToAllInfo = false,
                    ReadyForClose = false,
                    State = "New",
                    UserAddEnrollment = new Guid("F474DC73-592A-41F4-9C01-0B28DC62B501"),
                    UserAddEnrollmentFullName = "Martin Quovomi",
                    UserModEnrollment = new Guid("F474DC73-592A-41F4-9C01-0B28DC62B501"),
                    UserModEnrollmentFullName = "Martin Quovomi",
                    UserEndEnrollment = new Guid(),
                    UserEndEnrollmentFullName = null,
                    UserReeEnrollment = new Guid(),
                    UserReeEnrollmentFullName = null,
                    ActionRequest = 0,
                    ActionExecuted = 0,
                    ActionFinished = false
                };
                yield return new Enrollments
                {
                    Id = new Guid("A107D228-B31E-422E-8755-D4CEC85A33C8"),
                    Nr = 4,
                    Year = 2023,
                    DateAddEnrollment = new DateTime(2023, 8, 4, 7, 0, 0),
                    DateModEnrollment = new DateTime(2023, 8, 5, 7, 0, 0),
                    DateEndEnrollment = null,
                    DateReeEnrollment = null,
                    DateEndDeclareByUser = new DateTime(2023, 8, 11, 7, 0, 0),
                    DateEndDeclareByDepartment = new DateTime(2023, 8, 11, 7, 0, 0),
                    DateEndDeclareByDepartmentUser = new Guid("6F929C62-3A50-4F29-AF99-F4D188754FC0"),
                    DateEndDeclareByDepartmentUserFullName = "Eva Pedrov",
                    Department = "IT",
                    Description = "Test enrollment 4",
                    Group = "Project Orion",
                    Priority = PriorityValues.Normal,
                    SMSToUserInfo = false,
                    SMSToAllInfo = false,
                    MailToUserInfo = false,
                    MailToAllInfo = false,
                    ReadyForClose = false,
                    State = "Assigned",
                    UserAddEnrollment = new Guid("8532C003-D391-42AA-843A-21393B46C1A8"),
                    UserAddEnrollmentFullName = "Colin Atkins",
                    UserModEnrollment = new Guid("8532C003-D391-42AA-843A-21393B46C1A8"),
                    UserModEnrollmentFullName = "Colin Atkins",
                    UserEndEnrollment = new Guid(),
                    UserEndEnrollmentFullName = null,
                    UserReeEnrollment = new Guid(),
                    UserReeEnrollmentFullName = null,
                    ActionRequest = 0,
                    ActionExecuted = 0,
                    ActionFinished = false
                };
                yield return new Enrollments
                {
                    Id = new Guid("3748715F-CA2F-444F-B434-8CC417DE2E4C"),
                    Nr = 7,
                    Year = 2023,
                    DateAddEnrollment = new DateTime(2023, 8, 7, 7, 0, 0),
                    DateModEnrollment = new DateTime(2023, 8, 12, 7, 0, 0),
                    DateEndEnrollment = new DateTime(2023, 8, 12, 7, 0, 0),
                    DateReeEnrollment = null,
                    DateEndDeclareByUser = new DateTime(2023, 8, 14, 7, 0, 0),
                    DateEndDeclareByDepartment = new DateTime(2023, 8, 14, 7, 0, 0),
                    DateEndDeclareByDepartmentUser = new Guid("40041296-20CF-407B-870E-3EE6049A5ABF"),
                    DateEndDeclareByDepartmentUserFullName = "Laila Bach",
                    Department = "Managers",
                    Description = "Test enrollment 7",
                    Group = "Alfa Avengers",
                    Priority = PriorityValues.Normal,
                    SMSToUserInfo = false,
                    SMSToAllInfo = false,
                    MailToUserInfo = false,
                    MailToAllInfo = false,
                    ReadyForClose = false,
                    State = "Closed",
                    UserAddEnrollment = new Guid("A5F16ECE-5C02-495D-B115-C34FE14C72E2"),
                    UserAddEnrollmentFullName = "Angelica Moreni",
                    UserModEnrollment = new Guid("A5F16ECE-5C02-495D-B115-C34FE14C72E2"),
                    UserModEnrollmentFullName = "Angelica Moreni",
                    UserEndEnrollment = new Guid("FBA6F4F6-BBD6-4088-8DD5-96E6AEF36E9C"),
                    UserEndEnrollmentFullName = "Ida Beukema",
                    UserReeEnrollment = new Guid(),
                    UserReeEnrollmentFullName = null,
                    ActionRequest = 0,
                    ActionExecuted = 0,
                    ActionFinished = false
                };
                yield return new Enrollments
                {
                    Id = new Guid("584C7104-C8A5-4E11-AFDC-7C9AF93C167D"),
                    Nr = 10,
                    Year = 2023,
                    DateAddEnrollment = new DateTime(2023, 8, 10, 7, 0, 0),
                    DateModEnrollment = new DateTime(2023, 8, 15, 7, 0, 0),
                    DateEndEnrollment = new DateTime(2023, 8, 15, 7, 0, 0),
                    DateReeEnrollment = null,
                    DateEndDeclareByUser = new DateTime(2023, 8, 17, 7, 0, 0),
                    DateEndDeclareByDepartment = new DateTime(2023, 8, 17, 7, 0, 0),
                    DateEndDeclareByDepartmentUser = new Guid("6F929C62-3A50-4F29-AF99-F4D188754FC0"),
                    DateEndDeclareByDepartmentUserFullName = "Eva Pedrov",
                    Department = "IT",
                    Description = "Test enrollment 10",
                    Group = "Project Orion",
                    Priority = PriorityValues.Critical,
                    SMSToUserInfo = false,
                    SMSToAllInfo = false,
                    MailToUserInfo = false,
                    MailToAllInfo = false,
                    ReadyForClose = false,
                    State = "Closed",
                    UserAddEnrollment = new Guid("A5F16ECE-5C02-495D-B115-C34FE14C72E2"),
                    UserAddEnrollmentFullName = "Angelica Moreni",
                    UserModEnrollment = new Guid("A5F16ECE-5C02-495D-B115-C34FE14C72E2"),
                    UserModEnrollmentFullName = "Angelica Moreni",
                    UserEndEnrollment = new Guid("FBA6F4F6-BBD6-4088-8DD5-96E6AEF36E9C"),
                    UserEndEnrollmentFullName = "Ida Beukema",
                    UserReeEnrollment = new Guid(),
                    UserReeEnrollmentFullName = null,
                    ActionRequest = 1,
                    ActionExecuted = 1,
                    ActionFinished = true
                };
            }
        }
        public static IEnumerable<EnrollmentsDto> EnrollmentsCasesDto
        {
            get
            {
                var config = new MapperConfiguration(cfg => cfg.CreateMap<Enrollments, EnrollmentsDto>());
                _mapper = config.CreateMapper();

                return EnrollmentsCases.Select(item => _mapper.Map<EnrollmentsDto>(item));
            }
        }
        public static IEnumerable<Enrollments> CRUDCases
        {
            get
            {
                yield return new Enrollments
                {
                    Id = new Guid("4FE0A770-C22C-4F77-8B7F-ACEA96493358"),
                    Nr = 11,
                    Year = 2023,
                    DateAddEnrollment = new DateTime(2023, 9, 1, 7, 0, 0),
                    DateModEnrollment = new DateTime(2023, 9, 1, 7, 0, 0),
                    DateEndEnrollment = null,
                    DateReeEnrollment = null,
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
                    State = "New",
                    UserAddEnrollment = new Guid("42E1156F-1F81-41E6-A5FB-1CBAB26F7F47"),
                    UserAddEnrollmentFullName = "Lana Raisic",
                    UserModEnrollment = new Guid("42E1156F-1F81-41E6-A5FB-1CBAB26F7F47"),
                    UserModEnrollmentFullName = "Lana Raisic",
                    UserEndEnrollment = new Guid(),
                    UserEndEnrollmentFullName = null,
                    UserReeEnrollment = new Guid(),
                    UserReeEnrollmentFullName = null,
                    ActionRequest = 0,
                    ActionExecuted = 0,
                    ActionFinished = false
                };
                yield return new Enrollments
                {
                    Id = new Guid("37455F97-7F14-4369-A6C2-6C9A74F9CC9E"),
                    Nr = 12,
                    Year = 2023,
                    DateAddEnrollment = new DateTime(2023, 9, 2, 7, 0, 0),
                    DateModEnrollment = new DateTime(2023, 9, 2, 7, 0, 0),
                    DateEndEnrollment = null,
                    DateReeEnrollment = null,
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
                    State = "New",
                    UserAddEnrollment = new Guid("FFE8CAB9-C2A9-4173-B00B-509A4BA737C8"),
                    UserAddEnrollmentFullName = "Kathie Marsh",
                    UserModEnrollment = new Guid("FFE8CAB9-C2A9-4173-B00B-509A4BA737C8"),
                    UserModEnrollmentFullName = "Kathie Marsh",
                    UserEndEnrollment = new Guid(),
                    UserEndEnrollmentFullName = null,
                    UserReeEnrollment = new Guid(),
                    UserReeEnrollmentFullName = null,
                    ActionRequest = 0,
                    ActionExecuted = 0,
                    ActionFinished = false
                };
                yield return new Enrollments
                {
                    Id = new Guid("B4A7F52E-CE31-4489-9F89-019F40EF424E"),
                    Nr = 13,
                    Year = 2023,
                    DateAddEnrollment = new DateTime(2023, 9, 3, 7, 0, 0),
                    DateModEnrollment = new DateTime(2023, 9, 3, 7, 0, 0),
                    DateEndEnrollment = null,
                    DateReeEnrollment = null,
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
                    State = "New",
                    UserAddEnrollment = new Guid("61008777-3BCA-4470-817D-35CFBD465921"),
                    UserAddEnrollmentFullName = "Peter Cottle",
                    UserModEnrollment = new Guid("61008777-3BCA-4470-817D-35CFBD465921"),
                    UserModEnrollmentFullName = "Peter Cottle",
                    UserEndEnrollment = new Guid(),
                    UserEndEnrollmentFullName = null,
                    UserReeEnrollment = new Guid(),
                    UserReeEnrollmentFullName = null,
                    ActionRequest = 0,
                    ActionExecuted = 0,
                    ActionFinished = false
                };
                yield return new Enrollments
                {
                    Id = new Guid("EBE99D81-CA2D-428D-871F-15585589B4FF"),
                    Nr = 14,
                    Year = 2023,
                    DateAddEnrollment = new DateTime(2023, 9, 4, 7, 0, 0),
                    DateModEnrollment = new DateTime(2023, 9, 4, 7, 0, 0),
                    DateEndEnrollment = null,
                    DateReeEnrollment = null,
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
                    State = "New",
                    UserAddEnrollment = new Guid("84249845-9D8E-48FD-898C-16BE0433BE66"),
                    UserAddEnrollmentFullName = "Michalina Yaveos",
                    UserModEnrollment = new Guid("84249845-9D8E-48FD-898C-16BE0433BE66"),
                    UserModEnrollmentFullName = "Michalina Yaveos",
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
        public static IEnumerable<EnrollmentsDto> CRUDCasesDto
        {
            get
            {
                var config = new MapperConfiguration(cfg => cfg.CreateMap<Enrollments, EnrollmentsDto>());
                _mapper = config.CreateMapper();

                return CRUDCases.Select(item => _mapper.Map<EnrollmentsDto>(item));
            }
        }
    }
}