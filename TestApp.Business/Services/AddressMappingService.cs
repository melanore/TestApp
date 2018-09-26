using System;
using TestApp.Business.Domain;
using TestApp.Core.Helpers;
using TestApp.Data.Entities;
using TestApp.Data.Services;
using AddressDto = TestApp.Business.Domain.Address;
using AddressEntity = TestApp.Data.Entities.Address;

namespace TestApp.Business.Services
{
    public class AddressMappingService : IEntityMappingService<AddressDto, AddressEntity>
    {
        public Delta<AddressEntity> Map(Delta<AddressDto> seed)
        {
            var delta = new Delta<AddressEntity>();
            var addressType = MapAddressType(seed.GetValue(s => s.AddressType));
            delta.SetValue(s => s.AddressType, addressType);

            if (seed.TryGetValue(s => s.Name, out var name)) delta.SetValue(s => s.Name, name);
            if (seed.TryGetValue(s => s.Street, out var street)) delta.SetValue(s => s.Street, street);
            if (seed.TryGetValue(s => s.Zip, out var zip)) delta.SetValue(s => s.Zip, zip);
            if (seed.TryGetValue(s => s.City, out var city)) delta.SetValue(s => s.City, city);
            if (seed.TryGetValue(s => s.Country, out var country)) delta.SetValue(s => s.Name, country);
            return delta;
        }

        public AddressDto Map(AddressEntity entity)
        {
            return new AddressDto
            {
                AddressType = MapAddressType(entity.AddressType),
                Name = entity.Name,
                Street = entity.Street,
                Zip = entity.Zip,
                City = entity.City,
                Country = entity.Country
            };
        }

        private static string MapAddressType(AddressType addressType)
        {
            switch (addressType)
            {
                case AddressType.Delivery: return DeliveryAddress.TYPE;
                case AddressType.Invoice: return InvoiceAddress.TYPE;
                case AddressType.Service: return ServiceAddress.TYPE;
                default: throw new NotSupportedException($"Address type {addressType} is not supported.");
            }
        }

        private static AddressType MapAddressType(string addressType)
        {
            switch (addressType)
            {
                case DeliveryAddress.TYPE: return AddressType.Delivery;
                case InvoiceAddress.TYPE: return AddressType.Invoice;
                case ServiceAddress.TYPE: return AddressType.Service;
                default: throw new NotSupportedException($"Address type {addressType} is not supported.");
            }
        }
    }
}