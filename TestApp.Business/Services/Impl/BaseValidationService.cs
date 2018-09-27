using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using TestApp.Business.Const;
using TestApp.Data.Domain;

namespace TestApp.Business.Services.Impl
{
    public abstract class BaseValidationService<TDomain, TEntity> : IValidationService<TDomain, TEntity> 
        where TDomain: IDomainObject<TEntity> where TEntity : IEntity
    {
        // ReSharper disable once StaticMemberInGenericType // ok in this case.
        protected static readonly ImmutableHashSet<string> CountryCodeLookup = 
            ISO3166.Country.List.Select(s => s.TwoLetterCode).ToImmutableHashSet(StringComparer.InvariantCultureIgnoreCase);

        public abstract Func<TDomain, (bool isSuccessfull, string errorMessage)>[] GetValidationPipeline();
        
        public List<string> Validate(TDomain record) => GetValidationPipeline().Select(s => s.Invoke(record)).Where(s => !s.isSuccessfull).Select(s => s.errorMessage).ToList();

        protected static (bool isSuccessfull, string errorMessage) ValidateLengthAndRequired(string propertyName, string propertyValue, int maxLength, bool isRequired = false)
        {
            switch (propertyValue?.Trim().Length)
            {
                case 0 when isRequired:
                case null when isRequired: 
                    return (false, string.Format(ValidationErrors.FIELD_IS_REQUIRED, propertyName));
                case int n when n > maxLength: 
                    return (false, string.Format(ValidationErrors.TOO_LONG, propertyName, maxLength));
                default: return (true, default);
            }
        }
    }
}