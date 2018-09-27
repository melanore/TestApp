using System.Collections.Generic;
using System.Threading.Tasks;
using TestApp.Core.Helpers;
using TestApp.Data.Domain;
using TestApp.Data.Entities;

namespace TestApp.Data.Repositories
{
    public interface ICustomerRepository
    {
        Task<(List<Customer> results, int totalCount)> Get(CustomerQuery query);
        Task<Customer> GetByIdAsync(string id, bool includeAddresses = false);
        Customer Create(Delta<Customer> delta, string systemUserId);
        Customer Update(Delta<Customer> delta, Customer entity, string systemUserId);
        void Delete(Customer entity);
    }
}