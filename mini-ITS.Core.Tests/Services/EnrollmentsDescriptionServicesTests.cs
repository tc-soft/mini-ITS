using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.Extensions.Configuration;
using NUnit.Framework;
using mini_ITS.Core.Database;
using mini_ITS.Core.Dto;
using mini_ITS.Core.Models;
using mini_ITS.Core.Options;
using mini_ITS.Core.Repository;
using mini_ITS.Core.Services;

namespace mini_ITS.Core.Tests.Services
{
    [TestFixture]
    public class EnrollmentsDescriptionServicesTests
    {
        private IMapper _mapper;
        private IUsersRepository _usersRepository;
        private IEnrollmentsDescriptionRepository _enrollmentsDescriptionRepository;
        private IEnrollmentsDescriptionServices _enrollmentsDescriptionServices;

        [SetUp]
        public void Init()
        {
            var _path = Path.Combine(Directory.GetParent(Directory.GetCurrentDirectory()).Parent.Parent.Parent.FullName, "mini-ITS.Web");

            var configuration = new ConfigurationBuilder()
               .SetBasePath(_path)
               .AddJsonFile("appsettings.json", false)
               .Build();

            var _databaseOptions = Microsoft.Extensions.Options.Options.Create(configuration.GetSection("DatabaseOptions").Get<DatabaseOptions>());
            var _sqlConnectionString = new SqlConnectionString(_databaseOptions);
            _usersRepository = new UsersRepository(_sqlConnectionString);
            _mapper = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<EnrollmentsDescriptionDto, EnrollmentsDescription>();
                cfg.CreateMap<EnrollmentsDescription, EnrollmentsDescriptionDto>();
            }).CreateMapper();
            _enrollmentsDescriptionRepository = new EnrollmentsDescriptionRepository(_sqlConnectionString);
            _enrollmentsDescriptionServices = new EnrollmentsDescriptionServices(_enrollmentsDescriptionRepository, _usersRepository, _mapper);
        }
        [Test]
        public async Task GetAsync_CheckAll()
        {
            TestContext.Out.WriteLine("Get enrollmentsDescriptionDto by GetAsync() and check valid...\n");
            var enrollmentsDescriptionDto = await _enrollmentsDescriptionServices.GetAsync();

            Assert.That(enrollmentsDescriptionDto.Count() >= 10, "ERROR - number of items is less than 10");
            Assert.That(enrollmentsDescriptionDto, Is.InstanceOf<IEnumerable<EnrollmentsDescriptionDto>>(), "ERROR - return type");
            Assert.That(enrollmentsDescriptionDto, Is.All.InstanceOf<EnrollmentsDescriptionDto>(), "ERROR - all instance is not of <EnrollmentsDescriptionDto>()");
            Assert.That(enrollmentsDescriptionDto, Is.Ordered.Ascending.By("DateAddDescription"), "ERROR - sort");
            Assert.That(enrollmentsDescriptionDto, Is.Unique);

            foreach (var item in enrollmentsDescriptionDto)
            {
                TestContext.Out.WriteLine($"Description: {item.Description}");
            }
            TestContext.Out.WriteLine($"\nNumber of records: {enrollmentsDescriptionDto.Count()}");
        }
        [TestCaseSource(typeof(EnrollmentsDescriptionServicesTestsData), nameof(EnrollmentsDescriptionServicesTestsData.EnrollmentsDescriptionCases))]
        public async Task GetAsync_CheckId(EnrollmentsDescriptionDto enrollmentsDescriptionDto)
        {
            TestContext.Out.WriteLine("Get enrollmentsDescriptionDto by GetAsync(id) and check valid...\n");
            var enrollmentDescriptionDto = await _enrollmentsDescriptionServices.GetAsync(enrollmentsDescriptionDto.Id);

            Assert.That(enrollmentDescriptionDto, Is.TypeOf<EnrollmentsDescriptionDto>(), "ERROR - return type");

            Assert.That(enrollmentDescriptionDto.Id, Is.EqualTo(enrollmentsDescriptionDto.Id), $"ERROR - {nameof(enrollmentsDescriptionDto.Id)} is not equal");
            Assert.That(enrollmentDescriptionDto.EnrollmentId, Is.EqualTo(enrollmentsDescriptionDto.EnrollmentId), $"ERROR - {nameof(enrollmentsDescriptionDto.EnrollmentId)} is not equal");
            Assert.That(enrollmentDescriptionDto.DateAddDescription, Is.EqualTo(enrollmentsDescriptionDto.DateAddDescription), $"ERROR - {nameof(enrollmentsDescriptionDto.DateAddDescription)} is not equal");
            Assert.That(enrollmentDescriptionDto.DateModDescription, Is.EqualTo(enrollmentsDescriptionDto.DateModDescription), $"ERROR - {nameof(enrollmentsDescriptionDto.DateModDescription)} is not equal");
            Assert.That(enrollmentDescriptionDto.UserAddDescription, Is.EqualTo(enrollmentsDescriptionDto.UserAddDescription), $"ERROR - {nameof(enrollmentsDescriptionDto.UserAddDescription)} is not equal");
            Assert.That(enrollmentDescriptionDto.UserAddDescriptionFullName, Is.EqualTo(enrollmentsDescriptionDto.UserAddDescriptionFullName), $"ERROR - {nameof(enrollmentsDescriptionDto.UserAddDescriptionFullName)} is not equal");
            Assert.That(enrollmentDescriptionDto.UserModDescription, Is.EqualTo(enrollmentsDescriptionDto.UserModDescription), $"ERROR - {nameof(enrollmentsDescriptionDto.UserModDescription)} is not equal");
            Assert.That(enrollmentDescriptionDto.UserModDescriptionFullName, Is.EqualTo(enrollmentsDescriptionDto.UserModDescriptionFullName), $"ERROR - {nameof(enrollmentsDescriptionDto.UserModDescriptionFullName)} is not equal");
            Assert.That(enrollmentDescriptionDto.Description, Is.EqualTo(enrollmentsDescriptionDto.Description), $"ERROR - {nameof(enrollmentsDescriptionDto.Description)} is not equal");
            Assert.That(enrollmentDescriptionDto.ActionExecuted, Is.EqualTo(enrollmentsDescriptionDto.ActionExecuted), $"ERROR - {nameof(enrollmentsDescriptionDto.ActionExecuted)} is not equal");

            TestContext.Out.WriteLine($"Id                         : {enrollmentDescriptionDto.Id}");
            TestContext.Out.WriteLine($"EnrollmentId               : {enrollmentDescriptionDto.EnrollmentId}");
            TestContext.Out.WriteLine($"DateAddDescription         : {enrollmentDescriptionDto.DateAddDescription}");
            TestContext.Out.WriteLine($"DateModDescription         : {enrollmentDescriptionDto.DateModDescription}");
            TestContext.Out.WriteLine($"UserAddDescription         : {enrollmentDescriptionDto.UserAddDescription}");
            TestContext.Out.WriteLine($"UserAddDescriptionFullName : {enrollmentDescriptionDto.UserAddDescriptionFullName}");
            TestContext.Out.WriteLine($"UserModDescription         : {enrollmentDescriptionDto.UserModDescription}");
            TestContext.Out.WriteLine($"UserModDescriptionFullName : {enrollmentDescriptionDto.UserModDescriptionFullName}");
            TestContext.Out.WriteLine($"Description                : {enrollmentDescriptionDto.Description}");
            TestContext.Out.WriteLine($"ActionExecuted             : {enrollmentDescriptionDto.ActionExecuted}");
        }
        [TestCaseSource(typeof(EnrollmentsDescriptionServicesTestsData), nameof(EnrollmentsDescriptionServicesTestsData.EnrollmentsDescriptionCases))]
        public async Task GetEnrollmentDescriptionsAsync_CheckId(EnrollmentsDescriptionDto enrollmentsDescriptionDto)
        {
            TestContext.Out.WriteLine("Get enrollmentsDescriptionDto by GetEnrollmentDescriptionsAsync(id) and check valid...\n");
            var enrollmentDescriptionDto = await _enrollmentsDescriptionServices.GetEnrollmentDescriptionsAsync(enrollmentsDescriptionDto.EnrollmentId);

            Assert.That(enrollmentDescriptionDto.Count() >= 2, "ERROR - number of items is less than 2");
            Assert.That(enrollmentDescriptionDto, Is.InstanceOf<IEnumerable<EnrollmentsDescriptionDto>>(), "ERROR - return type");
            Assert.That(enrollmentDescriptionDto, Is.All.InstanceOf<EnrollmentsDescriptionDto>(), "ERROR - all instance is not of <EnrollmentsDescriptionDto>()");
            Assert.That(enrollmentDescriptionDto, Is.Ordered.Ascending.By("DateAddDescription"), "ERROR - sort");
            Assert.That(enrollmentDescriptionDto, Is.Unique);

            foreach (var item in enrollmentDescriptionDto)
            {
                TestContext.Out.WriteLine($"Description: {item.Description}");
            }
            TestContext.Out.WriteLine($"\nNumber of records: {enrollmentDescriptionDto.Count()}");
        }
        [TestCaseSource(typeof(EnrollmentsDescriptionServicesTestsData), nameof(EnrollmentsDescriptionServicesTestsData.CRUDCases))]
        public async Task CreateAsync(EnrollmentsDescriptionDto enrollmentsDescriptionDto)
        {
            TestContext.Out.WriteLine("Create enrollmentDescription by CreateAsync(enrollmentsDescriptionDto, username) and check valid...\n");
            var user = await _usersRepository.GetAsync(enrollmentsDescriptionDto.UserAddDescription);
            var id = await _enrollmentsDescriptionServices.CreateAsync(enrollmentsDescriptionDto, user.Login);
            var enrollmentDescriptionDto = await _enrollmentsDescriptionServices.GetAsync(id);

            Assert.That(enrollmentDescriptionDto, Is.TypeOf<EnrollmentsDescriptionDto>(), "ERROR - return type");

            Assert.That(enrollmentDescriptionDto.Id, Is.TypeOf<Guid>(), $"ERROR - {nameof(enrollmentsDescriptionDto.Id)} is not Guid type");
            Assert.That(enrollmentDescriptionDto.EnrollmentId, Is.EqualTo(enrollmentsDescriptionDto.EnrollmentId), $"ERROR - {nameof(enrollmentsDescriptionDto.EnrollmentId)} is not equal");
            Assert.That(enrollmentDescriptionDto.UserAddDescription, Is.EqualTo(enrollmentsDescriptionDto.UserAddDescription), $"ERROR - {nameof(enrollmentsDescriptionDto.UserAddDescription)} is not equal");
            Assert.That(enrollmentDescriptionDto.UserAddDescriptionFullName, Is.EqualTo(enrollmentsDescriptionDto.UserAddDescriptionFullName), $"ERROR - {nameof(enrollmentsDescriptionDto.UserAddDescriptionFullName)} is not equal");
            Assert.That(enrollmentDescriptionDto.UserModDescription, Is.EqualTo(enrollmentsDescriptionDto.UserModDescription), $"ERROR - {nameof(enrollmentsDescriptionDto.UserModDescription)} is not equal");
            Assert.That(enrollmentDescriptionDto.UserModDescriptionFullName, Is.EqualTo(enrollmentsDescriptionDto.UserModDescriptionFullName), $"ERROR - {nameof(enrollmentsDescriptionDto.UserModDescriptionFullName)} is not equal");
            Assert.That(enrollmentDescriptionDto.Description, Is.EqualTo(enrollmentsDescriptionDto.Description), $"ERROR - {nameof(enrollmentsDescriptionDto.Description)} is not equal");
            Assert.That(enrollmentDescriptionDto.ActionExecuted, Is.EqualTo(enrollmentsDescriptionDto.ActionExecuted), $"ERROR - {nameof(enrollmentsDescriptionDto.ActionExecuted)} is not equal");

            TestContext.Out.WriteLine($"Id                         : {enrollmentDescriptionDto.Id}");
            TestContext.Out.WriteLine($"EnrollmentId               : {enrollmentDescriptionDto.EnrollmentId}");
            TestContext.Out.WriteLine($"DateAddDescription         : {enrollmentDescriptionDto.DateAddDescription}");
            TestContext.Out.WriteLine($"DateModDescription         : {enrollmentDescriptionDto.DateModDescription}");
            TestContext.Out.WriteLine($"UserAddDescription         : {enrollmentDescriptionDto.UserAddDescription}");
            TestContext.Out.WriteLine($"UserAddDescriptionFullName : {enrollmentDescriptionDto.UserAddDescriptionFullName}");
            TestContext.Out.WriteLine($"UserModDescription         : {enrollmentDescriptionDto.UserModDescription}");
            TestContext.Out.WriteLine($"UserModDescriptionFullName : {enrollmentDescriptionDto.UserModDescriptionFullName}");
            TestContext.Out.WriteLine($"Description                : {enrollmentDescriptionDto.Description}");
            TestContext.Out.WriteLine($"ActionExecuted             : {enrollmentDescriptionDto.ActionExecuted}");

            TestContext.Out.WriteLine("\nDelete enrollmentDescription by DeleteAsync(id) and check valid...");
            await _enrollmentsDescriptionServices.DeleteAsync(enrollmentDescriptionDto.Id);
            enrollmentDescriptionDto = await _enrollmentsDescriptionServices.GetAsync(enrollmentsDescriptionDto.Id);
            Assert.That(enrollmentDescriptionDto, Is.Null, "ERROR - delete enrollmentDescription");
        }
        [TestCaseSource(typeof(EnrollmentsDescriptionServicesTestsData), nameof(EnrollmentsDescriptionServicesTestsData.CRUDCases))]
        public async Task UpdateAsync(EnrollmentsDescriptionDto enrollmentsDescriptionDto)
        {
            TestContext.Out.WriteLine("Create enrollmentDescription by CreateAsync(enrollmentsDescriptionDto, username) and check valid...\n");
            var user = await _usersRepository.GetAsync(enrollmentsDescriptionDto.UserAddDescription);
            var id = await _enrollmentsDescriptionServices.CreateAsync(enrollmentsDescriptionDto, user.Login);
            var enrollmentDescriptionDto = await _enrollmentsDescriptionServices.GetAsync(id);

            Assert.That(enrollmentDescriptionDto, Is.TypeOf<EnrollmentsDescriptionDto>(), "ERROR - return type");

            Assert.That(enrollmentDescriptionDto.Id, Is.TypeOf<Guid>(), $"ERROR - {nameof(enrollmentsDescriptionDto.Id)} is not Guid type");
            Assert.That(enrollmentDescriptionDto.EnrollmentId, Is.EqualTo(enrollmentsDescriptionDto.EnrollmentId), $"ERROR - {nameof(enrollmentsDescriptionDto.EnrollmentId)} is not equal");
            Assert.That(enrollmentDescriptionDto.UserAddDescription, Is.EqualTo(enrollmentsDescriptionDto.UserAddDescription), $"ERROR - {nameof(enrollmentsDescriptionDto.UserAddDescription)} is not equal");
            Assert.That(enrollmentDescriptionDto.UserAddDescriptionFullName, Is.EqualTo(enrollmentsDescriptionDto.UserAddDescriptionFullName), $"ERROR - {nameof(enrollmentsDescriptionDto.UserAddDescriptionFullName)} is not equal");
            Assert.That(enrollmentDescriptionDto.UserModDescription, Is.EqualTo(enrollmentsDescriptionDto.UserModDescription), $"ERROR - {nameof(enrollmentsDescriptionDto.UserModDescription)} is not equal");
            Assert.That(enrollmentDescriptionDto.UserModDescriptionFullName, Is.EqualTo(enrollmentsDescriptionDto.UserModDescriptionFullName), $"ERROR - {nameof(enrollmentsDescriptionDto.UserModDescriptionFullName)} is not equal");
            Assert.That(enrollmentDescriptionDto.Description, Is.EqualTo(enrollmentsDescriptionDto.Description), $"ERROR - {nameof(enrollmentsDescriptionDto.Description)} is not equal");
            Assert.That(enrollmentDescriptionDto.ActionExecuted, Is.EqualTo(enrollmentsDescriptionDto.ActionExecuted), $"ERROR - {nameof(enrollmentsDescriptionDto.ActionExecuted)} is not equal");

            TestContext.Out.WriteLine($"Id                         : {enrollmentDescriptionDto.Id}");
            TestContext.Out.WriteLine($"EnrollmentId               : {enrollmentDescriptionDto.EnrollmentId}");
            TestContext.Out.WriteLine($"DateAddDescription         : {enrollmentDescriptionDto.DateAddDescription}");
            TestContext.Out.WriteLine($"DateModDescription         : {enrollmentDescriptionDto.DateModDescription}");
            TestContext.Out.WriteLine($"UserAddDescription         : {enrollmentDescriptionDto.UserAddDescription}");
            TestContext.Out.WriteLine($"UserAddDescriptionFullName : {enrollmentDescriptionDto.UserAddDescriptionFullName}");
            TestContext.Out.WriteLine($"UserModDescription         : {enrollmentDescriptionDto.UserModDescription}");
            TestContext.Out.WriteLine($"UserModDescriptionFullName : {enrollmentDescriptionDto.UserModDescriptionFullName}");
            TestContext.Out.WriteLine($"Description                : {enrollmentDescriptionDto.Description}");
            TestContext.Out.WriteLine($"ActionExecuted             : {enrollmentDescriptionDto.ActionExecuted}");

            TestContext.Out.WriteLine("\nUpdate enrollmentDescription by UpdateAsync(enrollmentsDescriptionDto, username) and check valid...\n");
            var caesarHelper = new CaesarHelper();
            enrollmentDescriptionDto.Description = caesarHelper.Encrypt(enrollmentDescriptionDto.Description);
            await _enrollmentsDescriptionServices.UpdateAsync(enrollmentDescriptionDto, user.Login);
            enrollmentDescriptionDto = await _enrollmentsDescriptionServices.GetAsync(id);

            Assert.That(enrollmentDescriptionDto, Is.TypeOf<EnrollmentsDescriptionDto>(), "ERROR - return type");

            Assert.IsNotNull(enrollmentDescriptionDto.Id, $"ERROR - {nameof(enrollmentDescriptionDto.Id)} is null");
            Assert.IsNotNull(enrollmentDescriptionDto.EnrollmentId, $"ERROR - {nameof(enrollmentDescriptionDto.EnrollmentId)} is null");
            Assert.IsNotNull(enrollmentDescriptionDto.DateAddDescription, $"ERROR - {nameof(enrollmentDescriptionDto.DateAddDescription)} is null");
            Assert.IsNotNull(enrollmentDescriptionDto.DateModDescription, $"ERROR - {nameof(enrollmentDescriptionDto.DateModDescription)} is null");
            Assert.IsNotNull(enrollmentDescriptionDto.UserAddDescription, $"ERROR - {nameof(enrollmentDescriptionDto.UserAddDescription)} is null");
            Assert.IsNotNull(enrollmentDescriptionDto.UserAddDescriptionFullName, $"ERROR - {nameof(enrollmentDescriptionDto.UserAddDescriptionFullName)} is null");
            Assert.IsNotNull(enrollmentDescriptionDto.UserModDescription, $"ERROR - {nameof(enrollmentDescriptionDto.UserModDescription)} is null");
            Assert.IsNotNull(enrollmentDescriptionDto.UserModDescriptionFullName, $"ERROR - {nameof(enrollmentDescriptionDto.UserModDescriptionFullName)} is null");
            Assert.IsNotNull(enrollmentDescriptionDto.Description, $"ERROR - {nameof(enrollmentDescriptionDto.Description)} is null");
            Assert.IsNotNull(enrollmentDescriptionDto.ActionExecuted, $"ERROR - {nameof(enrollmentDescriptionDto.ActionExecuted)} is null");

            TestContext.Out.WriteLine($"Id                         : {enrollmentDescriptionDto.Id}");
            TestContext.Out.WriteLine($"EnrollmentId               : {enrollmentDescriptionDto.EnrollmentId}");
            TestContext.Out.WriteLine($"DateAddDescription         : {enrollmentDescriptionDto.DateAddDescription}");
            TestContext.Out.WriteLine($"DateModDescription         : {enrollmentDescriptionDto.DateModDescription}");
            TestContext.Out.WriteLine($"UserAddDescription         : {enrollmentDescriptionDto.UserAddDescription}");
            TestContext.Out.WriteLine($"UserAddDescriptionFullName : {enrollmentDescriptionDto.UserAddDescriptionFullName}");
            TestContext.Out.WriteLine($"UserModDescription         : {enrollmentDescriptionDto.UserModDescription}");
            TestContext.Out.WriteLine($"UserModDescriptionFullName : {enrollmentDescriptionDto.UserModDescriptionFullName}");
            TestContext.Out.WriteLine($"Description                : {enrollmentDescriptionDto.Description}");
            TestContext.Out.WriteLine($"ActionExecuted             : {enrollmentDescriptionDto.ActionExecuted}");

            TestContext.Out.WriteLine("\nUpdate enrollmentDescription by UpdateAsync(enrollmentsDescriptionDto, username) and check valid...\n");
            enrollmentDescriptionDto.Description = caesarHelper.Decrypt(enrollmentDescriptionDto.Description);
            await _enrollmentsDescriptionServices.UpdateAsync(enrollmentDescriptionDto, user.Login);
            enrollmentDescriptionDto = await _enrollmentsDescriptionServices.GetAsync(id);

            Assert.That(enrollmentDescriptionDto, Is.TypeOf<EnrollmentsDescriptionDto>(), "ERROR - return type");

            Assert.That(enrollmentDescriptionDto.Id, Is.TypeOf<Guid>(), $"ERROR - {nameof(enrollmentsDescriptionDto.Id)} is not Guid type");
            Assert.That(enrollmentDescriptionDto.EnrollmentId, Is.EqualTo(enrollmentsDescriptionDto.EnrollmentId), $"ERROR - {nameof(enrollmentsDescriptionDto.EnrollmentId)} is not equal");
            Assert.That(enrollmentDescriptionDto.UserAddDescription, Is.EqualTo(enrollmentsDescriptionDto.UserAddDescription), $"ERROR - {nameof(enrollmentsDescriptionDto.UserAddDescription)} is not equal");
            Assert.That(enrollmentDescriptionDto.UserAddDescriptionFullName, Is.EqualTo(enrollmentsDescriptionDto.UserAddDescriptionFullName), $"ERROR - {nameof(enrollmentsDescriptionDto.UserAddDescriptionFullName)} is not equal");
            Assert.That(enrollmentDescriptionDto.UserModDescription, Is.EqualTo(enrollmentsDescriptionDto.UserModDescription), $"ERROR - {nameof(enrollmentsDescriptionDto.UserModDescription)} is not equal");
            Assert.That(enrollmentDescriptionDto.UserModDescriptionFullName, Is.EqualTo(enrollmentsDescriptionDto.UserModDescriptionFullName), $"ERROR - {nameof(enrollmentsDescriptionDto.UserModDescriptionFullName)} is not equal");
            Assert.That(enrollmentDescriptionDto.Description, Is.EqualTo(enrollmentsDescriptionDto.Description), $"ERROR - {nameof(enrollmentsDescriptionDto.Description)} is not equal");
            Assert.That(enrollmentDescriptionDto.ActionExecuted, Is.EqualTo(enrollmentsDescriptionDto.ActionExecuted), $"ERROR - {nameof(enrollmentsDescriptionDto.ActionExecuted)} is not equal");

            TestContext.Out.WriteLine($"Id                         : {enrollmentDescriptionDto.Id}");
            TestContext.Out.WriteLine($"EnrollmentId               : {enrollmentDescriptionDto.EnrollmentId}");
            TestContext.Out.WriteLine($"DateAddDescription         : {enrollmentDescriptionDto.DateAddDescription}");
            TestContext.Out.WriteLine($"DateModDescription         : {enrollmentDescriptionDto.DateModDescription}");
            TestContext.Out.WriteLine($"UserAddDescription         : {enrollmentDescriptionDto.UserAddDescription}");
            TestContext.Out.WriteLine($"UserAddDescriptionFullName : {enrollmentDescriptionDto.UserAddDescriptionFullName}");
            TestContext.Out.WriteLine($"UserModDescription         : {enrollmentDescriptionDto.UserModDescription}");
            TestContext.Out.WriteLine($"UserModDescriptionFullName : {enrollmentDescriptionDto.UserModDescriptionFullName}");
            TestContext.Out.WriteLine($"Description                : {enrollmentDescriptionDto.Description}");
            TestContext.Out.WriteLine($"ActionExecuted             : {enrollmentDescriptionDto.ActionExecuted}");

            TestContext.Out.WriteLine("\nDelete enrollmentDescription by DeleteAsync(id) and check valid...");
            await _enrollmentsDescriptionServices.DeleteAsync(enrollmentDescriptionDto.Id);
            enrollmentDescriptionDto = await _enrollmentsDescriptionServices.GetAsync(enrollmentsDescriptionDto.Id);
            Assert.That(enrollmentDescriptionDto, Is.Null, "ERROR - delete enrollmentDescription");

        }
        [TestCaseSource(typeof(EnrollmentsDescriptionServicesTestsData), nameof(EnrollmentsDescriptionServicesTestsData.CRUDCases))]
        public async Task DeleteAsync(EnrollmentsDescriptionDto enrollmentsDescriptionDto)
        {
            TestContext.Out.WriteLine("Create enrollmentDescription by CreateAsync(enrollmentsDescriptionDto, username) and check valid...\n");
            var user = await _usersRepository.GetAsync(enrollmentsDescriptionDto.UserAddDescription);
            var id = await _enrollmentsDescriptionServices.CreateAsync(enrollmentsDescriptionDto, user.Login);
            var enrollmentDescriptionDto = await _enrollmentsDescriptionServices.GetAsync(id);

            Assert.That(enrollmentDescriptionDto, Is.TypeOf<EnrollmentsDescriptionDto>(), "ERROR - return type");

            Assert.That(enrollmentDescriptionDto.Id, Is.TypeOf<Guid>(), $"ERROR - {nameof(enrollmentsDescriptionDto.Id)} is not Guid type");
            Assert.That(enrollmentDescriptionDto.EnrollmentId, Is.EqualTo(enrollmentsDescriptionDto.EnrollmentId), $"ERROR - {nameof(enrollmentsDescriptionDto.EnrollmentId)} is not equal");
            Assert.That(enrollmentDescriptionDto.UserAddDescription, Is.EqualTo(enrollmentsDescriptionDto.UserAddDescription), $"ERROR - {nameof(enrollmentsDescriptionDto.UserAddDescription)} is not equal");
            Assert.That(enrollmentDescriptionDto.UserAddDescriptionFullName, Is.EqualTo(enrollmentsDescriptionDto.UserAddDescriptionFullName), $"ERROR - {nameof(enrollmentsDescriptionDto.UserAddDescriptionFullName)} is not equal");
            Assert.That(enrollmentDescriptionDto.UserModDescription, Is.EqualTo(enrollmentsDescriptionDto.UserModDescription), $"ERROR - {nameof(enrollmentsDescriptionDto.UserModDescription)} is not equal");
            Assert.That(enrollmentDescriptionDto.UserModDescriptionFullName, Is.EqualTo(enrollmentsDescriptionDto.UserModDescriptionFullName), $"ERROR - {nameof(enrollmentsDescriptionDto.UserModDescriptionFullName)} is not equal");
            Assert.That(enrollmentDescriptionDto.Description, Is.EqualTo(enrollmentsDescriptionDto.Description), $"ERROR - {nameof(enrollmentsDescriptionDto.Description)} is not equal");
            Assert.That(enrollmentDescriptionDto.ActionExecuted, Is.EqualTo(enrollmentsDescriptionDto.ActionExecuted), $"ERROR - {nameof(enrollmentsDescriptionDto.ActionExecuted)} is not equal");

            TestContext.Out.WriteLine($"Id                         : {enrollmentDescriptionDto.Id}");
            TestContext.Out.WriteLine($"EnrollmentId               : {enrollmentDescriptionDto.EnrollmentId}");
            TestContext.Out.WriteLine($"DateAddDescription         : {enrollmentDescriptionDto.DateAddDescription}");
            TestContext.Out.WriteLine($"DateModDescription         : {enrollmentDescriptionDto.DateModDescription}");
            TestContext.Out.WriteLine($"UserAddDescription         : {enrollmentDescriptionDto.UserAddDescription}");
            TestContext.Out.WriteLine($"UserAddDescriptionFullName : {enrollmentDescriptionDto.UserAddDescriptionFullName}");
            TestContext.Out.WriteLine($"UserModDescription         : {enrollmentDescriptionDto.UserModDescription}");
            TestContext.Out.WriteLine($"UserModDescriptionFullName : {enrollmentDescriptionDto.UserModDescriptionFullName}");
            TestContext.Out.WriteLine($"Description                : {enrollmentDescriptionDto.Description}");
            TestContext.Out.WriteLine($"ActionExecuted             : {enrollmentDescriptionDto.ActionExecuted}");

            TestContext.Out.WriteLine("\nDelete enrollmentDescription by DeleteAsync(id) and check valid...");
            await _enrollmentsDescriptionServices.DeleteAsync(enrollmentDescriptionDto.Id);
            enrollmentDescriptionDto = await _enrollmentsDescriptionServices.GetAsync(enrollmentsDescriptionDto.Id);
            Assert.That(enrollmentDescriptionDto, Is.Null, "ERROR - delete enrollmentDescription");
        }
    }
}