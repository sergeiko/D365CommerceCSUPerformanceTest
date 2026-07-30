namespace Contoso.PerformanceTestingRuntime.CommerceRuntime.Controllers
{
    using System.Threading.Tasks;
    using Contoso.PerformanceTestingRuntime.CommerceRuntime.Contracts;
    using Contoso.PerformanceTestingRuntime.CommerceRuntime.Messages;
    using Microsoft.Dynamics.Commerce.Runtime.DataModel;
    using Microsoft.Dynamics.Commerce.Runtime.Hosting.Contracts;

    /// <summary>
    /// Provides endpoints for single workflow performance testing.
    /// </summary>
    [RoutePrefix("PerformanceTests")]
    public class PTWorkflowController : IController
    {
        /// <summary>
        /// Executes one sale performance workflow.
        /// </summary>
        /// <param name="context">The endpoint context.</param>
        /// <param name="workflowParameters">The workflow parameters.</param>
        /// <returns>The workflow response.</returns>
        [HttpPost]
        [Authorization(CommerceRoles.Device, CommerceRoles.Employee, CommerceRoles.Application)]
        public async Task<PTWorkflowResponse> PTWorkflowExecute(
            IEndpointContext context,
            PTWorkflowExecuteRequest workflowParameters)
        {
            workflowParameters = workflowParameters ?? new PTWorkflowExecuteRequest();

            var request = new PTWorkflowRequest(
                workflowParameters.WorkflowRequestId,
                workflowParameters.channelId,
                workflowParameters.storeId,
                workflowParameters.terminalId,
                workflowParameters.staffid,
                workflowParameters.CustomerAccountNumber,
                workflowParameters.Lines);

            return await context.ExecuteAsync<PTWorkflowResponse>(request).ConfigureAwait(false);
        }
    }
}
