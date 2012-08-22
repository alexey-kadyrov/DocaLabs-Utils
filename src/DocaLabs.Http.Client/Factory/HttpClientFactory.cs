using System;
using System.Collections.Concurrent;
using System.Reflection;
using System.Text;
using DocaLabs.Utils;

namespace DocaLabs.Http.Client.Factory
{
    /// <summary>
    /// Defines methods to generate and instantiate a class for a given interface to hook into http services using HttpClient class.
    /// </summary>
    /// For:
    ///     class OutData {
    ///     }
    ///     class InData {
    ///     }
    ///     interface IInterface {
    ///         OutData GetData(InData query);
    ///     }
    /// 
    /// It will generate something like:
    ///     class IInterface_ClientsImpl : HttpClientAccessBase, IInterface {
    ///         public OutData GetData(InData query) {
    ///             return GetInstance&lt;InData, OutData>(null, null, "DocaLabs.Http.Client.Factory.IInterface.GetData").Execute(query);
    ///         }
    ///     }
    public static class HttpClientFactory
    {
        const string ClassNameTemplate =
            "{0}_{1}_ClientsImpl";

        const string ClassDeclarationTemplate =
            "public class {0} : DocaLabs.Http.Client.Factory.HttpClientAccessBase, {1}\r\n{{\r\n";

        // {0} - result type, {1} - interface method name, {2} - query type, {3} - service Uri, {4} - request method, {5} - endpoint name
        const string MethodTemplate = 
            "\tpublic {0} {1}({2} query)\r\n" +
            "\t{{\r\n" +
            "\t\treturn GetInstance<{2}, {0}>({3}, {4}, \"{5}\").Execute(query);\r\n" +
            "\t}}\r\n";

        static ConcurrentDictionary<Type, Type> Implementations { get; set; }

        static HttpClientFactory()
        {
            Implementations = new ConcurrentDictionary<Type, Type>();
        }

        /// <summary>
        /// Generates and instantiates class for a given interface.
        /// </summary>
        public static T Create<T>()
        {
            return (T)Create(typeof (T));
        }

        /// <summary>
        /// Generates and instantiates class for a given interface.
        /// </summary>
        public static object Create(Type interfaceType)
        {
            return Activator.CreateInstance(Implementations.GetOrAdd(interfaceType, Generate));
        }

        static Type Generate(Type interfaceType)
        {
            var className = GetClassName(interfaceType);

            return InitializeCompiler().Compile(EmmitClassBody(interfaceType, className)).GetType(className);
        }

        static string GetClassName(Type interfaceType)
        {
            return string.Format(ClassNameTemplate, interfaceType.Name, Guid.NewGuid().ToString("N"));
        }

        static CompileHelper InitializeCompiler()
        {
            var helper = new CompileHelper();

            helper.ReferencedAssemblies.AddRange(CompileHelper.GetLoadedAssemblies());

            return helper;
        }

        static string EmmitClassBody(Type interfaceType, string className)
        {
            var classBuilder = new StringBuilder();

            classBuilder.AppendFormat(ClassDeclarationTemplate, className, interfaceType.FullName);

            foreach (var method in interfaceType.GetAllMethods(BindingFlags.Instance | BindingFlags.Public))
            {
                classBuilder.Append(EmmitMethodBody(method));
            }

            classBuilder.AppendLine("}");

            return classBuilder.ToString();
        }

        static string EmmitMethodBody(MethodInfo info)
        {
            if(info.ReturnType == typeof(void))
                throw new HttpClientFactoryException(string.Format(Resources.Text.method_must_have_return_value, info.Name));

            var args = info.GetParameters();
            if(args.Length != 1)
                throw new HttpClientFactoryException(string.Format(Resources.Text.method_must_have_only_one_argument, info.Name));

            if (info.DeclaringType == null)
                throw new HttpClientFactoryException(string.Format(Resources.Text.declaring_is_unknown, info.Name));

            string requesMethod = null, serviceUrl = null, enpointName = info.DeclaringType.FullName + "." + info.Name;

            ExtractSuggestedParameters(info, ref serviceUrl, ref requesMethod);

            requesMethod = AdjustRequestMethod(requesMethod);

            serviceUrl = AdjustServiceUrl(serviceUrl);

            return string.Format(MethodTemplate, info.ReturnType.FullName, info.Name, args[0].ParameterType.FullName, serviceUrl, requesMethod, enpointName);
        }

        static void ExtractSuggestedParameters(MethodInfo info, ref string serviceUrl, ref string requesMethod)
        {
            var attrs = info.GetCustomAttributes(typeof (HttpServiceAttribute), true);
            if (attrs.Length <= 0)
                return;

            requesMethod = ((HttpServiceAttribute) attrs[0]).Method;
            serviceUrl = ((HttpServiceAttribute) attrs[0]).ServiceUrl;
        }

        static string AdjustRequestMethod(string suggestedRequesMethod)
        {
            return string.IsNullOrWhiteSpace(suggestedRequesMethod) 
                ? "null" 
                : "\"" + suggestedRequesMethod + "\"";
        }

        static string AdjustServiceUrl(string serviceUrl)
        {
            return string.IsNullOrWhiteSpace(serviceUrl) 
                ? "null" 
                : "new System.Uri(\"" + serviceUrl + "\")";
        }
    }
}
