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

        /// <summary>
        /// Creates an instance of a concrete class implementing the TInterface, the class is derived from HttpClient.
        /// </summary>
        /// <typeparam name="TInterface">
        /// Interface which should be implemented, the interface must implement only one method with one parameter and non void return parameter.
        /// The method's parameter type will be used as the TQuery generic parameter and the return type as the TResult.
        /// The method can have any name. The method's implementation will call to TResult Execute(TQuery query) method of the HttpClient.
        /// </typeparam>
        /// <param name="serviceUrl">The URL of the service.</param>
        /// <param name="configurationName">If the configuration name is used to get the endpoint configuration from the config file, if the parameter is null it will default to the interface's type full name.</param>
        public static TInterface CreateInstance<TInterface>(Uri serviceUrl = null, string configurationName = null)
        {
            return (TInterface)CreateInstance(typeof(TInterface), serviceUrl, configurationName);
        }

        /// <summary>
        /// Creates an instance of a concrete class implementing the interfaceType, the class is derived from HttpClient.
        /// </summary>
        /// <param name="interfaceType">
        /// Interface which should be implemented, the interface must implement only one method with one parameter and non void return parameter.
        /// The method's parameter type will be used as the TQuery generic parameter and the return type as the TResult.
        /// The method can have any name. The method's implementation will call to TResult Execute(TQuery query) method of the HttpClient.
        /// </param>
        /// <param name="serviceUrl">The URL of the service.</param>
        /// <param name="configurationName">If the configuration name is used to get the endpoint configuration from the config file, if the parameter is null it will default to the interface's type full name.</param>
        public static object CreateInstance(Type interfaceType, Uri serviceUrl = null, string configurationName = null)
        {
            return CreateInstance(null, interfaceType, serviceUrl, configurationName);
        }

        /// <summary>
        /// Creates an instance of a concrete class implementing the interfaceType, the class is derived from baseType.
        /// </summary>
        /// <param name="baseType">
        /// The base class which will be used to generate the concrete type implementing the interface.
        /// The class must have non default constructor:
        ///     (Uri serviceUrl, string configurationName, Func&lt;Func&lt;TResult>, TResult> retryStrategy) 
        ///     or
        ///     (Uri serviceUrl, string configurationName)
        /// </param>
        /// <param name="interfaceType">
        /// Interface which should be implemented, the interface must implement only one method with one parameter and non void return parameter.
        /// The method's parameter type will be used as the TQuery generic parameter and the return type as the TResult.
        /// The method can have any name. The method's implementation will call to TResult Execute(TQuery query) method of the baseType.
        /// </param>
        /// <param name="serviceUrl">The URL of the service.</param>
        /// <param name="configurationName">If the configuration name is used to get the endpoint configuration from the config file, if the parameter is null it will default to the interface's type full name.</param>
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
            var threeParamCtor = true;

            var baseCtor = baseType.GetConstructor(
                new[] {typeof (Uri), typeof (string), interfaceInfo.RetryStragtegyType});

            if (baseCtor == null)
            {
                threeParamCtor = false;

                baseCtor = baseType.GetConstructor(new[] { typeof(Uri), typeof(string)});

                if(baseCtor == null)
                    throw new InvalidOperationException(string.Format(Resources.Text.must_implement_constructor, baseType.FullName, interfaceInfo.RetryStragtegyType.FullName));
            }

            var ctor = typeBuilder.DefineConstructor(
                MethodAttributes.Public, CallingConventions.Standard | CallingConventions.HasThis,
                new[] {typeof (Uri), typeof (string)});

            var ctorGenerator = ctor.GetILGenerator();

            ctorGenerator.Emit(OpCodes.Ldarg_0);
            ctorGenerator.Emit(OpCodes.Ldarg_1);
            ctorGenerator.Emit(OpCodes.Ldarg_2);

            if (threeParamCtor)
                ctorGenerator.Emit(OpCodes.Ldnull);

            ctorGenerator.Emit(OpCodes.Call, baseCtor);
            ctorGenerator.Emit(OpCodes.Ret);
        }

        static void DefineServiceCallMethod(Type baseType, ClientInterfaceInfo interfaceInfo, TypeBuilder typeBuilder)
        {
            var baseExecute = baseType.GetMethod("Execute", new[] { interfaceInfo.QueryType });
            if (baseExecute == null)
                throw new InvalidOperationException(
                    string.Format(Resources.Text.must_have_execute_method, baseType.FullName, interfaceInfo.ResultType.FullName, interfaceInfo.QueryType.FullName));

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
                    throw new ArgumentException(string.Format(Resources.Text.must_be_interface, interfaceType.FullName), "interfaceType");

                var methods = interfaceType.GetMethods();
                if (methods.Length != 1)
                    throw new ArgumentException(string.Format(Resources.Text.must_have_only_one_method, interfaceType.FullName), "interfaceType");

                ServiceExecuteMethod = methods[0];
                ResultType = ServiceExecuteMethod.ReturnType;

                if (ResultType == typeof(void))
                    throw new ArgumentException(string.Format(Resources.Text.must_have_non_void_retun_parameter, ServiceExecuteMethod.Name, interfaceType.FullName), "interfaceType");

                var parameters = ServiceExecuteMethod.GetParameters();
                if (parameters.Length != 1)
                    throw new ArgumentException(string.Format(Resources.Text.method_must_have_only_one_argument, ServiceExecuteMethod.Name, interfaceType.FullName), "interfaceType");

                QueryType = parameters[0].ParameterType;

                //typeof(Func<Func<Result>, Result>)
                RetryStragtegyType = typeof(Func<,>).MakeGenericType(typeof(Func<>).MakeGenericType(ResultType), ResultType);
            }
        }
    }
}
