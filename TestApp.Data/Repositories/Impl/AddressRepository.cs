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
    public class AddressRepository : IAddressRepository
    {
        public AddressRepository(TestAppDbContext dbContext)
        {
            DbContext = dbContext;
        }

        private TestAppDbContext DbContext { get; }

        public async Task<bool> ExistsByCustomerIdAsync<T>(string customerId) where T : Address, new()
        {
            return await DbContext.Addresses.AsNoTracking().OfType<T>().AnyAsync(s => s.CustomerId == customerId);
        }
        
        public async Task<T> GetByCustomerIdAsync<T>(string customerId) where T : Address, new()
        {
            return await DbContext.Addresses.AsNoTracking().OfType<T>().SingleOrDefaultAsync(s => s.CustomerId == customerId);
        }

        public async Task<(List<Address> results, int totalCount)> Get(AddressQuery query)
        {
            if (query == null) query = new AddressQuery();
            var addressQuery = DbContext.Addresses.AsNoTracking();

            var totalCount = addressQuery.Count();

            addressQuery = addressQuery.Skip(query.PageSize * query.PageIndex).Take(query.PageSize);
      
            if (!string.IsNullOrEmpty(query.CustomerId)) addressQuery = addressQuery.Where(s => s.CustomerId == query.CustomerId);
            addressQuery = OrderQueryByOrderKeyAndDirection(addressQuery, query);

            var results = await addressQuery.ToListAsync();
            return (results, totalCount);
        }

        public Address Create(Delta<Address> delta, string systemUserId)
        {
            delta.AddFilter(s => s.Customer);
            var address = delta.ToObject();
            address.FillStandardFieldsOnCreation(systemUserId);
            DbContext.Addresses.Add(address);
            return address;
        }

        public Address Update(Delta<Address> delta, Address entity, string systemUserId)
        {
            delta.AddFilter(s => s.Customer);
            delta.AddFilter(s => s.CustomerId);
            if (!delta.GetState(entity).HasValue) return entity;
            delta.Apply(entity);
            entity.FillStandardFieldsOnUpdating(systemUserId);
            DbContext.Addresses.Update(entity);
            return entity;
        }

        public void Delete(Address entity)
        {
            DbContext.Addresses.Remove(entity);
        }

        private static IQueryable<Address> OrderQueryByOrderKeyAndDirection(IQueryable<Address> addressQuery, AddressQuery query)
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