using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using mini_ITS.Core.Dto;
using mini_ITS.Core.Models;

namespace mini_ITS.Core.Tests
{
    public class EnrollmentsDescriptionTestsData
    {
        private static IMapper _mapper;

        public static IEnumerable<EnrollmentsDescription> EnrollmentsDescriptionCases
        {
            get
            {
                yield return new EnrollmentsDescription
                {
                    Id = new Guid("9E303D5E-5A08-41B1-8BE9-0008379A7CC4"),
                    EnrollmentId = new Guid("6AC164EC-C578-43EB-9B4A-0478C316DD03"),
                    DateAddDescription = new DateTime(2023, 9, 1, 0, 0, 0),
                    DateModDescription = new DateTime(2023, 9, 1, 0, 0, 0),
                    UserAddDescription = new Guid("6F929C62-3A50-4F29-AF99-F4D188754FC0"),
                    UserAddDescriptionFullName = "Eva Pedrov",
                    UserModDescription = new Guid("6F929C62-3A50-4F29-AF99-F4D188754FC0"),
                    UserModDescriptionFullName = "Eva Pedrov",
                    Description = "Zgłoszenie przyjęto do realizacji.",
                    ActionExecuted = 0
                };
                yield return new EnrollmentsDescription
                {
                    Id = new Guid("9A32F7DA-35F0-4D29-9DB5-006A149282BB"),
                    EnrollmentId = new Guid("6AC164EC-C578-43EB-9B4A-0478C316DD03"),
                    DateAddDescription = new DateTime(2023, 9, 3, 0, 0, 0),
                    DateModDescription = new DateTime(2023, 9, 3, 0, 0, 0),
                    UserAddDescription = new Guid("253C1456-9966-4E0F-BBFD-0BB202DE221E"),
                    UserAddDescriptionFullName = "Victoria Sandoval",
                    UserModDescription = new Guid("253C1456-9966-4E0F-BBFD-0BB202DE221E"),
                    UserModDescriptionFullName = "Victoria Sandoval",
                    Description = "Zgłoszenie wykonane.",
                    ActionExecuted = 0
                };
                yield return new EnrollmentsDescription
                {
                    Id = new Guid("AD59ED9D-EAB5-4C17-B606-03302BFEEF9F"),
                    EnrollmentId = new Guid("9F38B77C-3444-4E69-BF2E-C1A71E4B5B6E"),
                    DateAddDescription = new DateTime(2023, 9, 7, 0, 0, 0),
                    DateModDescription = new DateTime(2023, 9, 7, 0, 0, 0),
                    UserAddDescription = new Guid("DA38CA53-D59F-49FB-B5E8-4118A13AABD9"),
                    UserAddDescriptionFullName = "Louis Gilliam",
                    UserModDescription = new Guid("DA38CA53-D59F-49FB-B5E8-4118A13AABD9"),
                    UserModDescriptionFullName = "Louis Gilliam",
                    Description = "Zgłoszenie przyjęto do realizacji.",
                    ActionExecuted = 0
                };
                yield return new EnrollmentsDescription
                {
                    Id = new Guid("45BA2201-7795-402D-AD0D-03C5118BF367"),
                    EnrollmentId = new Guid("9F38B77C-3444-4E69-BF2E-C1A71E4B5B6E"),
                    DateAddDescription = new DateTime(2023, 9, 10, 0, 0, 0),
                    DateModDescription = new DateTime(2023, 9, 10, 0, 0, 0),
                    UserAddDescription = new Guid("BCF44EFA-FB2B-4B13-B507-261C90CCFD17"),
                    UserAddDescriptionFullName = "Martin Visser",
                    UserModDescription = new Guid("BCF44EFA-FB2B-4B13-B507-261C90CCFD17"),
                    UserModDescriptionFullName = "Martin Visser",
                    Description = "Zgłoszenie wykonane.",
                    ActionExecuted = 1
                };
            }
        }
        public static IEnumerable<EnrollmentsDescriptionDto> EnrollmentsDescriptionCasesDto
        {
            get
            {
                var config = new MapperConfiguration(cfg => cfg.CreateMap<EnrollmentsDescription, EnrollmentsDescriptionDto>());
                _mapper = config.CreateMapper();

                return EnrollmentsDescriptionCases.Select(item => _mapper.Map<EnrollmentsDescriptionDto>(item));
            }
        }
        public static IEnumerable<EnrollmentsDescription> CRUDCases
        {
            get
            {
                yield return new EnrollmentsDescription
                {
                    Id = new Guid("BB7E3669-7636-4D87-9846-995A1E060A2A"),
                    EnrollmentId = new Guid("339E285F-C8FA-4B0B-93CB-5CAF96CDD0C8"),
                    DateAddDescription = new DateTime(2023, 9, 10, 0, 0, 0),
                    DateModDescription = new DateTime(2023, 9, 10, 0, 0, 0),
                    UserAddDescription = new Guid("6F929C62-3A50-4F29-AF99-F4D188754FC0"),
                    UserAddDescriptionFullName = "Eva Pedrov",
                    UserModDescription = new Guid("6F929C62-3A50-4F29-AF99-F4D188754FC0"),
                    UserModDescriptionFullName = "Eva Pedrov",
                    Description = "Test 1",
                    ActionExecuted = 0
                };
                yield return new EnrollmentsDescription
                {
                    Id = new Guid("79D82A8C-2040-4A02-AB28-1671C9085D7D"),
                    EnrollmentId = new Guid("339E285F-C8FA-4B0B-93CB-5CAF96CDD0C8"),
                    DateAddDescription = new DateTime(2023, 9, 10, 0, 0, 0),
                    DateModDescription = new DateTime(2023, 9, 10, 0, 0, 0),
                    UserAddDescription = new Guid("253C1456-9966-4E0F-BBFD-0BB202DE221E"),
                    UserAddDescriptionFullName = "Victoria Sandoval",
                    UserModDescription = new Guid("253C1456-9966-4E0F-BBFD-0BB202DE221E"),
                    UserModDescriptionFullName = "Victoria Sandoval",
                    Description = "Test 2",
                    ActionExecuted = 0
                };
                yield return new EnrollmentsDescription
                {
                    Id = new Guid("C7BE2EDE-BA5A-4C55-B068-960490C12BA2"),
                    EnrollmentId = new Guid("339E285F-C8FA-4B0B-93CB-5CAF96CDD0C8"),
                    DateAddDescription = new DateTime(2023, 9, 10, 0, 0, 0),
                    DateModDescription = new DateTime(2023, 9, 10, 0, 0, 0),
                    UserAddDescription = new Guid("DA38CA53-D59F-49FB-B5E8-4118A13AABD9"),
                    UserAddDescriptionFullName = "Louis Gilliam",
                    UserModDescription = new Guid("DA38CA53-D59F-49FB-B5E8-4118A13AABD9"),
                    UserModDescriptionFullName = "Louis Gilliam",
                    Description = "Test 3",
                    ActionExecuted = 0
                };
                yield return new EnrollmentsDescription
                {
                    Id = new Guid("DC8520BF-B52D-4294-9672-31635655013B"),
                    EnrollmentId = new Guid("339E285F-C8FA-4B0B-93CB-5CAF96CDD0C8"),
                    DateAddDescription = new DateTime(2023, 9, 10, 0, 0, 0),
                    DateModDescription = new DateTime(2023, 9, 10, 0, 0, 0),
                    UserAddDescription = new Guid("BCF44EFA-FB2B-4B13-B507-261C90CCFD17"),
                    UserAddDescriptionFullName = "Martin Visser",
                    UserModDescription = new Guid("BCF44EFA-FB2B-4B13-B507-261C90CCFD17"),
                    UserModDescriptionFullName = "Martin Visser",
                    Description = "Test 4",
                    ActionExecuted = 1
                };
            }
        }
        public static IEnumerable<EnrollmentsDescriptionDto> CRUDCasesDto
        {
            get
            {
                var config = new MapperConfiguration(cfg => cfg.CreateMap<EnrollmentsDescription, EnrollmentsDescriptionDto>());
                _mapper = config.CreateMapper();

                return CRUDCases.Select(item => _mapper.Map<EnrollmentsDescriptionDto>(item));
            }
        }
    }
}