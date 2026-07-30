namespace Contoso.PerformanceTestingRuntime.CommerceRuntime.Contracts
{
    using System.Collections.ObjectModel;
    using System.Runtime.Serialization;

    /// <summary>
    /// Defines the API payload used to execute a single PT workflow.
    /// </summary>
    [DataContract]
    public class PTWorkflowExecuteRequest
    {
        /// <summary>
        /// Gets or sets the caller-supplied workflow request identifier.
        /// </summary>
        [DataMember]
        public string WorkflowRequestId { get; set; }

        /// <summary>
        /// Gets or sets the retail channel record identifier.
        /// </summary>
        [DataMember(Name = "channelId")]
        public long channelId { get; set; }

        /// <summary>
        /// Gets or sets the store identifier.
        /// </summary>
        [DataMember(Name = "storeId")]
        public string storeId { get; set; }

        /// <summary>
        /// Gets or sets the terminal identifier.
        /// </summary>
        [DataMember(Name = "terminalId")]
        public string terminalId { get; set; }

        /// <summary>
        /// Gets or sets the staff identifier.
        /// </summary>
        [DataMember(Name = "staffid")]
        public string staffid { get; set; }

        /// <summary>
        /// Gets or sets the optional customer account number.
        /// </summary>
        [DataMember]
        public string CustomerAccountNumber { get; set; }

        /// <summary>
        /// Gets or sets product ids and quantities to add to the cart.
        /// </summary>
        [DataMember]
        public Collection<PTWorkflowLine> Lines { get; set; }
    }
}
