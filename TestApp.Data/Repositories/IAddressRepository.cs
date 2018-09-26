using System.Threading.Tasks;
using TestApp.Core.Helpers;
using TestApp.Data.Entities;

namespace TestApp.Data.Repositories
{
    public interface IAddressRepository
    {
        Task<PagedResource<Address>> Get<T>(T query) where T : ResourceQuery, new();
        Task<T> GetByCustomerIdAsync<T>(string customerId) where T : Address, new();
        void Create(Delta<Address> delta, string systemUserId);
        void Update(Delta<Address> delta, Address entity, string systemUserId);
        void Delete(Address entity);
    }
}