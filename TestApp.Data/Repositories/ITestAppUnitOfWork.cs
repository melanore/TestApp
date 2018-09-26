using System.Threading.Tasks;

namespace TestApp.Data.Repositories
{
    public interface ITestAppUnitOfWork
    {
        IAddressRepository AddressRepository { get; }
        ICustomerRepository CustomerRepository { get; }
        Task SaveAsync();
    }
}