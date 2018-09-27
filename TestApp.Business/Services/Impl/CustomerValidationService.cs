using System;
using TestApp.Business.Const;
using CustomerDto = TestApp.Business.Domain.Customer;
using CustomerEntity = TestApp.Data.Entities.Customer;

namespace TestApp.Business.Services.Impl
{
    public class CustomerValidationService : BaseValidationService<CustomerDto, CustomerEntity>
    {
        public override Func<CustomerDto, (bool isSuccessfull, string errorMessage)>[] GetValidationPipeline() => new Func<CustomerDto, (bool isSuccessfull, string errorMessage)>[]
        {
            customer => ValidateLengthAndRequired(nameof(customer.Name), customer.Name, 100, isRequired: true),
            customer => ValidateLengthAndRequired(nameof(customer.Street), customer.Street, 100),
            customer => ValidateLengthAndRequired(nameof(customer.Zip), customer.Zip, 20),
            customer => ValidateLengthAndRequired(nameof(customer.City), customer.City, 100),
            customer => ValidateLengthAndRequired(nameof(customer.Country), customer.Country, 2),
            customer => !string.IsNullOrEmpty(customer.Country) && !CountryCodeLookup.Contains(customer.Country) 
                ? (false, string.Format(ValidationErrors.NOT_A_COUNTRY_CODE, customer.Country)) 
                : (true, default)
        };
    }
}