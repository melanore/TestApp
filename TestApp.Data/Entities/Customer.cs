using System.Collections.Generic;

namespace TestApp.Data.Entities
{
    public class Customer
    {
        public string Id { get; private set; }
        
        public string Name { get; set; }
        public string Street { get; set; }
        public string Zip { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
        
        public List<Address> Addresses { get; set; }
    }
}