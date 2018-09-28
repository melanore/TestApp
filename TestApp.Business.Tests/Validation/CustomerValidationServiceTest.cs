using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SoftwareApproach.TestingExtensions;
using TestApp.Business.Domain;
using TestApp.Business.Services.Impl;

namespace TestApp.Business.Tests.Validation
{
    [TestClass]
    public class CustomerValidationServiceTest
    {
        private CustomerValidationService UnitUnderTest { get; } = new CustomerValidationService();

        [TestMethod]
        public void ProperCustomer_ShouldSucceed()
        {
            // Arrange
            var customer = CreateValidCustomer();

            // Act
            var errors = UnitUnderTest.Validate(customer);
            
            //Assert
            errors.ShouldBeEmpty();
        }
        
        [TestMethod]
        public void Customer_WithMissingName_ShouldFail()
        {
            // Arrange
            var customer = CreateValidCustomer();
            customer.Name = string.Empty;

            // Act
            var errors = UnitUnderTest.Validate(customer);
            
            //Assert
            errors.ShouldHaveCountOf(1);
        }
        
        [TestMethod]
        public void Customer_WithNullName_ShouldFail()
        {
            // Arrange
            var customer = CreateValidCustomer();
            customer.Name = default;

            // Act
            var errors = UnitUnderTest.Validate(customer);
            
            //Assert
            errors.ShouldHaveCountOf(1);
        }
        
        [TestMethod]
        public void Customer_WithTooBigProperty_ShouldFail()
        {
            // Arrange
            const int invalidLength = 21;
            var customer = CreateValidCustomer();
            customer.Zip = Enumerable.Range(default, invalidLength).Aggregate(new StringBuilder(), (builder, i) => builder.Append(i)).ToString();

            // Act
            var errors = UnitUnderTest.Validate(customer);
            
            //Assert
            errors.ShouldHaveCountOf(1);
        }
        
        [TestMethod]
        public void Customer_WithInvalidCountryCode_ShouldFail()
        {
            // Arrange
            var customer = CreateValidCustomer();
            customer.Country = "12";

            // Act
            var errors = UnitUnderTest.Validate(customer);
            
            //Assert
            errors.ShouldHaveCountOf(1);
        }
        
        private static Customer CreateValidCustomer() => new Customer
        {
            Name = "test name",
            Street = "Some street",
            Zip = "zip",
            City = "some city",
            Country = "pl"
        };
    }
}