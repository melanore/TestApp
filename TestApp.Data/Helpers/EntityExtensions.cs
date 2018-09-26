using System;
using TestApp.Data.Domain;

namespace TestApp.Data.Helpers
{
    public static class EntityExtensions
    {
        public static void FillStandardFieldsOnCreation<T>(this T entity, string actorId) where T : IEntity
        {
            entity.CreatedBy = actorId;
            entity.CreatedDateTime = DateTime.UtcNow;
            FillStandardFieldsOnUpdating(entity, actorId);
        }

        public static void FillStandardFieldsOnUpdating<T>(this T entity, string actorId) where T : IEntity
        {
            entity.UpdatedBy = actorId;
            entity.UpdatedDateTime = DateTime.UtcNow;
            entity.Version++;
        }
    }
}