namespace Contoso.PerformanceTestingRuntime.CommerceRuntime.Execution
{
    using System.Threading.Tasks;
    using Microsoft.Dynamics.Commerce.Runtime;
    using Microsoft.Dynamics.Commerce.Runtime.Messages;

    internal class RequestContextRequestExecutor : ICommerceRequestExecutor
    {
        private readonly RequestContext context;

        internal RequestContextRequestExecutor(RequestContext context)
        {
            this.context = context;
        }

        public Task<TResponse> ExecuteAsync<TResponse>(Request request)
            where TResponse : Response
        {
            ThrowIf.Null(this.context, nameof(this.context));
            return this.context.ExecuteAsync<TResponse>(request);
        }
    }
}
