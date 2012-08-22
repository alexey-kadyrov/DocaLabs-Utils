using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data.Services.Common;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using DocaLabs.Utils.Conversion;

namespace DocaLabs.Storage.Core.Utils
{
    /// <summary>
    /// Defines method that let to find an entity in the IQueryable source by primary key(s).
    /// </summary>
    public static class QueryableEntityFinder
    {
        static ConcurrentDictionary<Type, ParsedTypeInformation> TypeCache { get; set; }
        static volatile CustomConverter _converter;

        /// <summary>
        /// Gets or sets custom converter which is used to convert input keys to values that are defined on the entity.
        /// Setting the property to null will force to return the CustomConverter.Current next time the getter is called.
        /// The getter will never return null value.
        /// </summary>
        public static CustomConverter Converter
        {
            get { return _converter ?? CustomConverter.Current; }
            set { _converter = value; }
        }

        static QueryableEntityFinder()
        {
            TypeCache = new ConcurrentDictionary<Type, ParsedTypeInformation>();
        }

        /// <summary>
        /// Finds an entity in the IQueryable source by primary key(s).
        /// </summary>
        /// <typeparam name="T">Entity type.</typeparam>
        /// <param name="source">The source IQueryable collection.</param>
        /// <param name="keyValues">Keys to be found in the collection.</param>
        /// <returns>Found object or null.</returns>
        public static T FindByKeys<T>(this IQueryable source, params object[] keyValues) where T : class
        {
            return FindByKeys(source, typeof(T), keyValues) as T;
        }

        /// <summary>
        /// Finds an entity in the IQueryable source by primary key(s).
        /// </summary>
        /// <remarks>
        /// The order of checking whenever an entity's property is key:
        ///     1. Properties that are specified in DataServiceKeyAttribute.
        ///     2. Property that's marked by KeyAttribute. As the attribute cannot specify order
        ///        and that reflection doesn't guaranty the order there must be only one marked property.
        ///     3. Property with type name + 'Id'.
        ///     4. Property with name 'Id'.
        /// The reflection on a type happens only once and the result is cached.
        /// </remarks>
        /// <param name="entityType">Entity type.</param>
        /// <param name="source">The source IQueryable collection.</param>
        /// <param name="keyValues">Keys to be found in the collection.</param>
        /// <returns>Found object or null.</returns>
        public static object FindByKeys(this IQueryable source, Type entityType, params object[] keyValues)
        {
            if(source == null)
                throw new ArgumentNullException("source");

            if(entityType == null)
                throw new ArgumentNullException("entityType");

            if(keyValues == null || keyValues.Length <= 0)
                throw new ArgumentNullException("keyValues");

            return TypeCache.GetOrAdd(entityType, t => new ParsedTypeInformation(t)).Execute(source, keyValues);
        }

        class ParsedTypeInformation
        {
            IList<PropertyInfo> KeyProperties { get; set; }
            Type EntityType { get; set; }

            public ParsedTypeInformation(Type entityType)
            {
                EntityType = entityType;

                KeyProperties = GetKeyProperties();

                if (KeyProperties == null || KeyProperties.Count == 0)
                    throw new InvalidOperationException(string.Format(Resources.Text.no_key_defined_for_entity_0, EntityType));
            }

            /// <remarks>
            /// The method is using 'Where' as not all providers support 'SingleOrDefault'.
            /// </remarks>>
            public object Execute(IQueryable source, IList<object> values)
            {
                var enumerator = GetQueryEnumerator(source, values);

                if (enumerator.MoveNext())
                {
                    var o = enumerator.Current;

                    if (enumerator.MoveNext())
                        throw new InvalidOperationException(Resources.Text.get_unique_returned_more_than_one_entity);

                    return o;
                }

                return null;
            }

            IEnumerator GetQueryEnumerator(IQueryable source, IList<object> values)
            {
                var query = source.Provider.CreateQuery(Expression.Call(typeof(Queryable), "Where", new[] { source.ElementType }, new[]
                {
                    source.Expression,
                    Expression.Quote(MakeGetUniquePredicate(values))
                }));

                return query.GetEnumerator();
            }

            IList<PropertyInfo> GetKeyProperties()
            {
                return TryDataServiceKeyAttribute() 
                    ?? TryKeyAttribute() 
                    ?? TryGetPropertyByName(EntityType.Name + "Id") 
                    ?? TryGetPropertyByName("Id");
            }

            IList<PropertyInfo> TryDataServiceKeyAttribute()
            {
                var attributes = EntityType.GetCustomAttributes(typeof(DataServiceKeyAttribute), true);

                if (attributes.Length == 0)
                    return null;

                var keys = new List<PropertyInfo>();

                foreach (var keyName in ((DataServiceKeyAttribute)attributes[0]).KeyNames)
                {
                    var property = TryGetPropertyByName(keyName);
                    if (property == null)
                        throw new InvalidOperationException(string.Format(Resources.Text.no_matching_key_property_0_for_entity_1, keyName, EntityType));

                    keys.AddRange(property);
                }

                return keys.Count > 0 ? keys : null;
            }

            IList<PropertyInfo> TryKeyAttribute()
            {
                var properties = EntityType.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.FlattenHierarchy)
                    .Where(f => f.GetCustomAttributes(typeof(System.ComponentModel.DataAnnotations.KeyAttribute), true).Any())
                    .ToArray();

                if (properties.Length > 1)
                    throw new InvalidOperationException(string.Format(Resources.Text.more_than_one_property_marked_as_primary_key_in_0, EntityType));

                return properties.Length > 0 ? properties : null;
            }

            IList<PropertyInfo> TryGetPropertyByName(string propertyName)
            {
                var property = EntityType.GetProperty(propertyName, BindingFlags.IgnoreCase | BindingFlags.Instance |
                                                      BindingFlags.Public | BindingFlags.NonPublic |
                                                      BindingFlags.FlattenHierarchy);

                return property != null ? new[] { property } : null;
            }

            Expression MakeGetUniquePredicate(IList<object> values)
            {
                if (KeyProperties.Count != values.Count())
                    throw new InvalidOperationException(string.Format(Resources.Text.number_of_key_properties_doesn_match_values_for_entity_0, EntityType));

                var lambdaParameter = Expression.Parameter(EntityType, "x");

                var equalityExpressions = new LinkedList<Expression>();

                for (var i = 0; i < KeyProperties.Count; i++)
                {
                    var propertyInfo = KeyProperties[i];

                    equalityExpressions.AddLast(Expression.MakeBinary(ExpressionType.Equal,
                        Expression.Property(lambdaParameter, propertyInfo),
                        Expression.Constant(Converter.ChangeType(values[i], propertyInfo.PropertyType))));
                }

                return Expression.Lambda(MakeChain(equalityExpressions.First), new[] { lambdaParameter });
            }

            static Expression MakeChain(LinkedListNode<Expression> equalityNode)
            {
                if (equalityNode == null)
                    return null;

                var currentValue = equalityNode.Value;

                var innerExpression = MakeChain(equalityNode.Next);

                return innerExpression == null ? currentValue : Expression.AndAlso(currentValue, innerExpression);
            }
        }
    }
}
