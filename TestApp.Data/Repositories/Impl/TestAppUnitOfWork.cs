using System.Threading.Tasks;

namespace TestApp.Data.Repositories.Impl
{
    public class TestAppUnitOfWork : ITestAppUnitOfWork
    {
        public TestAppUnitOfWork(TestAppDbContext dbContext, IAddressRepository addressRepository, ICustomerRepository customerRepository)
        {
            DbContext = dbContext;
            AddressRepository = addressRepository;
            CustomerRepository = customerRepository;
        }
        
        public IAddressRepository AddressRepository { get; }
        public ICustomerRepository CustomerRepository { get; }
        
        private TestAppDbContext DbContext { get; }

        public async Task SaveAsync() => await DbContext.SaveChangesAsync();
    }
}