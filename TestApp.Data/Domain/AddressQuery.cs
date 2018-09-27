using TestApp.Core.Helpers;

namespace TestApp.Data.Domain
{
    public class AddressQuery : OrderedResourceQuery<AddressOrderKey>
    {
        public string CustomerId { get; set; }
        public override AddressOrderKey? SortBy { get; set; }
    }

    public enum AddressOrderKey
    {
        CreatedDateTime,
        UpdatedDateTime,
        AddressType
    }
}