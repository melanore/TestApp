using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TestApp.Core.Helpers;
using TestApp.Data.Domain;
using TestApp.Data.Entities;
using TestApp.Data.Helpers;

namespace TestApp.Data.Repositories.Impl
{
    public class CustomerRepository : ICustomerRepository
    {
        public CustomerRepository(TestAppDbContext dbContext)
        {
            DbContext = dbContext;
        }

        private TestAppDbContext DbContext { get; }

        public async Task<Customer> GetByIdAsync(string id, bool includeAddresses = false)
        {
            var query = DbContext.Customers.AsNoTracking();
            if (includeAddresses) query = query.Include(s => s.Addresses);
            return await query.SingleOrDefaultAsync(s => s.Id == id);
        }

        public async Task<(List<Customer> results, int totalCount)> Get(CustomerQuery query)
        {
            if (query == null) query = new CustomerQuery();
            var customersQuery = DbContext.Customers.AsNoTracking();

            var totalCount = customersQuery.Count();

            customersQuery = customersQuery.Skip(query.PageSize * query.PageIndex).Take(query.PageSize);
            customersQuery = OrderQueryByOrderKeyAndDirection(customersQuery, query);
            var results = await customersQuery.ToListAsync();
            return (results, totalCount);
        }

        public Customer Create(Delta<Customer> delta, string systemUserId)
        {
            delta.AddFilter(s => s.Id);
            delta.AddFilter(s => s.Addresses);

            var customer = delta.ToObject();
            customer.FillStandardFieldsOnCreation(systemUserId);
            DbContext.Customers.Add(customer);
            return customer;
        }

        public Customer Update(Delta<Customer> delta, Customer customer, string systemUserId)
        {
            delta.AddFilter(s => s.Id);
            delta.AddFilter(s => s.Addresses);

            if (!delta.GetState(customer).HasValue) return customer;
            delta.Apply(customer);
            customer.FillStandardFieldsOnUpdating(systemUserId);
            DbContext.Customers.Update(customer);
            return customer;
        }

        public void Delete(Customer entity)
        {
            DbContext.Customers.Remove(entity);
        }

        private static IQueryable<Customer> OrderQueryByOrderKeyAndDirection(IQueryable<Customer> customersQuery, CustomerQuery query)
        {
            switch (query.SortBy)
            {
                case CustomerOrderKey.CreatedDateTime when query.SortOrder == SortOrder.Ascending:
                    return customersQuery.OrderBy(s => s.CreatedDateTime);
                case CustomerOrderKey.CreatedDateTime when query.SortOrder == SortOrder.Descending:
                    return customersQuery.OrderByDescending(s => s.CreatedDateTime);

                case CustomerOrderKey.UpdatedDateTime when query.SortOrder == SortOrder.Ascending:
                    return customersQuery.OrderBy(s => s.UpdatedDateTime);
                case CustomerOrderKey.UpdatedDateTime when query.SortOrder == SortOrder.Descending:
                    return customersQuery.OrderByDescending(s => s.UpdatedDateTime);

                case CustomerOrderKey.Name when query.SortOrder == SortOrder.Ascending:
                    return customersQuery.OrderBy(s => s.Name);
                case CustomerOrderKey.Name when query.SortOrder == SortOrder.Descending:
                    return customersQuery.OrderByDescending(s => s.Name);

                default: return customersQuery;
            }
        }
    }
}