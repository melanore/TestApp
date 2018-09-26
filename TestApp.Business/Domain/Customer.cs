using TestApp.Data.Domain;

namespace TestApp.Business.Domain
{
    public class Customer : IDomainObject<Data.Entities.Customer>
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Street { get; set; }
        public string Zip { get; set; }
        public string City { get; set; }
        public string Country { get; set; }

        public Address InvoiceAddress { get; set; }
        public Address DeliveryAddress { get; set; }
        public Address ServiceAddress { get; set; }
    }
}