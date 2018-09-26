using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TestApp.Core.Helpers;
using TestApp.Data.Domain;
using TestApp.Data.Entities;
using TestApp.Data.Helpers;

namespace TestApp.Data.Services.Impl
{
    public class AddressRepository : IAddressRepository
    {
        public AddressRepository(TestAppDbContext dbContext)
        {
            DbContext = dbContext;
        }

        private TestAppDbContext DbContext { get; }

        public async Task<T> GetByCustomerIdAsync<T>(string customerId) where T : Address, new()
        {
            return await DbContext.Addresses.AsNoTracking().OfType<T>().SingleOrDefaultAsync(s => s.CustomerId == customerId);
        }

        public async Task<PagedResource<Address>> Get<T>(T query) where T : ResourceQuery, new()
        {
            if (query == null) query = new T();
            var customersQuery = DbContext.Addresses.AsNoTracking();

            var totalCount = customersQuery.Count();

            customersQuery = customersQuery.Skip(query.PageSize * query.PageIndex).Take(query.PageSize);
            if (query is OrderedAddressQuery orderedQueryFilter) customersQuery = OrderQueryByOrderKeyAndDirection(customersQuery, orderedQueryFilter);
            var results = await customersQuery.ToListAsync();
            return new PagedResource<Address>(results, totalCount, query);
        }

        public void Create(Delta<Address> delta, string systemUserId)
        {
            delta.AddFilter(s => s.Customer);
            var customer = delta.ToObject();
            customer.FillStandardFieldsOnCreation(systemUserId);
            DbContext.Addresses.Add(customer);
        }

        public void Update(Delta<Address> delta, Address entity, string systemUserId)
        {
            delta.AddFilter(s => s.Customer);
            if (!delta.GetState(entity).HasValue) return;
            delta.Apply(entity);
            entity.FillStandardFieldsOnUpdating(systemUserId);
            DbContext.Addresses.Update(entity);
        }

        public void Delete(Address entity)
        {
            DbContext.Addresses.Remove(entity);
        }

        private static IQueryable<Address> OrderQueryByOrderKeyAndDirection(IQueryable<Address> addressQuery, OrderedAddressQuery query)
        {
            switch (query.SortBy)
            {
                case AddressOrderKey.CreatedDateTime when query.SortOrder == SortOrder.Ascending:
                    return addressQuery.OrderBy(s => s.CreatedDateTime);
                case AddressOrderKey.CreatedDateTime when query.SortOrder == SortOrder.Descending:
                    return addressQuery.OrderByDescending(s => s.CreatedDateTime);

                case AddressOrderKey.UpdatedDateTime when query.SortOrder == SortOrder.Ascending:
                    return addressQuery.OrderBy(s => s.UpdatedDateTime);
                case AddressOrderKey.UpdatedDateTime when query.SortOrder == SortOrder.Descending:
                    return addressQuery.OrderByDescending(s => s.UpdatedDateTime);

                case AddressOrderKey.AddressType when query.SortOrder == SortOrder.Ascending:
                    return addressQuery.OrderBy(s => s.AddressType);
                case AddressOrderKey.AddressType when query.SortOrder == SortOrder.Descending:
                    return addressQuery.OrderByDescending(s => s.AddressType);

                default: return addressQuery;
            }
        }
    }
}