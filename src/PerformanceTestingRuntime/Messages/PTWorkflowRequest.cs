namespace Contoso.PerformanceTestingRuntime.CommerceRuntime.Messages
{
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Runtime.Serialization;
    using Contoso.PerformanceTestingRuntime.CommerceRuntime.Contracts;
    using Microsoft.Dynamics.Commerce.Runtime.Messages;

    /// <summary>
    /// Defines the CRT request used to execute one PT workflow.
    /// </summary>
    [DataContract]
    public class PTWorkflowRequest : Request
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PTWorkflowRequest"/> class.
        /// </summary>
        /// <param name="workflowRequestId">The workflow request id.</param>
        /// <param name="channelId">The channel record id.</param>
        /// <param name="storeId">The store id.</param>
        /// <param name="terminalId">The terminal id.</param>
        /// <param name="staffId">The staff id.</param>
        /// <param name="customerAccountNumber">The optional customer account number.</param>
        /// <param name="lines">The product lines to add to the cart.</param>
        public PTWorkflowRequest(
            string workflowRequestId,
            long channelId,
            string storeId,
            string terminalId,
            string staffId,
            string customerAccountNumber,
            IEnumerable<PTWorkflowLine> lines)
        {
            this.WorkflowRequestId = workflowRequestId;
            this.ChannelId = channelId;
            this.StoreId = storeId;
            this.TerminalId = terminalId;
            this.StaffId = staffId;
            this.CustomerAccountNumber = customerAccountNumber;
            this.Lines = new Collection<PTWorkflowLine>((lines ?? Enumerable.Empty<PTWorkflowLine>()).ToList());
        }

        /// <summary>
        /// Gets the workflow request id.
        /// </summary>
        [DataMember]
        public string WorkflowRequestId { get; private set; }

        /// <summary>
        /// Gets the channel record id.
        /// </summary>
        [DataMember]
        public long ChannelId { get; private set; }

        /// <summary>
        /// Gets the store id.
        /// </summary>
        [DataMember]
        public string StoreId { get; private set; }

        /// <summary>
        /// Gets the terminal id.
        /// </summary>
        [DataMember]
        public string TerminalId { get; private set; }

        /// <summary>
        /// Gets the staff id.
        /// </summary>
        [DataMember]
        public string StaffId { get; private set; }

        /// <summary>
        /// Gets the optional customer account number.
        /// </summary>
        [DataMember]
        public string CustomerAccountNumber { get; private set; }

        /// <summary>
        /// Gets the product lines to add to the cart.
        /// </summary>
        [DataMember]
        public Collection<PTWorkflowLine> Lines { get; private set; }
    }
}
