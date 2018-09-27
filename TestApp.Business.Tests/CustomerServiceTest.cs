using System.Collections.Generic;
using System.Threading.Tasks;
using FakeItEasy;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SoftwareApproach.TestingExtensions;
using TestApp.Business.Services;
using TestApp.Business.Services.Impl;
using TestApp.Core.Exceptions;
using TestApp.Data.Domain;
using TestApp.Data.Entities;
using TestApp.Data.Repositories;
using TestApp.Data.Services;
using Address = TestApp.Data.Entities.Address;
using CustomerDto = TestApp.Business.Domain.Customer;
using CustomerEntity = TestApp.Data.Entities.Customer;

namespace TestApp.Business.Tests
{
    [TestClass]
    public class CustomerServiceTest
    {
        private const string FAKE_SYSTEM_USER_ID = "someid";
        
        private CustomerService UnitUnderTest { get; set; }
        private ITestAppUnitOfWork UowStub { get; set; }
        private IEntityMappingService<CustomerDto, CustomerEntity> MappingStub { get; set; }
        private IValidationService<CustomerDto, CustomerEntity> ValidationStub { get; set; }
        
        [TestInitialize]
        public void Initialize()
        {
            UowStub = A.Fake<ITestAppUnitOfWork>();
            MappingStub = new CustomerMappingService(new AddressMappingService());
            ValidationStub = A.Fake<IValidationService<CustomerDto, CustomerEntity>>();
            A.CallTo(() => UowStub.CustomerRepository).Returns(A.Fake<ICustomerRepository>());
            A.CallTo(() => UowStub.AddressRepository).Returns(A.Fake<IAddressRepository>());
            UnitUnderTest = new CustomerService(UowStub, ValidationStub, MappingStub);
        }
        
        [TestMethod]
        public async Task GetCustomersReturnsPagedCollectionOfCustomers()
        {
            // Arrange
            const int totalCount = 1;
            var expected = new List<CustomerEntity>
            {
                new CustomerEntity
                {
                    Id = "123",
                    Name = "Test Name",
                    Country = "pl",
                    Addresses = new List<Address>
                    {
                        new ServiceAddress {Name = "default"}
                    }
                }
            };
            
            A.CallTo(() => UowStub.CustomerRepository.Get(A<CustomerQuery>._)).Returns((expected, totalCount));
            
            // Act
            var actual = await UnitUnderTest.GetCustomersAsync(new CustomerQuery(), FAKE_SYSTEM_USER_ID);
            
            //Assert
            A.CallTo(() => UowStub.CustomerRepository.Get(A<CustomerQuery>._)).MustHaveHappenedOnceExactly();
            A.CallTo(() => UowStub.SaveAsync()).MustNotHaveHappened();
            A.CallTo(() => ValidationStub.Validate(A<CustomerDto>._)).MustNotHaveHappened();

            actual.PageIndex.ShouldEqual(0);
            actual.TotalPageCount.ShouldEqual(1);
            actual.TotalItemCount.ShouldEqual(1);
            expected[0].Id.ShouldEqual(actual.Items[0].Id);
            expected[0].Name.ShouldEqual(actual.Items[0].Name);
            expected[0].Country.ShouldEqual(actual.Items[0].Country);
            expected[0].Street.ShouldBeNull();
            expected[0].Zip.ShouldBeNull();
            expected[0].City.ShouldBeNull();
            expected[0].Addresses.ShouldNotBeNull();
            expected[0].Addresses.ShouldHaveCountOf(1);
        }
        
        [TestMethod]
        public async Task GetCustomerByIdReturnsProperCustomerFromRepo()
        {
            // Arrange
            var expected = new CustomerEntity 
            { 
                Id = "123", 
                Name = "Test Name", 
                Country = "pl", 
                Addresses = new List<Address>
                {
                    new ServiceAddress { Name = "default" }
                } 
            };
            A.CallTo(() => UowStub.CustomerRepository.GetByIdAsync(expected.Id, true)).Returns(expected);
            
            // Act
            var actual = await UnitUnderTest.GetCustomerAsync(expected.Id, FAKE_SYSTEM_USER_ID);
            
            //Assert
            A.CallTo(() => UowStub.CustomerRepository.GetByIdAsync(expected.Id, true)).MustHaveHappenedOnceExactly();
            A.CallTo(() => UowStub.SaveAsync()).MustNotHaveHappened();
            A.CallTo(() => ValidationStub.Validate(A<CustomerDto>._)).MustNotHaveHappened();

            expected.Id.ShouldEqual(actual.Id);
            expected.Name.ShouldEqual(actual.Name);
            expected.Country.ShouldEqual(actual.Country);
            expected.Street.ShouldBeNull();
            expected.Zip.ShouldBeNull();
            expected.City.ShouldBeNull();
            expected.Addresses.ShouldNotBeNull();
            expected.Addresses.ShouldHaveCountOf(1);
        }
        
        [TestMethod]
        public async Task GetCustomerByInvalidIdThrowsNotFoundException()
        {
            // Arrange
            const string missingUserId = "someId";
            A.CallTo(() => UowStub.CustomerRepository.GetByIdAsync(missingUserId, true)).Returns(default(CustomerEntity));
            
            // Act
            await Assert.ThrowsExceptionAsync<NotFoundException>(() => UnitUnderTest.GetCustomerAsync(missingUserId, FAKE_SYSTEM_USER_ID));
            
            //Assert
            A.CallTo(() => UowStub.CustomerRepository.GetByIdAsync(missingUserId, true)).MustHaveHappenedOnceExactly();
            A.CallTo(() => UowStub.SaveAsync()).MustNotHaveHappened();
            A.CallTo(() => ValidationStub.Validate(A<CustomerDto>._)).MustNotHaveHappened();
        }
    }
}