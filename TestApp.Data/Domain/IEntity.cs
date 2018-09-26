using System;

namespace TestApp.Data.Domain
{
    public interface IEntity
    {
        DateTime CreatedDateTime { get; set; }
        string CreatedBy { get; set; }

        DateTime UpdatedDateTime { get; set; }
        string UpdatedBy { get; set; }

        int Version { get; set; }
    }
}