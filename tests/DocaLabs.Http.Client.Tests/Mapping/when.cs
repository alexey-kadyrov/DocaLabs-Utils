using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using System.Threading;
using System.Threading.Tasks;
using Machine.Specifications;

namespace DocaLabs.Http.Client.Tests.Mapping
{
    [Subject(typeof(HttpClient<,>))]
    public class when
    {
        static IService1 service;
        static object o;
        //static Result r;

        Because of = () =>
        {
            var testInterfaces = new List<Type>();
            for (var i = 0; i < 100000; i++)
                testInterfaces.Add(InterfaceBuilder.CreateInterface(i, typeof(Query), typeof(Result)));

            int c = 0;
            int maxC = 0;
            int k = 0;
            Parallel.ForEach(testInterfaces, x =>
            {
                var cc = Interlocked.Increment(ref c);
                if (cc > maxC)
                    maxC = cc;

                var ii = Interlocked.Increment(ref k);
                if((ii % 1000) == 0)
                    Console.WriteLine(ii);

                //Console.WriteLine(cc);
                var s = HttpClientFactory.CreateInstance(x, new Uri("http://www.contoso.foo/"));
                var r = s.GetType().GetMethod("CallService").Invoke(s, new object[] { new Query { Name = "name" } });
                Interlocked.Decrement(ref c);
                //Console.WriteLine(s.GetType().FullName);
            });

            Console.WriteLine("Max concurrency {0}", maxC);

            //var s0 = HttpClientFactory.CreateInstance(testInterfaces[0], new Uri("http://www.contoso.foo/"));
            //var s1 = HttpClientFactory.CreateInstance<IService1>(new Uri("http://www.contoso.foo/"));

            //service = s1;
            //r = service.Execute(new Query {Name = "name"});
        };

        It should_be_not_null =
            () => service.ShouldBeNull();
    }

    public class Query
    {
        public string Name { get; set; }
    }

    public class Result
    {
        public string Data { get; set; }
    }

    public interface IService1
    {
        Result Execute(Query query);
    }

    public class Service : HttpClient<Query, Result>, IService1
    {
        public Service(Uri serviceUrl, string configurationName)
            : base(serviceUrl, configurationName)
        {
        }
    }

    static class InterfaceBuilder
    {
        static readonly ModuleBuilder ModuleBuilder;

        static InterfaceBuilder()
        {
            var assemblyName = typeof(InterfaceBuilder).Namespace + "__test_interfaces";

            var assemblyBuilder = AppDomain.CurrentDomain.DefineDynamicAssembly(
                new AssemblyName(assemblyName),
                AssemblyBuilderAccess.Run);

            ModuleBuilder = assemblyBuilder.DefineDynamicModule(assemblyName);
        }

        public static Type CreateInterface(int suffix, Type queryType, Type resultType)
        {
            var typeBuilder = ModuleBuilder.DefineType(
                "ITestService" + suffix, TypeAttributes.Interface | TypeAttributes.Abstract | TypeAttributes.Public);

            typeBuilder.DefineMethod("CallService", 
                MethodAttributes.Public | MethodAttributes.Abstract | MethodAttributes.Virtual, resultType, new[] { queryType });

            return typeBuilder.CreateType();
        }
    }
}
