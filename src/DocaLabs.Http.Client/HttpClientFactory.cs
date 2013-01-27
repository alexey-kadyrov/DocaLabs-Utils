using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

namespace DocaLabs.Http.Client
{
    /// <summary>
    /// Generates an implementation of HttpClient for a given interface.
    /// </summary>
    public static class HttpClientFactory
    {
        const string Suffix = "__http_client_impl";
        static readonly ModuleBuilder ModuleBuilder;
        static readonly object Locker;
        static readonly Dictionary<Type, ConstructorInfo> Constructors;

        static HttpClientFactory()
        {
            var assemblyName = typeof(HttpClientFactory).Namespace + Suffix;

            var assemblyBuilder = AppDomain.CurrentDomain.DefineDynamicAssembly(
                new AssemblyName(assemblyName),
                AssemblyBuilderAccess.Run);

            ModuleBuilder = assemblyBuilder.DefineDynamicModule(assemblyName);
            Constructors = new Dictionary<Type, ConstructorInfo>();
            Locker = new object();
        }

        public static TInterface CreateInstance<TInterface>(Uri serviceUrl = null, string configurationName = null)
        {
            return (TInterface)CreateInstance(typeof(TInterface), serviceUrl, configurationName);
        }

        public static object CreateInstance(Type interfaceType, Uri serviceUrl = null, string configurationName = null)
        {
            return CreateInstance(null, interfaceType, serviceUrl, configurationName);
        }

        public static object CreateInstance(Type baseType, Type interfaceType, Uri serviceUrl = null, string configurationName = null)
        {
            var constructor = GetMappedConstructor(baseType, interfaceType);

            if (string.IsNullOrWhiteSpace(configurationName))
                configurationName = interfaceType.FullName;

            return constructor.Invoke(new object[] { serviceUrl, configurationName });
        }

        static ConstructorInfo GetMappedConstructor(Type baseType, Type interfaceType)
        {
            lock (Locker)
            {
                ConstructorInfo constructor;

                if (!Constructors.TryGetValue(interfaceType, out constructor))
                {
                    Constructors[interfaceType] = constructor = InitType(baseType, interfaceType);
                }

                return constructor;
            }
        }

        static ConstructorInfo InitType(Type baseType, Type interfaceType)
        {
            return CreateTypeFrom(baseType, interfaceType).GetConstructor(new[] { typeof(Uri), typeof(string) });
        }

        static Type CreateTypeFrom(Type baseType, Type interfaceType)
        {
            var interfaceInfo = new ClientInterfaceInfo(interfaceType);

            if(baseType == null)
                baseType = typeof(HttpClient<,>).MakeGenericType(interfaceInfo.QueryType, interfaceInfo.ResultType);

            var typeBuilder = ModuleBuilder.DefineType(
                interfaceType.FullName + Suffix, 
                TypeAttributes.Class | TypeAttributes.Public, 
                baseType, 
                new[] { interfaceType });

            DefineConstructor(baseType, interfaceInfo, typeBuilder);

            DefineServiceCallMethod(baseType, interfaceInfo, typeBuilder);

            return typeBuilder.CreateType();
        }

        static void DefineConstructor(Type baseType, ClientInterfaceInfo interfaceInfo, TypeBuilder typeBuilder)
        {
            var baseCtor = baseType.GetConstructor(
                new[] {typeof (Uri), typeof (string), interfaceInfo.RetryStragtegyType});

            if (baseCtor == null)
                throw new InvalidOperationException(string.Format(
                        "The base type {0} must have constructor with parameters: (Uri serviceUrl, string configurationName, {1} retryStrategy)",
                        baseType.FullName, interfaceInfo.RetryStragtegyType.FullName));

            var ctor = typeBuilder.DefineConstructor(
                MethodAttributes.Public, CallingConventions.Standard | CallingConventions.HasThis,
                new[] {typeof (Uri), typeof (string)});

            var ctorGenerator = ctor.GetILGenerator();

            ctorGenerator.Emit(OpCodes.Ldarg_0);
            ctorGenerator.Emit(OpCodes.Ldarg_1);
            ctorGenerator.Emit(OpCodes.Ldarg_2);
            ctorGenerator.Emit(OpCodes.Ldnull);
            ctorGenerator.Emit(OpCodes.Call, baseCtor);
            ctorGenerator.Emit(OpCodes.Ret);
        }

        static void DefineServiceCallMethod(Type baseType, ClientInterfaceInfo interfaceInfo, TypeBuilder typeBuilder)
        {
            var baseExecute = baseType.GetMethod("Execute", new[] { interfaceInfo.QueryType });
            if (baseExecute == null)
                throw new InvalidOperationException(string.Format("The base type {0} must have method: {1} Execute({2} query)",
                                                                  baseType.FullName, interfaceInfo.ResultType.FullName,
                                                                  interfaceInfo.QueryType.FullName));

            var newExecute = typeBuilder.DefineMethod(interfaceInfo.ServiceExecuteMethod.Name,
                                MethodAttributes.Public | MethodAttributes.Virtual | MethodAttributes.HideBySig,
                                CallingConventions.Standard | CallingConventions.HasThis,
                                interfaceInfo.ResultType, new[] { interfaceInfo.QueryType });

            var executeGenerator = newExecute.GetILGenerator();

            executeGenerator.Emit(OpCodes.Ldarg_0);
            executeGenerator.Emit(OpCodes.Ldarg_1);
            executeGenerator.Emit(OpCodes.Call, baseExecute);
            executeGenerator.Emit(OpCodes.Ret);

            typeBuilder.DefineMethodOverride(newExecute, interfaceInfo.ServiceExecuteMethod);
        }

        class ClientInterfaceInfo
        {
            public MethodInfo ServiceExecuteMethod { get; private set; }
            public Type QueryType { get; private set; }
            public Type ResultType { get; private set; }
            public Type RetryStragtegyType { get; private set; }

            public ClientInterfaceInfo(Type interfaceType)
            {
                if (interfaceType == null)
                    throw new ArgumentNullException("interfaceType");

                if (!interfaceType.IsInterface)
                    throw new ArgumentException(string.Format("The type {0} must be interface.", interfaceType.FullName), "interfaceType");

                var methods = interfaceType.GetMethods();
                if (methods.Length != 1)
                    throw new ArgumentException(string.Format("The type {0} must have only one method.", interfaceType.FullName), "interfaceType");

                ServiceExecuteMethod = methods[0];
                ResultType = ServiceExecuteMethod.ReturnType;

                if (ResultType == typeof(void))
                    throw new ArgumentException(string.Format("The return type of {0} method in {1} must not be void.", ServiceExecuteMethod.Name, interfaceType.FullName), "interfaceType");

                var parameters = ServiceExecuteMethod.GetParameters();
                if (parameters.Length != 1)
                    throw new ArgumentException(string.Format("The method {0} in {1} must have only one parameter.", ServiceExecuteMethod.Name, interfaceType.FullName), "interfaceType");

                QueryType = parameters[0].ParameterType;

                //typeof(Func<Func<Result>, Result>)
                RetryStragtegyType = typeof(Func<,>).MakeGenericType(typeof(Func<>).MakeGenericType(ResultType), ResultType);
            }
        }
    }
}
