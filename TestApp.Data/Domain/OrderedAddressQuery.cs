using TestApp.Core.Helpers;

namespace TestApp.Data.Domain
{
    public class OrderedAddressQuery : OrderedResourceQuery<AddressOrderKey>
    {
        public override AddressOrderKey? SortBy { get; set; }
    }

    public enum AddressOrderKey
    {
        CreatedDateTime,
        UpdatedDateTime,
        AddressType
    }
}