namespace Contoso.PerformanceTestingRuntime.CommerceRuntime.RequestHandlers
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Diagnostics;
    using System.Globalization;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Contoso.PerformanceTestingRuntime.CommerceRuntime.Contracts;
    using Contoso.PerformanceTestingRuntime.CommerceRuntime.Execution;
    using Contoso.PerformanceTestingRuntime.CommerceRuntime.Messages;
    using Contoso.PerformanceTestingRuntime.CommerceRuntime.Workflows;
    using Microsoft.Dynamics.Commerce.Runtime;
    using Microsoft.Dynamics.Commerce.Runtime.Messages;

    /// <summary>
    /// Handles single sale workflow performance requests.
    /// </summary>
    public class PTWorkflowRequestHandler : IRequestHandlerAsync
    {
        private const int MaximumCartIdLength = 44;
        private const string CartIdPrefix = "PT-";
        private const string SuccessStatus = "Success";
        private const string ValidationFailedStatus = "ValidationFailed";
        private const string FailedStatus = "Failed";

        private readonly PTSaleWorkflow workflow;

        /// <summary>
        /// Initializes a new instance of the <see cref="PTWorkflowRequestHandler"/> class.
        /// </summary>
        public PTWorkflowRequestHandler()
        {
            this.workflow = new PTSaleWorkflow();
        }

        /// <summary>
        /// Gets the request types supported by this handler.
        /// </summary>
        public IEnumerable<Type> SupportedRequestTypes => new[] { typeof(PTWorkflowRequest) };

        /// <summary>
        /// Executes the request.
        /// </summary>
        /// <param name="request">The request to execute.</param>
        /// <returns>The workflow response.</returns>
        public async Task<Response> Execute(Request request)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            if (request.GetType() != typeof(PTWorkflowRequest))
            {
                string message = string.Format(CultureInfo.InvariantCulture, "Request '{0}' is not supported.", request.GetType());
                throw new NotSupportedException(message);
            }

            var workflowRequest = (PTWorkflowRequest)request;
            string validationMessage = Validate(workflowRequest);
            string cartId = string.IsNullOrWhiteSpace(validationMessage) ? BuildCartId(workflowRequest.WorkflowRequestId) : null;
            if (!string.IsNullOrWhiteSpace(validationMessage))
            {
                return CreateResponse(workflowRequest, cartId, null, ValidationFailedStatus, validationMessage);
            }

            DateTimeOffset startedUtc = DateTimeOffset.UtcNow;
            Stopwatch stopwatch = Stopwatch.StartNew();
            var measurements = new Collection<PTWorkflowMeasurement>();

            try
            {
                var executor = new RequestContextRequestExecutor(request.RequestContext);
                PTWorkflowResponse response = await this.workflow.RunAsync(executor, workflowRequest, cartId, measurements).ConfigureAwait(false);
                stopwatch.Stop();

                response.Performance = CreatePerformance(startedUtc, stopwatch, measurements);
                response.Status = SuccessStatus;
                response.StatusMessage = "Completed.";
                return response;
            }
            catch (Exception exception)
            {
                stopwatch.Stop();
                return CreateResponse(
                    workflowRequest,
                    cartId,
                    CreatePerformance(startedUtc, stopwatch, measurements),
                    FailedStatus,
                    exception.Message);
            }
        }

        private static string Validate(PTWorkflowRequest request)
        {
            var errors = new List<string>();
            AddRequiredValidationError(errors, request.WorkflowRequestId, nameof(request.WorkflowRequestId));
            AddRequiredValidationError(errors, request.StoreId, nameof(request.StoreId));
            AddRequiredValidationError(errors, request.TerminalId, nameof(request.TerminalId));
            AddRequiredValidationError(errors, request.StaffId, nameof(request.StaffId));

            if (request.ChannelId <= 0)
            {
                errors.Add("ChannelId is required and must be greater than zero.");
            }

            if (request.Lines == null || request.Lines.Count == 0)
            {
                errors.Add("Lines must contain at least one product.");
            }
            else
            {
                if (request.Lines.Any(line => line == null))
                {
                    errors.Add("Lines cannot contain null entries.");
                }

                foreach (PTWorkflowLine line in request.Lines.Where(line => line != null))
                {
                    if (line.ProductId <= 0)
                    {
                        errors.Add("Each line ProductId must be greater than zero.");
                    }

                    if (line.Quantity <= 0)
                    {
                        errors.Add("Each line Quantity must be greater than zero.");
                    }
                }
            }

            if (errors.Count == 0)
            {
                string cartId = BuildCartId(request.WorkflowRequestId);
                if (cartId.Length > MaximumCartIdLength)
                {
                    errors.Add(string.Format(
                        CultureInfo.InvariantCulture,
                        "WorkflowRequestId is too long. Cart id '{0}' exceeds {1} characters.",
                        cartId,
                        MaximumCartIdLength));
                }
            }

            return string.Join(" ", errors);
        }

        private static void AddRequiredValidationError(ICollection<string> errors, string value, string fieldName)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                errors.Add(string.Format(CultureInfo.InvariantCulture, "{0} is required.", fieldName));
            }
        }

        private static string BuildCartId(string workflowRequestId)
        {
            return CartIdPrefix + SanitizeCartIdPart(workflowRequestId);
        }

        private static string SanitizeCartIdPart(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return string.Empty;
            }

            var builder = new StringBuilder(value.Length);
            foreach (char character in value.Trim())
            {
                builder.Append(IsCartIdCharacter(character) ? character : '-');
            }

            return builder.ToString();
        }

        private static bool IsCartIdCharacter(char character)
        {
            return char.IsLetterOrDigit(character) || character == '-' || character == '_';
        }

        private static PTWorkflowPerformance CreatePerformance(
            DateTimeOffset startedUtc,
            Stopwatch stopwatch,
            Collection<PTWorkflowMeasurement> measurements)
        {
            return new PTWorkflowPerformance
            {
                StartedUtc = startedUtc,
                EndedUtc = DateTimeOffset.UtcNow,
                DurationMilliseconds = stopwatch.ElapsedMilliseconds,
                Measurements = measurements ?? new Collection<PTWorkflowMeasurement>(),
            };
        }

        private static PTWorkflowResponse CreateResponse(
            PTWorkflowRequest request,
            string cartId,
            PTWorkflowPerformance performance,
            string status,
            string statusMessage)
        {
            return new PTWorkflowResponse
            {
                WorkflowRequestId = request?.WorkflowRequestId,
                CartId = cartId,
                CustomerAccountNumber = request?.CustomerAccountNumber,
                TotalItems = 0,
                TotalQuantity = 0M,
                TotalSaleAmount = 0M,
                Performance = performance,
                Lines = new Collection<PTWorkflowSaleLineResult>(),
                Status = status,
                StatusMessage = statusMessage,
            };
        }
    }
}
