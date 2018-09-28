using System.Collections.Generic;
using System.Threading.Tasks;
using FakeItEasy;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SoftwareApproach.TestingExtensions;
using TestApp.Business.Domain;
using TestApp.Business.Services;
using TestApp.Business.Services.Impl;
using TestApp.Core.Exceptions;
using TestApp.Core.Helpers;
using TestApp.Data.Domain;
using TestApp.Data.Entities;
using TestApp.Data.Repositories;
using TestApp.Data.Services;

using AddressDto = TestApp.Business.Domain.Address;
using AddressEntity = TestApp.Data.Entities.Address;

namespace TestApp.Business.Tests
{
    [TestClass]
    public class AddressServiceTest
    {
        private const string FAKE_SYSTEM_USER_ID = "someid";
        
        private AddressService UnitUnderTest { get; set; }
        private ITestAppUnitOfWork UowStub { get; set; }
        private IEntityMappingService<AddressDto, AddressEntity> MappingStub { get; set; }
        private IValidationService<AddressDto, AddressEntity> ValidationStub { get; set; }
        
        [TestInitialize]
        public void Initialize()
        {
            UowStub = A.Fake<ITestAppUnitOfWork>();
            MappingStub = new AddressMappingService();
            ValidationStub = A.Fake<IValidationService<AddressDto, AddressEntity>>();
            A.CallTo(() => UowStub.CustomerRepository).Returns(A.Fake<ICustomerRepository>());
            A.CallTo(() => UowStub.AddressRepository).Returns(A.Fake<IAddressRepository>());
            UnitUnderTest = new AddressService(UowStub, ValidationStub, MappingStub);
        }
        
        [TestMethod]
        public async Task GetAddresses_WithProperQuery_ReturnsPagedCollectionOfAddresses()
        {
            // Arrange
            const int totalCount = 1;
            var expected = new List<AddressEntity>
            {
                new ServiceAddress
                {
                    Name = "Test Name",
                    Country = "pl"
                }
            };
            
            A.CallTo(() => UowStub.AddressRepository.Get(A<AddressQuery>._)).Returns((expected, totalCount));
            
            // Act
            var actual = await UnitUnderTest.GetAddressesAsync(new AddressQuery(), FAKE_SYSTEM_USER_ID);
            
            //Assert
            A.CallTo(() => UowStub.AddressRepository.Get(A<AddressQuery>._)).MustHaveHappenedOnceExactly();
            A.CallTo(() => UowStub.SaveAsync()).MustNotHaveHappened();
            A.CallTo(() => ValidationStub.Validate(A<AddressDto>._)).MustNotHaveHappened();

            actual.PageIndex.ShouldEqual(0);
            actual.TotalPageCount.ShouldEqual(1);
            actual.TotalItemCount.ShouldEqual(1);
            expected[0].Name.ShouldEqual(actual.Items[0].Name);
            expected[0].Country.ShouldEqual(actual.Items[0].Country);
            expected[0].Street.ShouldBeNull();
            expected[0].Zip.ShouldBeNull();
            expected[0].City.ShouldBeNull();
        }
        
        [TestMethod]
        public async Task GetAddressByCustomerId_ReturnsProperAddressFromRepo()
        {
            // Arrange
            var expected = new ServiceAddress
            { 
                CustomerId = "AA001",
                Name = "Test Name", 
                Country = "pl"
            };
            A.CallTo(() => UowStub.AddressRepository.GetByCustomerIdAsync<ServiceAddress>(expected.CustomerId)).Returns(expected);
            
            // Act
            var actual = await UnitUnderTest.GetCustomerAddressAsync(expected.CustomerId, AddressType.Service, FAKE_SYSTEM_USER_ID);
            
            //Assert
            A.CallTo(() => UowStub.AddressRepository.GetByCustomerIdAsync<ServiceAddress>(expected.CustomerId))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => UowStub.SaveAsync()).MustNotHaveHappened();
            A.CallTo(() => ValidationStub.Validate(A<AddressDto>._)).MustNotHaveHappened();

