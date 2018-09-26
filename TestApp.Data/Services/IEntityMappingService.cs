using TestApp.Core.Helpers;
using TestApp.Data.Domain;

namespace TestApp.Data.Services
{
    public interface IEntityMappingService<TSeed, TEntity> where TSeed: IDomainObject<TEntity>, new() where TEntity : IEntity, new()
    {
        Delta<TEntity> Map(Delta<TSeed> seed);
        TSeed Map(TEntity entity);
    }
}