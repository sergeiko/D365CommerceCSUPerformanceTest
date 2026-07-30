namespace Contoso.PerformanceTestingRuntime.CommerceRuntime.Execution
{
    using System.Threading.Tasks;
    using Microsoft.Dynamics.Commerce.Runtime.Messages;

    internal interface ICommerceRequestExecutor
    {
        Task<TResponse> ExecuteAsync<TResponse>(Request request)
            where TResponse : Response;
    }
}
