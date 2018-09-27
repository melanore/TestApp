using System.Collections.Generic;
using System.Threading.Tasks;
using TestApp.Business.Domain;
using TestApp.Core.Helpers;
using TestApp.Data.Domain;

namespace TestApp.Business.Services
{
    public interface IAddressService
    {
        Task<PagedResource<Address>> GetAddressesAsync(AddressQuery query, string systemUserId);
        Task<List<Address>> GetCustomerAddressesAsync(string customerId, string systemUserId);
        Task<Address> GetCustomerAddressAsync(string customerId, AddressType addressType, string systemUserId);
        Task<Address> CreateAddressAsync(string customerId, Delta<Address> delta, string systemUserId);
        Task<Address> UpdateAddressAsync(string customerId, Delta<Address> delta, string systemUserId);
        Task DeleteAddressAsync(string customerId, AddressType type, string systemUserId);
    }
}