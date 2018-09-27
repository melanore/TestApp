using System.Linq;
using System.Threading.Tasks;
using TestApp.Core.Exceptions;
using TestApp.Core.Helpers;
using TestApp.Data.Domain;
using TestApp.Data.Repositories;
using TestApp.Data.Services;
using CustomerDto = TestApp.Business.Domain.Customer;
using CustomerEntity = TestApp.Data.Entities.Customer;

namespace TestApp.Business.Services.Impl
{
    public class CustomerService : ICustomerService
    {
        public CustomerService(
            ITestAppUnitOfWork uow, 
            IValidationService<CustomerDto, CustomerEntity> customerValidationService,
            IEntityMappingService<CustomerDto, CustomerEntity> customerMappingService)
        {
            Uow = uow;
            CustomerValidationService = customerValidationService;
            CustomerMappingService = customerMappingService;
        }
        
        private ITestAppUnitOfWork Uow { get; }
        private IValidationService<CustomerDto, CustomerEntity> CustomerValidationService { get; }
        private IEntityMappingService<CustomerDto, CustomerEntity> CustomerMappingService { get; }
        
        public async Task<PagedResource<CustomerDto>> GetCustomersAsync(CustomerQuery query, string systemUserId)
        {
            var (result, totalCount) = await Uow.CustomerRepository.Get(query);
            var customers = result.Select(CustomerMappingService.Map).ToList();
            return new PagedResource<CustomerDto>(customers, totalCount, query);
        }
        
        public async Task<CustomerDto> GetCustomerAsync(string customerId, string systemUserId)
        {
            var customerEntity = await Uow.CustomerRepository.GetByIdAsync(customerId, true);
            if (customerEntity == null) throw new NotFoundException($"Customer with id {customerId} not found.");
            return CustomerMappingService.Map(customerEntity);
        }
        
        public async Task<CustomerDto> CreateCustomerAsync(Delta<CustomerDto> delta, string systemUserId)
        {
            if (CustomerValidationService.Validate(delta.ToObject()) is var errors && errors.Any()) 
                throw new ValidationException("Invalid customer model.", errors);
            var entityDelta = CustomerMappingService.Map(delta);
            var customer = Uow.CustomerRepository.Create(entityDelta, systemUserId);
            await Uow.SaveAsync();
            return CustomerMappingService.Map(customer);
        }
        
        public async Task<CustomerDto> UpdateCustomerAsync(string customerId, Delta<CustomerDto> delta, string systemUserId)
        {
            var customerEntity = await Uow.CustomerRepository.GetByIdAsync(customerId);
            if (customerEntity == null) throw new NotFoundException($"Customer with id {customerId} not found.");
            var entityDelta = CustomerMappingService.Map(delta);
            entityDelta.Apply(customerEntity);

            var customer = CustomerMappingService.Map(customerEntity);
            if (CustomerValidationService.Validate(customer) is var errors && errors.Any()) 
                throw new ValidationException("Invalid customer model.", errors);
            
            Uow.CustomerRepository.Update(entityDelta, customerEntity, systemUserId);
            await Uow.SaveAsync();
            
            return customer;
        }
 
        public async Task DeleteCustomerAsync(string customerId, string systemUserId)
        {
            var customerEntity = await Uow.CustomerRepository.GetByIdAsync(customerId);
            if (customerEntity == null) throw new NotFoundException($"Customer with id {customerId} not found.");

            //addresses are configured to be removed as cascade on delete. otherwise - include addresses in get by id and explicitly remove here.
            Uow.CustomerRepository.Delete(customerEntity);
            await Uow.SaveAsync();
        }
    }
}