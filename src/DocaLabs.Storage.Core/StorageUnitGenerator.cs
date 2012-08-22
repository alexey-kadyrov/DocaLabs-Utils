using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using DocaLabs.Utils;

namespace DocaLabs.Storage.Core
{
    /// <summary>
    /// Generates type which implements specified unit of work interface.
    /// </summary>
    /// <example>
    /// For the interface like bellow:
    /// public interface ICustomerUnit : IDisposable
    /// {
    ///     ISessionContext Context { get; }
    ///     IStorageSet(Customer) Customers { get; }
    ///     IStorageSet(CustomerCategory) Categories { get; }
    ///     void Refresh(object obj);
    ///     void SaveChanges();
    /// }
    /// 
    /// The class like this will be generated:
    /// public class __UNIT_of_ICustomerUnit : ICustomerUnit
    /// {
    ///     SessionContext _context;
    ///     public ISessionContext Context { get { return _context; } }
    ///     public IStorageSet(Customer> Customers) { get; private set; }
    ///     public IStorageSet(CustomerCategory) Categories { get; private set; }
    ///     public void Refresh(object obj)
    ///     {
    ///         _context.Refresh(obj);
    ///     }
    ///     public void SaveChanges()
    ///     {
    ///         _context.SaveChanges();
    ///     }
    ///     public void Dispose()
    ///     {
    ///         _context.Dispose();
    ///     }
    ///     public __UNIT_of_ICustomerUnit(ISessionContext context, Func(Type, object) getStorageSet)
    ///     {
    ///         _context = context;
    ///         Customers = (IStorageSet(Customer))getStorageSet(typeof(Customer));
    ///         Categories = (IStorageSet(CustomerCategories))getStorageSet(typeof(CustomerCategories));
    ///     }
    /// }
    /// </example>
    public static class StorageUnitGenerator
    {
        static ConcurrentDictionary<Type, Type> ImplementedInterfaces { get; set; }

        static StorageUnitGenerator()
        {
            ImplementedInterfaces = new ConcurrentDictionary<Type, Type>();
        }

        public static Type GetImplementation<TInterface, TContext>()
        {
            return GetImplementation(typeof (TInterface), typeof (TContext));
        }

        public static Type GetImplementation(Type interfaceType, Type contextType)
        {
            return ImplementedInterfaces.GetOrAdd(interfaceType, key => new TypeBuilder(key, interfaceType).Build());
        }

        class TypeBuilder
        {
            Type InterfaceType { get; set; }
            Type ContextType { get; set; }

            string InterfaceName { get; set; }
            string ClassName { get; set; }

            public TypeBuilder(Type interfaceType, Type contextType)
            {
                InterfaceType = interfaceType;
                ContextType = contextType;

                InterfaceName = InterfaceType.FullName;
                ClassName = "__UNIT_of_" + InterfaceName.Replace('.', '_');
            }

            public Type Build()
            {
                var compiler = new CompileHelper();

                return compiler.Compile(GenerateCode()).GetType(ClassName);
            }

            string GenerateCode()
            {
                var sourceCode = new StringBuilder(8196);

                GenerateBeginClassDeclaration(sourceCode);

                foreach (var property in InterfaceType.GetAllProperties(BindingFlags.Public | BindingFlags.Instance))
                    GenerateProperty(property, sourceCode);

                foreach (var method in InterfaceType.GetAllMethods(BindingFlags.Public | BindingFlags.Instance))
                    GenerateMethod(method, sourceCode);

                GenerateConstructor(sourceCode);

                GenerateEndClassDeclaration(sourceCode);

                return sourceCode.ToString();
            }

            void GenerateBeginClassDeclaration(StringBuilder sourceCode)
            {
                sourceCode
                    .Append("public class ")
                    .Append(ClassName)
                    .Append(" : ")
                    .AppendLine(InterfaceName)
                    .AppendLine("{");
            }

            static void GenerateEndClassDeclaration(StringBuilder sourceCode)
            {
                sourceCode.AppendLine("}");
            }

            void GenerateProperty(PropertyInfo property, StringBuilder sourceCode)
            {
            }

            void GenerateMethod(MethodInfo method, StringBuilder sourceCode)
            {
            }

            void GenerateConstructor(StringBuilder sourceCode)
            {
            }

        }
    }
}
