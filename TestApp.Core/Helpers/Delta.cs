using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Newtonsoft.Json;

namespace TestApp.Core.Helpers
{
    public class Delta<T> where T : new()
    {
        private static readonly Dictionary<string, Action<object, object>> TypeSettersTemplate =
            (typeof(T).GetProperties() ?? Enumerable.Empty<PropertyInfo>())
            .ToDictionary<PropertyInfo, string, Action<object, object>>(property => property.Name, property => property.SetValue);

        private readonly Dictionary<string, Action<object, object>> currentTypeSetters =
            TypeSettersTemplate.ToDictionary(kvp => kvp.Key, kvp => kvp.Value, StringComparer.InvariantCultureIgnoreCase);

        [JsonExtensionData]
        public Dictionary<string, object> ObjectPropertyValues { get; } = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);

        public T ToObject()
        {
            var obj = new T();
            Apply(obj);
            return obj;
        }

        public void AddFilter<TProperty>(params Expression<Func<T, TProperty>>[] filters)
        {
            foreach (var filter in filters ?? Enumerable.Empty<Expression<Func<T, TProperty>>>())
                currentTypeSetters.Remove(GetPropertyInfoFromExpression(filter).Name);
        }

        public void SetValue<TProperty>(Expression<Func<T, TProperty>> property, object value)
        {
            var propertyInfo = GetPropertyInfoFromExpression(property);
            ObjectPropertyValues[propertyInfo.Name] = value;
        }

        public TProperty GetValue<TProperty>(Expression<Func<T, TProperty>> property)
        {
            var propertyInfo = GetPropertyInfoFromExpression(property);
            ObjectPropertyValues.TryGetValue(propertyInfo.Name, out var val);
            return (TProperty) Unwrap(propertyInfo, val);
        }

        public bool TryGetValue<TProperty>(Expression<Func<T, TProperty>> property, out TProperty value)
        {
            var propertyInfo = GetPropertyInfoFromExpression(property);
            var result = ObjectPropertyValues.TryGetValue(propertyInfo.Name, out var boxedValue);
            value = result ? (TProperty) Unwrap(propertyInfo, boxedValue) : default;
            return result;
        }

        private static PropertyInfo GetPropertyInfoFromExpression<TProperty>(Expression<Func<T, TProperty>> propertyExpression)
        {
            var me = propertyExpression.Body as MemberExpression;
            return
                !(me?.Member is PropertyInfo)
                    ? throw new NotSupportedException("Only simple property accessors are supported")
                    : (PropertyInfo) me.Member;
        }

        public void Apply(T inputObject)
        {
            const BindingFlags bindingFlags = BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance;

            if (ObjectPropertyValues == null) return;
            foreach (var objectPropertyNameValue in ObjectPropertyValues)
                if (currentTypeSetters.Keys.Select(k => k.ToLower()).Contains(objectPropertyNameValue.Key.ToLower()))
                {
                    var property = typeof(T).GetProperty(objectPropertyNameValue.Key, bindingFlags);
                    var value = Unwrap(property, objectPropertyNameValue.Value);
                    property.SetValue(inputObject, value);
                }
        }

        public State? GetState(T originalObject)
        {
            const BindingFlags bindingFlags = BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance;

            return ObjectPropertyValues?
                .Where(v => currentTypeSetters.Keys.Select(k => k.ToLower()).Contains(v.Key.ToLower()))
                .Select(v =>
                {
                    var property = typeof(T).GetProperty(v.Key, bindingFlags);

                    return new
                    {
                        Value = Unwrap(property, v.Value),
                        OriginalValue = property.GetValue(originalObject)
                    };
                })
                .Where(s => !Equals(s.OriginalValue, s.Value))
                .Select(s => new {s.OriginalValue, s.Value, DefaultValue = GetDefault(s.Value)})
                .Aggregate(default(State), (state, propertyData) => state | (
                                                                        Equals(propertyData.Value, propertyData.DefaultValue) || propertyData.Value == null
                                                                            ? State.HasDeletion
                                                                            : Equals(propertyData.OriginalValue, propertyData.DefaultValue) ||
                                                                              propertyData.OriginalValue == null
                                                                                ? State.HasAddition
                                                                                : State.HasUpdate));
        }

        private static object GetDefault(object obj)
        {
            var type = obj.GetType();
            if (typeof(Nullable<>) == type) type = Nullable.GetUnderlyingType(type);
            return type == typeof(string)
                ? string.Empty
                : typeof(Default<>).MakeGenericType(type).GetProperty(nameof(Default<object>.Instance)).GetValue(null);
        }

        private static object Unwrap(PropertyInfo propertyInfo, object boxedValue)
        {
            var isNullable = propertyInfo.PropertyType.IsGenericType && propertyInfo.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>);
            var unboxedType = isNullable
                ? Nullable.GetUnderlyingType(propertyInfo.PropertyType)
                : propertyInfo.PropertyType;

            if (unboxedType.IsEnum)
                switch (boxedValue)
                {
                    case null when isNullable: return null;
                    case string s: return Enum.Parse(unboxedType, s, true);
                    case var obj: return Enum.ToObject(unboxedType, obj);
                }

            return boxedValue;
        }
    }

    [Flags]
    public enum State
    {
        HasAddition = 1,
        HasDeletion = 2,
        HasUpdate = 4
    }

    internal class Default<TDefault>
    {
        // strongly typed reflection helper
        // ReSharper disable once UnusedMember.Local
        // ReSharper disable once UnusedAutoPropertyAccessor.Global
        public static TDefault Instance { get; } = default;
    }
}