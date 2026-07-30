namespace Contoso.PerformanceTestingRuntime.CommerceRuntime.Messages
{
    using System.Collections.ObjectModel;
    using System.Runtime.Serialization;
    using Contoso.PerformanceTestingRuntime.CommerceRuntime.Contracts;
    using Microsoft.Dynamics.Commerce.Runtime.Messages;

    /// <summary>
    /// Defines the CRT response returned by one PT workflow execution.
    /// </summary>
    [DataContract]
    public class PTWorkflowResponse : Response
    {
        /// <summary>
        /// Gets or sets the workflow request id.
        /// </summary>
        [DataMember]
        public string WorkflowRequestId { get; set; }

        /// <summary>
        /// Gets or sets the cart id.
        /// </summary>
        [DataMember]
        public string CartId { get; set; }

        /// <summary>
        /// Gets or sets the sales transaction id.
        /// </summary>
        [DataMember]
        public string TransactionId { get; set; }

        /// <summary>
        /// Gets or sets the sales order id.
        /// </summary>
        [DataMember]
        public string SalesId { get; set; }

        /// <summary>
        /// Gets or sets the optional customer account number.
        /// </summary>
        [DataMember]
        public string CustomerAccountNumber { get; set; }

        /// <summary>
        /// Gets or sets the total number of sale lines.
        /// </summary>
        [DataMember]
        public int TotalItems { get; set; }

        /// <summary>
        /// Gets or sets the total sold quantity.
        /// </summary>
        [DataMember]
        public decimal TotalQuantity { get; set; }

        /// <summary>
        /// Gets or sets the final sale amount.
        /// </summary>
        [DataMember]
        public decimal TotalSaleAmount { get; set; }

        /// <summary>
        /// Gets or sets the sale currency.
        /// </summary>
        [DataMember]
        public string Currency { get; set; }

        /// <summary>
        /// Gets or sets the whole-workflow timing details.
        /// </summary>
        [DataMember]
        public PTWorkflowPerformance Performance { get; set; }

        /// <summary>
        /// Gets or sets the line-level sale results.
        /// </summary>
        [DataMember]
        public Collection<PTWorkflowSaleLineResult> Lines { get; set; }

        /// <summary>
        /// Gets or sets the workflow status.
        /// </summary>
        [DataMember]
        public string Status { get; set; }

        /// <summary>
        /// Gets or sets the workflow status message.
        /// </summary>
        [DataMember]
        public string StatusMessage { get; set; }
    }
}
