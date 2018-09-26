using TestApp.Data.Domain;

namespace TestApp.Business.Domain
{
    public class Address : IDomainObject<Data.Entities.Address>
    {
        public string Name { get; set; }
        public string Street { get; set; }
        public string Zip { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
        public AddressType AddressType { get; set; }
    }

    public enum AddressType
    {
        Invoice = 0,
        Delivery = 1,
        Service = 2
    }
}