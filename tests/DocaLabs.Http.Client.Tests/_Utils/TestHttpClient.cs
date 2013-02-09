using System;
using System.Threading;

namespace DocaLabs.Http.Client.Tests._Utils
{
    public class TestHttpClient<TQuery, TResult> : HttpClient<TQuery, TResult>
    {
        public TimeSpan ExecutePipelineTime { get; set; }

        public string ExecutionMarker { get; set; }

        public TestHttpClient(Uri baseUrl, string configurationName)
            : base(baseUrl, configurationName)
        {
        }

        protected override TResult ExecutePipeline(TQuery query)
        {
            if (ExecutePipelineTime != TimeSpan.Zero)
                Thread.Sleep(ExecutePipelineTime);

            var result = Activator.CreateInstance<TResult>();

            if(typeof(TQuery) != typeof(VoidType) && typeof(TResult) != typeof(VoidType))
                typeof(TResult).GetProperty("Value").SetValue(result, typeof(TQuery).GetProperty("Value").GetValue(query, null));

            ExecutionMarker = "Pipeline was executed.";

            return result;
        }
    }

    public class TestHttpClient2 : TestHttpClient<TestsQuery, TestResult>
    {
        public TestHttpClient2(Uri baseUrl, string configurationName)
            : base(baseUrl, configurationName)
        {
        }
    }
}
