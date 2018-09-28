using System.Collections.Generic;
using FakeItEasy;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SoftwareApproach.TestingExtensions;
using TestApp.Business.Services.Impl;
using TestApp.Core.Helpers;
using TestApp.Data.Entities;
using TestApp.Data.Services;
using CustomerDto = TestApp.Business.Domain.Customer;
using AddressDto = TestApp.Business.Domain.Address;
using CustomerEntity = TestApp.Data.Entities.Customer;
using AddressEntity = TestApp.Data.Entities.Address;

namespace TestApp.Business.Tests.Mapping
{
    [TestClass]
    public class CustomerMappingServiceTest
    {
        private IEntityMappingService<AddressDto, AddressEntity> AddressMappingServiceStub { get; set; }
        private CustomerMappingService UnitUnderTest { get; set; } 

        [TestInitialize]
        public void Initialize()
        {
            AddressMappingServiceStub = A.Fake<IEntityMappingService<AddressDto, AddressEntity>>();
            UnitUnderTest = new CustomerMappingService(AddressMappingServiceStub);
        }
        
        [TestMethod]
        public void ProperCustomerDelta_ShouldBeMapped()
        {
            // Arrange
            var delta = CreateDelta();

            // Act
            var entityDelta = UnitUnderTest.Map(delta);
            
            //Assert
            entityDelta.GetValue(s => s.Id).ShouldEqual(delta.GetValue(s => s.Id));
            entityDelta.GetValue(s => s.Name).ShouldEqual(delta.GetValue(s => s.Name));
            entityDelta.GetValue(s => s.Street).ShouldEqual(delta.GetValue(s => s.Street));
            entityDelta.GetValue(s => s.Zip).ShouldEqual(delta.GetValue(s => s.Zip));
            entityDelta.GetValue(s => s.City).ShouldEqual(delta.GetValue(s => s.City));
            entityDelta.GetValue(s => s.Country).ShouldEqual(delta.GetValue(s => s.Country));
        }
        
        [TestMethod]
        public void CustomerDelta_WithNullProperty_ShouldBeMapped_WithNullProperty()
        {
            // Arrange
            var delta = CreateDelta();
            delta.SetValue(s => s.Name, default);

            // Act
            var entityDelta = UnitUnderTest.Map(delta);
            
            //Assert
            entityDelta.TryGetValue(s => s.Name, out var name).ShouldBeTrue();
            name.ShouldBeNull();
        }
        
        [TestMethod]
        public void ProperCustomer_ShouldBeMapped()
        {
            // Arrange
            var entity = CreateEntity();

            // Act
            var record = UnitUnderTest.Map(entity);
            
            //Assert
            record.Id.ShouldEqual(entity.Id);
            record.Name.ShouldEqual(entity.Name);
            record.Street.ShouldEqual(entity.Street);
            record.Zip.ShouldEqual(entity.Zip);
            record.City.ShouldEqual(entity.City);
            record.Country.ShouldEqual(entity.Country);
        }
        
        [TestMethod]
        public void Customer_WithAddresses_ShouldCallAddressMappingService()
        {
            // Arrange
            var entity = CreateEntity();
            var address = new DeliveryAddress();
            entity.Addresses = new List<AddressEntity> { address };

            // Act
            UnitUnderTest.Map(entity);
            
            //Assert
            A.CallTo(() => AddressMappingServiceStub.Map(address)).MustHaveHappenedOnceExactly();
        }

        private static Delta<CustomerDto> CreateDelta()
        {
            var delta = new Delta<CustomerDto>();
            delta.SetValue(s => s.Name, "some name");
            delta.SetValue(s => s.Street, "some street");
            delta.SetValue(s => s.Zip, "some zip");
            delta.SetValue(s => s.City, "some city");
            delta.SetValue(s => s.Country, "pl");
            return delta;
        }
        
        private static CustomerEntity CreateEntity() => new CustomerEntity
        {
            Id = "AA001",
            Name = "some name",
            Street = "some street",
            Zip = "some zip",
            City = "some city",
            Country = "pl"
        };
    }
}