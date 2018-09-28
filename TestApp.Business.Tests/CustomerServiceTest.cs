using System.Collections.Generic;
using System.Threading.Tasks;
using FakeItEasy;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SoftwareApproach.TestingExtensions;
using TestApp.Business.Services;
using TestApp.Business.Services.Impl;
using TestApp.Core.Exceptions;
using TestApp.Core.Helpers;
using TestApp.Data.Domain;
using TestApp.Data.Entities;
using TestApp.Data.Repositories;
using TestApp.Data.Services;

using AddressEntity = TestApp.Data.Entities.Address;
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
        public async Task GetCustomers_WithProperQuery_ReturnsPagedCollectionOfCustomers()
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
                    Addresses = new List<AddressEntity>
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
        public async Task GetCustomerById_ReturnsProperCustomerFromRepo()
        {
            // Arrange
            var expected = new CustomerEntity 
            { 
                Id = "123", 
                Name = "Test Name", 
                Country = "pl", 
                Addresses = new List<AddressEntity>
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
        public async Task GetCustomerById_IfNotExists_ThrowsNotFoundException()
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
        
        [TestMethod]
        public async Task Create_ValidCustomer_Succeed()
        {
            // Arrange
            var delta = A.Fake<Delta<CustomerDto>>();

            // Act
            await UnitUnderTest.CreateCustomerAsync(delta, FAKE_SYSTEM_USER_ID);
            
            //Assert
            A.CallTo(() => UowStub.CustomerRepository.Create(A<Delta<CustomerEntity>>._, FAKE_SYSTEM_USER_ID)).MustHaveHappenedOnceExactly();
            A.CallTo(() => UowStub.SaveAsync()).MustHaveHappenedOnceExactly();
            A.CallTo(() => ValidationStub.Validate(A<CustomerDto>._)).MustHaveHappenedOnceExactly();
        }
        
        [TestMethod]
        public async Task Create_InvalidCustomer_ThrowsValidationException()
        {
            // Arrange
            var delta = A.Fake<Delta<CustomerDto>>();
            A.CallTo(() => ValidationStub.Validate(A<CustomerDto>._)).Returns(new List<string> {"some error"});

            // Act
            await Assert.ThrowsExceptionAsync<ValidationException>(() => UnitUnderTest.CreateCustomerAsync(delta, FAKE_SYSTEM_USER_ID));
            
            //Assert
            A.CallTo(() => UowStub.SaveAsync()).MustNotHaveHappened();
            A.CallTo(() => ValidationStub.Validate(A<CustomerDto>._)).MustHaveHappenedOnceExactly();
        }
        
        [TestMethod]
        public async Task Update_ValidCustomer_Succeed()
        {
            // Arrange
            const string customerId = "AA001";
            var delta = A.Fake<Delta<CustomerDto>>();

            // Act
            await UnitUnderTest.UpdateCustomerAsync(customerId, delta, FAKE_SYSTEM_USER_ID);
            
            //Assert
            A.CallTo(() => UowStub.CustomerRepository.Update(A<Delta<CustomerEntity>>._, A<CustomerEntity>._, FAKE_SYSTEM_USER_ID))
                .MustHaveHappenedOnceExactly();
            
            A.CallTo(() => UowStub.SaveAsync()).MustHaveHappenedOnceExactly();
            A.CallTo(() => ValidationStub.Validate(A<CustomerDto>._)).MustHaveHappenedOnceExactly();
        }
        
        [TestMethod]
        public async Task Update_InvalidCustomer_ThrowsValidationException()
        {
            // Arrange
            const string customerId = "AA001";
            var delta = A.Fake<Delta<CustomerDto>>();
            A.CallTo(() => ValidationStub.Validate(A<CustomerDto>._)).Returns(new List<string> {"some error"});

            // Act
            await Assert.ThrowsExceptionAsync<ValidationException>(() => UnitUnderTest.UpdateCustomerAsync(customerId, delta, FAKE_SYSTEM_USER_ID));
            
            //Assert
            A.CallTo(() => UowStub.SaveAsync()).MustNotHaveHappened();
            A.CallTo(() => ValidationStub.Validate(A<CustomerDto>._)).MustHaveHappenedOnceExactly();
        }
        
        [TestMethod]
        public async Task Update_MissingCustomer_ThrowsNotFoundException()
        {
            // Arrange
            const string missingCustomerId = "someId";
            var delta = A.Fake<Delta<CustomerDto>>();
            A.CallTo(() => UowStub.CustomerRepository.GetByIdAsync(missingCustomerId, false)).Returns(default(CustomerEntity));

            // Act
            await Assert.ThrowsExceptionAsync<NotFoundException>(() => UnitUnderTest.UpdateCustomerAsync(missingCustomerId, delta, FAKE_SYSTEM_USER_ID));
            
            //Assert
            A.CallTo(() => UowStub.SaveAsync()).MustNotHaveHappened();
        }
        
        [TestMethod]
        public async Task Delete_Customer_Succeed()
        {
            // Act
            await UnitUnderTest.DeleteCustomerAsync("someId", FAKE_SYSTEM_USER_ID);
            
            //Assert
            A.CallTo(() => UowStub.SaveAsync()).MustHaveHappened();
        }
        
        [TestMethod]
        public async Task Delete_MissingCustomer_ThrowsNotFoundException()
        {
            // Arrange
            const string missingCustomerId = "someId";
            A.CallTo(() => UowStub.CustomerRepository.GetByIdAsync(missingCustomerId, false)).Returns(default(CustomerEntity));

            // Act
            await Assert.ThrowsExceptionAsync<NotFoundException>(() => UnitUnderTest.DeleteCustomerAsync(missingCustomerId, FAKE_SYSTEM_USER_ID));
            
            //Assert
            A.CallTo(() => UowStub.SaveAsync()).MustNotHaveHappened();
        }
    }
}