using TestApp.Core.Helpers;

namespace TestApp.Data.Domain
{
    public class CustomerQuery : OrderedResourceQuery<CustomerOrderKey>
    {
        public override CustomerOrderKey? SortBy { get; set; }
    }

    public enum CustomerOrderKey
    {
        CreatedDateTime,
        UpdatedDateTime,
        Name
    }
}