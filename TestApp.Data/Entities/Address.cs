using System;
using TestApp.Data.Domain;

namespace TestApp.Data.Entities
{
    public class Address : IEntity
    {
        public string Name { get; set; }
        public string Street { get; set; }
        public string Zip { get; set; }
        public string City { get; set; }
        public string Country { get; set; }

        public string AddressType { get; protected set; }

        public string CustomerId { get; set; }
        public Customer Customer { get; set; }
        public DateTime CreatedDateTime { get; set; }
        public string CreatedBy { get; set; }
        public DateTime UpdatedDateTime { get; set; }
        public string UpdatedBy { get; set; }
        public int Version { get; set; }
    }

    public class InvoiceAddress : Address
    {
        public const string TYPE = "I";

        public InvoiceAddress()
        {
            AddressType = TYPE;
        }
    }

    public class DeliveryAddress : Address
    {
        public const string TYPE = "D";

        public DeliveryAddress()
        {
            AddressType = TYPE;
        }
    }

    public class ServiceAddress : Address
    {
        public const string TYPE = "S";

        public ServiceAddress()
        {
            AddressType = TYPE;
        }
    }
}