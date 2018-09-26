using System;
using System.Collections.Generic;
using TestApp.Data.Domain;

namespace TestApp.Data.Entities
{
    public class Customer : IEntity
    {
        public string Id { get; private set; }

        public string Name { get; set; }
        public string Street { get; set; }
        public string Zip { get; set; }
        public string City { get; set; }
        public string Country { get; set; }

        public List<Address> Addresses { get; set; }

        public DateTime CreatedDateTime { get; set; }
        public string CreatedBy { get; set; }
        public DateTime UpdatedDateTime { get; set; }
        public string UpdatedBy { get; set; }
        public int Version { get; set; }
    }
}