using System.Threading.Tasks;
using TestApp.Core.Helpers;
using TestApp.Data.Entities;

namespace TestApp.Data.Services
{
    public interface ICustomerRepository
    {
        Task<PagedResource<Customer>> Get<T>(T query) where T : ResourceQuery, new();
        Task<Customer> GetByIdAsync(string id);
        void Create(Delta<Customer> delta, string systemUserId);
        void Update(Delta<Customer> delta, Customer entity, string systemUserId);
        void Delete(Customer entity);
    }
}