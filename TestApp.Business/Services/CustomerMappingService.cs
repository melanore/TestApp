using System.Linq;
using TestApp.Core.Helpers;
using TestApp.Data.Entities;
using TestApp.Data.Services;

using AddressDto = TestApp.Business.Domain.Address;
using AddressEntity = TestApp.Data.Entities.Address;
using CustomerDto = TestApp.Business.Domain.Customer;
using CustomerEntity = TestApp.Data.Entities.Customer;

namespace TestApp.Business.Services
{
    public class CustomerMappingService : IEntityMappingService<CustomerDto, CustomerEntity>
    {
        public CustomerMappingService(IEntityMappingService<AddressDto, AddressEntity> addressMappingService)
        {
            AddressMappingService = addressMappingService;
        }
        
        private IEntityMappingService<AddressDto, AddressEntity> AddressMappingService { get; }
        
        public Delta<CustomerEntity> Map(Delta<CustomerDto> seed)
        {
            var delta = new Delta<CustomerEntity>();
            if (seed.TryGetValue(s => s.Name, out var name)) delta.SetValue(s => s.Name, name);
            if (seed.TryGetValue(s => s.Street, out var street)) delta.SetValue(s => s.Street, street);
            if (seed.TryGetValue(s => s.Zip, out var zip)) delta.SetValue(s => s.Zip, zip);
            if (seed.TryGetValue(s => s.City, out var city)) delta.SetValue(s => s.City, city);
            if (seed.TryGetValue(s => s.Country, out var country)) delta.SetValue(s => s.Name, country);
            return delta;
        }

        public CustomerDto Map(CustomerEntity entity)
        {
            var customer = new CustomerDto();
            if (entity.Addresses?.Any() ?? false)
            {
                var invoiceAddress = entity.Addresses?.OfType<InvoiceAddress>().FirstOrDefault();
                if (invoiceAddress != null) customer.InvoiceAddress = AddressMappingService.Map(invoiceAddress);
                
                var deliveryAddress = entity.Addresses?.OfType<DeliveryAddress>().FirstOrDefault();
                if (deliveryAddress != null) customer.DeliveryAddress = AddressMappingService.Map(deliveryAddress);
                
                var serviceAddress = entity.Addresses?.OfType<ServiceAddress>().FirstOrDefault();
                if (serviceAddress != null) customer.ServiceAddress = AddressMappingService.Map(serviceAddress);
            }
            
            customer.Name = entity.Name;
            customer.Street = entity.Street;
            customer.Zip = entity.Zip;
            customer.City = entity.City;
            customer.Country = entity.Country;
            return customer;
        }
    }
}