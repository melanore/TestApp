using System.Collections.Generic;
using System.Threading.Tasks;
using TestApp.Core.Helpers;
using TestApp.Data.Domain;
using TestApp.Data.Entities;

namespace TestApp.Data.Repositories
{
    public interface IAddressRepository
    {
        Task<bool> ExistsByCustomerIdAsync<T>(string customerId) where T : Address, new();
        Task<(List<Address> results, int totalCount)> Get(AddressQuery query);
        Task<T> GetByCustomerIdAsync<T>(string customerId) where T : Address, new();
        Address Create(Delta<Address> delta, string systemUserId);
        Address Update(Delta<Address> delta, Address entity, string systemUserId);
        void Delete(Address entity);
    }
}