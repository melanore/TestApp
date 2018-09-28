using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SoftwareApproach.TestingExtensions;
using TestApp.Business.Domain;
using TestApp.Business.Services.Impl;
using TestApp.Core.Helpers;
using TestApp.Data.Entities;
using AddressDto = TestApp.Business.Domain.Address;
using AddressEntity = TestApp.Data.Entities.Address;

namespace TestApp.Business.Tests.Mapping
{
    [TestClass]
    public class AddressMappingServiceTest
    {
        private AddressMappingService UnitUnderTest { get; } = new AddressMappingService();

        [TestMethod]
        public void ProperAddressDelta_ShouldBeMapped()
        {
            // Arrange
            var delta = CreateDelta(AddressType.Delivery);

            // Act
            var entityDelta = UnitUnderTest.Map(delta);
            
            //Assert
            entityDelta.TryGetValue(s => s.AddressType, out var discriminator).ShouldBeTrue();
            discriminator.ShouldEqual("D");
            entityDelta.GetValue(s => s.CustomerId).ShouldEqual(delta.GetValue(s => s.CustomerId));
            entityDelta.GetValue(s => s.Name).ShouldEqual(delta.GetValue(s => s.Name));
            entityDelta.GetValue(s => s.Street).ShouldEqual(delta.GetValue(s => s.Street));
            entityDelta.GetValue(s => s.Zip).ShouldEqual(delta.GetValue(s => s.Zip));
            entityDelta.GetValue(s => s.City).ShouldEqual(delta.GetValue(s => s.City));
            entityDelta.GetValue(s => s.Country).ShouldEqual(delta.GetValue(s => s.Country));
        }
        
        [TestMethod]
        public void AddressDelta_WithMissingAddressType_ThrowsNotSupportedException()
        {
            // Arrange
            var delta = CreateDelta(default);

            // Act
            Assert.ThrowsException<NotSupportedException>(() => UnitUnderTest.Map(delta));
        }
        
        [TestMethod]
        public void AddressDelta_AddressTypes_ShouldBeProperlyMapped()
        {
            // Arrange
            var delivery = CreateDelta(AddressType.Delivery);
            var service = CreateDelta(AddressType.Service);
            var invoice = CreateDelta(AddressType.Invoice);

            // Act
            var deliveryEntityDelta = UnitUnderTest.Map(delivery);
            var serviceEntityDelta = UnitUnderTest.Map(service);
            var invoiceEntityDelta = UnitUnderTest.Map(invoice);
            
            //Assert
            deliveryEntityDelta.GetValue(s => s.AddressType).ShouldEqual("D");
            serviceEntityDelta.GetValue(s => s.AddressType).ShouldEqual("S");
            invoiceEntityDelta.GetValue(s => s.AddressType).ShouldEqual("I");
        }
        
        [TestMethod]
        public void AddressDelta_WithNullProperty_ShouldBeMapped_WithNullProperty()
        {
            // Arrange
            var delta = CreateDelta(AddressType.Delivery);
            delta.SetValue(s => s.Name, default);

            // Act
            var entityDelta = UnitUnderTest.Map(delta);
            
            //Assert
            entityDelta.TryGetValue(s => s.Name, out var name).ShouldBeTrue();
            name.ShouldBeNull();
        }
        
        [TestMethod]
        public void ProperAddress_ShouldBeMapped()
        {
            // Arrange
            var entity = CreateEntity<DeliveryAddress>();

            // Act
            var record = UnitUnderTest.Map(entity);
            
            //Assert
            record.AddressType.ShouldEqual(AddressType.Delivery);
            record.CustomerId.ShouldEqual(entity.CustomerId);
            record.Name.ShouldEqual(entity.Name);
            record.Street.ShouldEqual(entity.Street);
            record.Zip.ShouldEqual(entity.Zip);
            record.City.ShouldEqual(entity.City);
            record.Country.ShouldEqual(entity.Country);
        }
        
        [TestMethod]
        public void Address_WithMissingAddressType_ThrowsNotSupportedException()
        {
            // Arrange
            var delta = CreateEntity<AddressEntity>();

            // Act
            Assert.ThrowsException<NotSupportedException>(() => UnitUnderTest.Map(delta));
        }
        
        [TestMethod]
        public void Address_AddressTypes_ShouldBeProperlyMapped()
        {
            // Arrange
            var delivery = CreateEntity<DeliveryAddress>();
            var service = CreateEntity<ServiceAddress>();
            var invoice = CreateEntity<InvoiceAddress>();

            // Act
            var deliveryRecord = UnitUnderTest.Map(delivery);
            var serviceRecord = UnitUnderTest.Map(service);
            var invoiceRecord = UnitUnderTest.Map(invoice);
            
            //Assert
            deliveryRecord.AddressType.ShouldEqual(AddressType.Delivery);
            serviceRecord.AddressType.ShouldEqual(AddressType.Service);
            invoiceRecord.AddressType.ShouldEqual(AddressType.Invoice);
        }

        private static Delta<AddressDto> CreateDelta(AddressType? addressType)
        {
            var delta = new Delta<AddressDto>();
            delta.SetValue(s => s.AddressType, addressType);
            delta.SetValue(s => s.Name, "some name");
            delta.SetValue(s => s.Street, "some street");
            delta.SetValue(s => s.Zip, "some zip");
            delta.SetValue(s => s.City, "some city");
            delta.SetValue(s => s.Country, "pl");
            return delta;
        }
        
        private static AddressEntity CreateEntity<T>() where T : AddressEntity, new() => new T
        {
            CustomerId = "AA001",
            Name = "some name",
            Street = "some street",
            Zip = "some zip",
            City = "some city",
            Country = "pl"
        };
    }
}