            expected.CustomerId.ShouldEqual(actual.CustomerId);
            expected.Name.ShouldEqual(actual.Name);
            expected.Country.ShouldEqual(actual.Country);
            expected.Street.ShouldBeNull();
            expected.Zip.ShouldBeNull();
            expected.City.ShouldBeNull();
        }
        
        [TestMethod]
        public async Task GetAddressByCustomerId_IfNotExists_ThrowsNotFoundException()
        {
            // Arrange
            const string missingUserId = "someId";
            A.CallTo(() => UowStub.AddressRepository.GetByCustomerIdAsync<ServiceAddress>(missingUserId)).Returns(default(ServiceAddress));
            
            // Act
            await Assert.ThrowsExceptionAsync<NotFoundException>(() => 
                UnitUnderTest.GetCustomerAddressAsync(missingUserId, AddressType.Service, FAKE_SYSTEM_USER_ID));
            
            //Assert
            A.CallTo(() => UowStub.AddressRepository.GetByCustomerIdAsync<ServiceAddress>(missingUserId)).MustHaveHappenedOnceExactly();
            A.CallTo(() => UowStub.SaveAsync()).MustNotHaveHappened();
            A.CallTo(() => ValidationStub.Validate(A<AddressDto>._)).MustNotHaveHappened();
        }
        
        [TestMethod]
        public async Task Create_ValidAddress_Succeed()
        {
            // Arrange
            const string customerId = "AA001";
            var delta = new Delta<AddressDto>();
            delta.SetValue(s => s.AddressType, AddressType.Service);
            A.CallTo(() => UowStub.AddressRepository.Create(A<Delta<AddressEntity>>._, FAKE_SYSTEM_USER_ID)).Returns(new ServiceAddress());

            // Act
            await UnitUnderTest.CreateAddressAsync(customerId, delta, FAKE_SYSTEM_USER_ID);
            
            //Assert
            A.CallTo(() => UowStub.AddressRepository.Create(A<Delta<AddressEntity>>._, FAKE_SYSTEM_USER_ID)).MustHaveHappenedOnceExactly();
            A.CallTo(() => UowStub.SaveAsync()).MustHaveHappenedOnceExactly();
            A.CallTo(() => ValidationStub.Validate(A<AddressDto>._)).MustHaveHappenedOnceExactly();
        }
        
        [TestMethod]
        public async Task Create_InvalidAddress_ThrowsValidationException()
        {
            // Arrange
            const string customerId = "AA001";
            var delta = new Delta<AddressDto>();
            delta.SetValue(s => s.AddressType, AddressType.Service);
            A.CallTo(() => ValidationStub.Validate(A<AddressDto>._)).Returns(new List<string> {"some error"});

            // Act
            await Assert.ThrowsExceptionAsync<ValidationException>(() => 
                UnitUnderTest.CreateAddressAsync(customerId, delta, FAKE_SYSTEM_USER_ID));
            
            //Assert
            A.CallTo(() => UowStub.SaveAsync()).MustNotHaveHappened();
            A.CallTo(() => ValidationStub.Validate(A<AddressDto>._)).MustHaveHappenedOnceExactly();
        }
        
        [TestMethod]
        public async Task Create_Address_IfAddressExists_ThrowsConflictException()
        {
            // Arrange
            const string customerId = "AA001";
            var delta = new Delta<AddressDto>();
            delta.SetValue(s => s.AddressType, AddressType.Service);
            A.CallTo(() => UowStub.AddressRepository.ExistsByCustomerIdAsync<ServiceAddress>(customerId)).Returns(true);

            // Act
            await Assert.ThrowsExceptionAsync<ConflictException>(() => 
                UnitUnderTest.CreateAddressAsync(customerId, delta, FAKE_SYSTEM_USER_ID));
            
            //Assert
            A.CallTo(() => UowStub.SaveAsync()).MustNotHaveHappened();
            A.CallTo(() => ValidationStub.Validate(A<AddressDto>._)).MustNotHaveHappened();
        }
        
