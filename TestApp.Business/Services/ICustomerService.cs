using System.Threading.Tasks;
using TestApp.Business.Domain;
using TestApp.Core.Helpers;
using TestApp.Data.Domain;

namespace TestApp.Business.Services
{
    public interface ICustomerService
    {
        Task<PagedResource<Customer>> GetCustomersAsync(CustomerQuery query, string systemUserId);
        Task<Customer> GetCustomerAsync(string customerId, string systemUserId);
        Task<Customer> CreateCustomerAsync(Delta<Customer> delta, string systemUserId);
        Task<Customer> UpdateCustomerAsync(string customerId, Delta<Customer> delta, string systemUserId);
        Task DeleteCustomerAsync(string customerId, string systemUserId);
    }
}