using System;
using TestApp.Business.Const;
using TestApp.Business.Domain;
using AddressDto = TestApp.Business.Domain.Address;
using AddressEntity = TestApp.Data.Entities.Address;

namespace TestApp.Business.Services.Impl
{
    public class AddressValidationService : BaseValidationService<AddressDto, AddressEntity>
    {
        public override Func<AddressDto, (bool isSuccessfull, string errorMessage)>[] GetValidationPipeline() => new Func<AddressDto, (bool isSuccessfull, string errorMessage)>[]
        {
            address => !address.AddressType.HasValue
                ? (false, string.Format(ValidationErrors.INVALID_VALUE, nameof(address.AddressType), address.AddressType, 
                    $"[{AddressType.Service}, {AddressType.Delivery}, {AddressType.Invoice}]")) 
                : (true, default),
            address => ValidateLengthAndRequired(nameof(address.Name), address.Name, 100),
            address => ValidateLengthAndRequired(nameof(address.Street), address.Street, 100),
            address => ValidateLengthAndRequired(nameof(address.Zip), address.Zip, 20),
            address => ValidateLengthAndRequired(nameof(address.City), address.City, 100),
            address => ValidateLengthAndRequired(nameof(address.Country), address.Country, 2),
            address => !string.IsNullOrEmpty(address.Country) && !CountryCodeLookup.Contains(address.Country) 
                ? (false, string.Format(ValidationErrors.NOT_A_COUNTRY_CODE, address.Country)) 
                : (true, default)
        };
    }
}