        [TestMethod]
        public async Task Update_ValidAddress_Succeed()
        {
            // Arrange
            const string customerId = "AA001";
            var delta = new Delta<AddressDto>();
            delta.SetValue(s => s.AddressType, AddressType.Service);
            A.CallTo(() => UowStub.AddressRepository.Update(A<Delta<AddressEntity>>._, A<AddressEntity>._, FAKE_SYSTEM_USER_ID))
                .Returns(new ServiceAddress());

            // Act
            await UnitUnderTest.UpdateAddressAsync(customerId, delta, FAKE_SYSTEM_USER_ID);
            
            //Assert
            A.CallTo(() => UowStub.AddressRepository.Update(A<Delta<AddressEntity>>._, A<AddressEntity>._, FAKE_SYSTEM_USER_ID))
                .MustHaveHappenedOnceExactly();
            
            A.CallTo(() => UowStub.SaveAsync()).MustHaveHappenedOnceExactly();
            A.CallTo(() => ValidationStub.Validate(A<AddressDto>._)).MustHaveHappenedOnceExactly();
        }
        
        [TestMethod]
        public async Task Update_InvalidAddress_ThrowsValidationException()
        {
            // Arrange
            const string customerId = "AA001";
            var delta = new Delta<AddressDto>();
            delta.SetValue(s => s.AddressType, AddressType.Service);
            A.CallTo(() => ValidationStub.Validate(A<AddressDto>._)).Returns(new List<string> {"some error"});

            // Act
            await Assert.ThrowsExceptionAsync<ValidationException>(() => 
                UnitUnderTest.UpdateAddressAsync(customerId, delta, FAKE_SYSTEM_USER_ID));
            
            //Assert
            A.CallTo(() => UowStub.SaveAsync()).MustNotHaveHappened();
            A.CallTo(() => ValidationStub.Validate(A<AddressDto>._)).MustHaveHappenedOnceExactly();
        }
        
        [TestMethod]
        public async Task Update_MissingAddress_ThrowsNotFoundException()
        {
            // Arrange
            const string missingCustomerId = "someId";
            var delta = new Delta<AddressDto>();
            delta.SetValue(s => s.AddressType, AddressType.Service);
            A.CallTo(() => UowStub.AddressRepository.GetByCustomerIdAsync<ServiceAddress>(missingCustomerId)).Returns(default(ServiceAddress));

            // Act
            await Assert.ThrowsExceptionAsync<NotFoundException>(() => 
                UnitUnderTest.UpdateAddressAsync(missingCustomerId, delta, FAKE_SYSTEM_USER_ID));
            
            //Assert
            A.CallTo(() => UowStub.SaveAsync()).MustNotHaveHappened();
        }
        
        [TestMethod]
        public async Task Delete_Address_Succeed()
        {
            // Act
            await UnitUnderTest.DeleteAddressAsync("someId", AddressType.Service, FAKE_SYSTEM_USER_ID);
            
            //Assert
            A.CallTo(() => UowStub.SaveAsync()).MustHaveHappened();
        }
        
        [TestMethod]
        public async Task Delete_MissingAddress_ThrowsNotFoundException()
        {
            // Arrange
            const string missingCustomerId = "someId";
            A.CallTo(() => UowStub.AddressRepository.GetByCustomerIdAsync<ServiceAddress>(missingCustomerId)).Returns(default(ServiceAddress));

            // Act
            await Assert.ThrowsExceptionAsync<NotFoundException>(() => 
                UnitUnderTest.DeleteAddressAsync(missingCustomerId, AddressType.Service, FAKE_SYSTEM_USER_ID));
            
            //Assert
            A.CallTo(() => UowStub.SaveAsync()).MustNotHaveHappened();
        }
    }
}