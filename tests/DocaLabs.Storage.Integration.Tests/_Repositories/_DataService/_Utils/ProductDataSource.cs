using System;
using System.Collections.Generic;
using System.Data.Services;
using System.Linq;

namespace DocaLabs.Storage.Integration.Tests._Repositories._DataService._Utils
{
    /// <remarks>
    /// For recommendations how to implement IUpdatable see:
    /// http://archive.msdn.microsoft.com/IUpdateableLinqToSql/Thread/List.aspx
    /// http://archive.msdn.microsoft.com/SubSonicForADONETDS/Release/ProjectReleases.aspx?ReleaseId=1899
    /// http://wildermuth.com/2008/8/3/Implementing_IUpdatable_(Part_3)
    /// </remarks>>
    public class ProductDataSource : IUpdatable
    {
        public static List<Product> UnderlyingProductCollection { get; private set; }

        static ProductDataSource()
        {
            UnderlyingProductCollection = new List<Product>();
        }

        public IQueryable<Product> Products { get { return UnderlyingProductCollection.AsQueryable(); } }

        public static void AddSource(params Product[] products)
        {
            UnderlyingProductCollection.AddRange(products);
        }

        /// <summary>
        /// Creates the resource of the specified type and that belongs to the specified container.
        /// </summary>
        /// <returns>
        /// The object representing a resource of specified type and belonging to the specified container.
        /// </returns>
        /// <param name="containerName">The name of the entity set to which the resource belongs.</param><param name="fullTypeName">The full namespace-qualified type name of the resource.</param>
        public object CreateResource(string containerName, string fullTypeName)
        {
            if (Type.GetType(fullTypeName, true) != typeof(Product))
                throw new InvalidOperationException("Unexpected type for resource.");

            var resource = new Product();

            UnderlyingProductCollection.Add(resource);

            return resource;
        }

        /// <summary>
        /// Gets the resource of the specified type identified by a query and type name. 
        /// </summary>
        /// <returns>
        /// An opaque object representing a resource of the specified type, referenced by the specified query.
        /// </returns>
        /// <param name="query">Language integrated query (LINQ) pointing to a particular resource.</param><param name="fullTypeName">The fully qualified type name of resource.</param>
        public object GetResource(IQueryable query, string fullTypeName)
        {
            var resource = query.Cast<object>().SingleOrDefault();

            // fullTypeName can be null for deletes
            if (fullTypeName != null && (resource == null || resource.GetType().FullName != fullTypeName))
                throw new InvalidOperationException("Unexpected type for resource.");

            return resource;
        }

        /// <summary>
        /// Resets the resource identified by the parameter <paramref name="resource "/>to its default value.
        /// </summary>
        /// <returns>
        /// The resource with its value reset to the default value.
        /// </returns>
        /// <param name="resource">The resource to be updated.</param>
        public object ResetResource(object resource)
        {
            var product = resource as Product;

            if (product != null)
            {
                product.Name = default(string);
                product.Price = default(decimal);
                product.Rating = default(int);
                product.ReleaseDate = default(DateTime);
            }

            return resource;
        }

        /// <summary>
        /// Sets the value of the property with the specified name on the target resource to the specified property value.
        /// </summary>
        /// <param name="targetResource">The target object that defines the property.</param><param name="propertyName">The name of the property whose value needs to be updated.</param><param name="propertyValue">The property value for update.</param>
        public void SetValue(object targetResource, string propertyName, object propertyValue)
        {
            var info = targetResource.GetType().GetProperty(propertyName);
            if (info == null)
                throw new InvalidOperationException("Can't find property.");

            info.SetValue(targetResource, propertyValue, null);
        }

        /// <summary>
        /// Gets the value of the specified property on the target object.
        /// </summary>
        /// <returns>
        /// The value of the object.
        /// </returns>
        /// <param name="targetResource">An opaque object that represents a resource.</param><param name="propertyName">The name of the property whose value needs to be retrieved.</param>
        public object GetValue(object targetResource, string propertyName)
        {
            var info = targetResource.GetType().GetProperty(propertyName);
            if (info == null)
                throw new InvalidOperationException("Can't find property.");

            return info.GetValue(targetResource, null);
        }

        /// <summary>
        /// Sets the value of the specified reference property on the target object.
        /// </summary>
        /// <param name="targetResource">The target object that defines the property.</param><param name="propertyName">The name of the property whose value needs to be updated.</param><param name="propertyValue">The property value to be updated.</param>
        public void SetReference(object targetResource, string propertyName, object propertyValue)
        {
            throw new NotSupportedException("The provider doesn't support relationships.");
        }

        /// <summary>
        /// Adds the specified value to the collection.
        /// </summary>
        /// <param name="targetResource">Target object that defines the property.</param><param name="propertyName">The name of the collection property to which the resource should be added..</param><param name="resourceToBeAdded">The opaque object representing the resource to be added.</param>
        public void AddReferenceToCollection(object targetResource, string propertyName, object resourceToBeAdded)
        {
            throw new NotSupportedException("The provider doesn't support relationships.");
        }

        /// <summary>
        /// Removes the specified value from the collection.
        /// </summary>
        /// <param name="targetResource">The target object that defines the property.</param><param name="propertyName">The name of the property whose value needs to be updated.</param><param name="resourceToBeRemoved">The property value that needs to be removed.</param>
        public void RemoveReferenceFromCollection(object targetResource, string propertyName, object resourceToBeRemoved)
        {
            throw new NotSupportedException("The provider doesn't support relationships.");
        }

        /// <summary>
        /// Deletes the specified resource.
        /// </summary>
        /// <param name="targetResource">The resource to be deleted.</param>
        public void DeleteResource(object targetResource)
        {
            UnderlyingProductCollection.RemoveAll(x => x.Id == ((Product)targetResource).Id);
        }

        /// <summary>
        /// Saves all the changes that have been made by using the <see cref="T:System.Data.Services.IUpdatable"/> APIs.
        /// </summary>
        public void SaveChanges()
        {
            // for simplicity all changes are applied immediately
        }

        /// <summary>
        /// Returns the instance of the resource represented by the specified resource object.
        /// </summary>
        /// <returns>
        /// Returns the instance of the resource represented by the specified resource object.
        /// </returns>
        /// <param name="resource">The object representing the resource whose instance needs to be retrieved.</param>
        public object ResolveResource(object resource)
        {
            return resource;
        }

        /// <summary>
        /// Cancels a change to the data.
        /// </summary>
        public void ClearChanges()
        {
            throw new NotSupportedException("The provider doesn't support ClearChanges.");
        }
    }
}
