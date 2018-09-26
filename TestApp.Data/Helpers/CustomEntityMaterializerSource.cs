using System;
using System.Linq.Expressions;
using System.Reflection;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using TestApp.Core.Util;

namespace TestApp.Data.Helpers
{
    // http://volosoft.com/datetime-specifykind-in-entity-framework-core-while-querying/
    internal class CustomEntityMaterializerSource : EntityMaterializerSource
    {
        private static readonly MethodInfo NormalizeMethod = typeof(DateTimeMapper).GetTypeInfo().GetMethod(nameof(DateTimeMapper.Normalize));

        public override Expression CreateReadValueExpression(Expression valueBuffer, Type type, int index, IPropertyBase property)
        {
            return type == typeof(DateTime)
                ? Expression.Call(NormalizeMethod, base.CreateReadValueExpression(valueBuffer, type, index, property))
                : base.CreateReadValueExpression(valueBuffer, type, index, property);
        }
    }
}