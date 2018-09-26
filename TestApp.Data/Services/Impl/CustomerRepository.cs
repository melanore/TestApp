using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TestApp.Core.Helpers;
using TestApp.Data.Domain;
using TestApp.Data.Entities;
using TestApp.Data.Helpers;

namespace TestApp.Data.Services.Impl
{
    public class CustomerRepository : ICustomerRepository
    {
        public CustomerRepository(TestAppDbContext dbContext)
        {
            DbContext = dbContext;
        }
        
        private TestAppDbContext DbContext { get; }
        
        public async Task<Customer> GetByIdAsync(string id) => await DbContext.Customers.AsNoTracking().SingleOrDefaultAsync(s => s.Id == id);

        public async Task<PagedResource<Customer>> Get<T>(T query) where T : ResourceQuery, new()
        {
            if (query == null) query = new T();
            var customersQuery = DbContext.Customers.AsNoTracking();
            
            var totalCount = customersQuery.Count();
            
            customersQuery = customersQuery.Skip(query.PageSize * query.PageIndex).Take(query.PageSize);
            if (query is OrderedCustomerQuery orderedQueryFilter) customersQuery = OrderQueryByOrderKeyAndDirection(customersQuery, orderedQueryFilter);
            var results = await customersQuery.ToListAsync();
            return new PagedResource<Customer>(results, totalCount, query);
        }
        
        public void Create(Delta<Customer> delta, string systemUserId)
        {
            delta.AddFilter(s => s.Id);
            delta.AddFilter(s => s.Addresses);
            
            var customer = delta.ToObject();
            customer.FillStandardFieldsOnCreation(systemUserId);
            DbContext.Customers.Add(customer);
        }

        public void Update(Delta<Customer> delta, Customer entity, string systemUserId)
        {
            delta.AddFilter(s => s.Id);
            delta.AddFilter(s => s.Addresses);
            
            if (!delta.GetState(entity).HasValue) return;
            delta.Apply(entity);
            entity.FillStandardFieldsOnUpdating(systemUserId);
            DbContext.Customers.Update(entity);
        }

        public void Delete(Customer entity)
        {
            //addresses are configured to be removed as cascade on delete. otherwise - explicit here.
            DbContext.Customers.Remove(entity);
        } 
        
        private static IQueryable<Customer> OrderQueryByOrderKeyAndDirection(IQueryable<Customer> customersQuery, OrderedCustomerQuery query)
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