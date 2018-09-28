using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SoftwareApproach.TestingExtensions;
using TestApp.Business.Domain;
using TestApp.Business.Services.Impl;

namespace TestApp.Business.Tests.Validation
{
    [TestClass]
    public class AddressValidationServiceTest
    {
        private AddressValidationService UnitUnderTest { get; } = new AddressValidationService();

        [TestMethod]
        public void ProperAddress_ShouldSucceed()
        {
            // Arrange
            var address = CreateValidAddress();

            // Act
            var errors = UnitUnderTest.Validate(address);
            
            //Assert
            errors.ShouldBeEmpty();
        }
        
        [TestMethod]
        public void Address_WithMissingAddressType_ShouldFail()
        {
            // Arrange
            var address = CreateValidAddress();
            address.AddressType = default;

            // Act
            var errors = UnitUnderTest.Validate(address);
            
            //Assert
            errors.ShouldHaveCountOf(1);
        }
        
        [TestMethod]
        public void Address_WithTooBigProperty_ShouldFail()
        {
            // Arrange
            const int invalidLength = 21;
            var customer = CreateValidAddress();
            customer.Zip = Enumerable.Range(default, invalidLength).Aggregate(new StringBuilder(), (builder, i) => builder.Append(i)).ToString();

            // Act
            var errors = UnitUnderTest.Validate(customer);
            
            //Assert
            errors.ShouldHaveCountOf(1);
        }
        
        [TestMethod]
        public void Address_WithInvalidCountryCode_ShouldFail()
        {
            // Arrange
            var customer = CreateValidAddress();
            customer.Country = "12";

            // Act
            var errors = UnitUnderTest.Validate(customer);
            
            //Assert
            errors.ShouldHaveCountOf(1);
        }
        
        private static Address CreateValidAddress() => new Address
        {
            Name = "test name",
            Street = "Some street",
            Zip = "zip",
            City = "some city",
            Country = "pl",
            AddressType = AddressType.Delivery
        };
    }
}