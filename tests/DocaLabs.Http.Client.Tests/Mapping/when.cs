using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Serialization;
using DocaLabs.Http.Client.JsonSerialization;
using DocaLabs.Http.Client.RequestSerialization;
using Machine.Specifications;

namespace DocaLabs.Http.Client.Tests.Mapping
{
    [Subject(typeof(HttpClient<,>))]
    public class when
    {
        static bool b;

        Because of = () =>
        {
            var sss = JsonSerializationProvider.Serializer;

            var x1 = HttpClientFactory.CreateInstance<Ix1>(new Uri("http://www.contoso.foo/"));
            var x2 = HttpClientFactory.CreateInstance<Ix2>(new Uri("http://www.contoso.foo/"));
            var x3 = HttpClientFactory.CreateInstance<Ix3>(new Uri("http://www.contoso.foo/"));
            var x4 = HttpClientFactory.CreateInstance<Ix4>(new Uri("http://www.contoso.foo/"));

            var s1 = HttpClientFactory.CreateInstance(typeof(TestHttpClient2<,>), typeof(IService1), new Uri("http://www.contoso.foo/")) as IService1;
            s1.Put(new Query());

            var s2 = HttpClientFactory.CreateInstance(typeof(TestHttpClient2<,>), typeof(IService2), new Uri("http://www.contoso.foo/")) as IService2;
            var v2 = s2.Get();

            var s3 = HttpClientFactory.CreateInstance(typeof(TestHttpClient2<,>), typeof(IService3), new Uri("http://www.contoso.foo/")) as IService3;
            s3.Do();

            var testInterfaces = new List<Type>();
            for (var i = 0; i < 1000; i++)
                testInterfaces.Add(InterfaceBuilder.CreateInterface(i, typeof(Query), typeof(Result)));

            var baseClass = typeof (TestHttpClient);
            var c = 0;
            var maxC = 0;
            var k = 0;
            Parallel.ForEach(testInterfaces, x =>
            {
                var cc = Interlocked.Increment(ref c);
                if (cc > maxC)
                    maxC = cc;

                var ii = Interlocked.Increment(ref k);
                if((ii % 1000) == 0)
                    Console.WriteLine(ii);

                //Console.WriteLine(cc);
                var s = HttpClientFactory.CreateInstance(baseClass, x, new Uri("http://www.contoso.foo/"));
                var r = s.GetType().GetMethod("CallService").Invoke(s, new object[] { new Query { Name = "name" } });
                Interlocked.Decrement(ref c);
                //Console.WriteLine(s.GetType().FullName);
            });

            Console.WriteLine("Max concurrency {0}", maxC);

            b = true;
            //var s0 = HttpClientFactory.CreateInstance(testInterfaces[0], new Uri("http://www.contoso.foo/"));
            //var s1 = HttpClientFactory.CreateInstance<IService1>(new Uri("http://www.contoso.foo/"));

            //service = s1;
            //r = service.Execute(new Query {Name = "name"});
        };

        It should = () => b.ShouldBeTrue();
    }

    [AttributeUsage(AttributeTargets.Interface)]
    public class Attribute1 : Attribute
    {
        
    }

    public class Attribute2 : Attribute
    {
        public int Value;
    }

    [SerializeAsXml, Attribute1]
    public interface Ix1
    {
        Result Get();
    }

    [SerializeAsXml(RequestContentEncoding = "x-gzip", Encoding = "utf8"), Attribute1, Attribute2(Value = 42)]
    public interface Ix2
    {
        Result Get();
    }

    [SerializeAsXml(Indent = true, Encoding = "utf8")]
    [XmlRoot("root", IsNullable = true)]
    public interface Ix3
    {
        Result Get();
    }

    [SerializeAsXml(RequestContentEncoding = "x-gzip", Encoding = "utf8")]
    [XmlRoot("root")]
    public interface Ix4
    {
        Result Get();
    }

    public interface IService1
    {
        void Put(Query query);
    }

    public interface IService2
    {
        Result Get();
    }

    public interface IService3
    {
        void Do();
    }

    public class Query
    {
        public string Name { get; set; }
    }

    public class Result
    {
        public string Data { get; set; }
    }

    public class TestHttpClient : HttpClient<Query, Result>
    {
        public TestHttpClient(Uri baseUrl, string configurationName)
            : base(baseUrl, configurationName)
        {
        }

        protected override Result ExecutePipeline(Query query)
        {
            Thread.Sleep(30);
            return new Result {Data = "Hello!"};
        }
    }

    public class TestHttpClient2<TQuery, TResult> : HttpClient<TQuery, TResult>
    {
        public TestHttpClient2(Uri baseUrl, string configurationName)
            : base(baseUrl, configurationName)
        {
        }

        protected override TResult ExecutePipeline(TQuery query)
        {
            Thread.Sleep(30);
            return default(TResult);
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
