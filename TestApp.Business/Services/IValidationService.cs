using System.Collections.Generic;
using TestApp.Data.Domain;

namespace TestApp.Business.Services
{
    public interface IValidationService<in TDomain, TEntity> where TDomain: IDomainObject<TEntity> where TEntity : IEntity
    {
        List<string> Validate(TDomain record);
    }
}