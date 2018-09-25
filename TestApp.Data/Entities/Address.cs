namespace TestApp.Data.Entities
{
    public abstract class Address
    {
        public Address(string addressType) => AddressType = addressType;
        
        public string Name { get; set; }
        public string Street { get; set; }
        public string Zip { get; set; }
        public string City { get; set; }
        public string Country { get; set; }

        public string AddressType { get; set; }

        public string CustomerId { get; set; }
        public Customer Customer { get; set; }
    }

    public class InvoiceAddress : Address
    {
        public InvoiceAddress() : base("I") { }
    }
    
    public class DeliveryAddress : Address
    {
        public DeliveryAddress() : base("D") { }
    }
    
    public class ServiceAddress : Address
    {
        public ServiceAddress() : base("S") { }
    }
}