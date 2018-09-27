using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TestApp.Business.Domain;
using TestApp.Core.Exceptions;
using TestApp.Core.Helpers;
using TestApp.Data.Domain;
using TestApp.Data.Entities;
using TestApp.Data.Repositories;
using TestApp.Data.Services;
using AddressDto = TestApp.Business.Domain.Address;
using AddressEntity = TestApp.Data.Entities.Address;

namespace TestApp.Business.Services.Impl
{
    public class AddressService : IAddressService
    {
        public AddressService(
            ITestAppUnitOfWork uow, 
            IValidationService<AddressDto, AddressEntity> addressValidationService,
            IEntityMappingService<AddressDto, AddressEntity> addressMappingService)
        {
            Uow = uow;
            AddressValidationService = addressValidationService;
            AddressMappingService = addressMappingService;
        }
        
        private ITestAppUnitOfWork Uow { get; }
        private IValidationService<AddressDto, AddressEntity> AddressValidationService { get; }
        private IEntityMappingService<AddressDto, AddressEntity> AddressMappingService { get; }
        
        public async Task<List<AddressDto>> GetCustomerAddressesAsync(string customerId, string systemUserId)
        {
            if (string.IsNullOrEmpty(customerId)) throw new ValidationException("CustomerId is required.");
            var (result, _) = await Uow.AddressRepository.Get(new AddressQuery {CustomerId = customerId});
            var addresses = result.Select(AddressMappingService.Map).ToList();
            return addresses;
        }
        
        public async Task<PagedResource<AddressDto>> GetAddressesAsync(AddressQuery query, string systemUserId)
        {
            var (result, totalCount) = await Uow.AddressRepository.Get(query);
            var addresses = result.Select(AddressMappingService.Map).ToList();
            return new PagedResource<AddressDto>(addresses, totalCount, query);
        }
        
        public async Task<AddressDto> GetCustomerAddressAsync(string customerId, AddressType addressType, string systemUserId)
        {
            if (string.IsNullOrEmpty(customerId)) throw new ValidationException("CustomerId is required.");
            var address = await GetAddressByCustomerIdAndTypeAsync(customerId, addressType);
            if (address == null) throw new NotFoundException($"Address type {addressType} for customer id {customerId} not found.");
            return AddressMappingService.Map(address);
        }
        
        public async Task<AddressDto> CreateAddressAsync(string customerId, Delta<AddressDto> delta, string systemUserId)
        {
            if (string.IsNullOrEmpty(customerId))
                throw new ValidationException("CustomerId is required.");
            if (!delta.TryGetValue(s => s.AddressType, out var addressType) || !addressType.HasValue)
                throw new ValidationException("Address type is required.");
            if (await AddressExistsByCustomerIdAndTypeAsync(customerId, addressType.Value))
                throw new ConflictException($"Address of type {addressType.Value} exists on customer {customerId}.");
            if (AddressValidationService.Validate(delta.ToObject()) is var errors && errors.Any()) 
                throw new ValidationException("Invalid address model.", errors);
            
            delta.SetValue(s => s.CustomerId, customerId);
            var entityDelta = AddressMappingService.Map(delta);
            var address = Uow.AddressRepository.Create(entityDelta, systemUserId);
            await Uow.SaveAsync();
            return AddressMappingService.Map(address);
        }
        
        public async Task<AddressDto> UpdateAddressAsync(string customerId, Delta<AddressDto> delta, string systemUserId)
        {
            if (string.IsNullOrEmpty(customerId))
                throw new ValidationException("CustomerId is required.");
            if (!delta.TryGetValue(s => s.AddressType, out var addressType) || !addressType.HasValue)
                throw new ValidationException("Address type is required.");
            var addressEntity = await GetAddressByCustomerIdAndTypeAsync(customerId, addressType.Value);
            if (addressEntity == null) 
                throw new NotFoundException($"Address type {addressType} for customer id {customerId} not found.");

            var entityDelta = AddressMappingService.Map(delta);
            if (entityDelta.GetValue(s => s.AddressType) != addressEntity.AddressType)
                throw new ValidationException($"Address type is readonly. Please delete existing address and add a new one of desired type.");
            
            entityDelta.Apply(addressEntity);

            var address = AddressMappingService.Map(addressEntity);
            if (AddressValidationService.Validate(address) is var errors && errors.Any()) 
                throw new ValidationException("Invalid address model.", errors);
            
            Uow.AddressRepository.Update(entityDelta, addressEntity, systemUserId);
            await Uow.SaveAsync();

            return address;
        }

        public async Task DeleteAddressAsync(string customerId, AddressType type, string systemUserId)
        {
            if (string.IsNullOrEmpty(customerId)) throw new ValidationException("CustomerId is required.");
            var addressEntity = await GetAddressByCustomerIdAndTypeAsync(customerId, type);
            if (addressEntity == null) throw new NotFoundException($"Address type {type} for customer id {customerId} not found.");
            Uow.AddressRepository.Delete(addressEntity);
            await Uow.SaveAsync();
        }
        
        private async Task<AddressEntity> GetAddressByCustomerIdAndTypeAsync(string customerId, AddressType type)
        {
            switch (type)
            {
                case AddressType.Invoice: return await Uow.AddressRepository.GetByCustomerIdAsync<InvoiceAddress>(customerId);
                case AddressType.Delivery: return await Uow.AddressRepository.GetByCustomerIdAsync<DeliveryAddress>(customerId);
                case AddressType.Service: return await Uow.AddressRepository.GetByCustomerIdAsync<ServiceAddress>(customerId);
                default: throw new ValidationException($"Invalid address type: {type}");
            }
        }
        
        private async Task<bool> AddressExistsByCustomerIdAndTypeAsync(string customerId, AddressType type)
        {
            switch (type)
            {
                case AddressType.Invoice: return await Uow.AddressRepository.ExistsByCustomerIdAsync<InvoiceAddress>(customerId);
                case AddressType.Delivery: return await Uow.AddressRepository.ExistsByCustomerIdAsync<DeliveryAddress>(customerId);
                case AddressType.Service: return await Uow.AddressRepository.ExistsByCustomerIdAsync<ServiceAddress>(customerId);
                default: throw new ValidationException($"Invalid address type: {type}");
            }
        }
    }
}