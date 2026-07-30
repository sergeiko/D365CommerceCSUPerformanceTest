namespace Contoso.PerformanceTestingRuntime.CommerceRuntime.Contracts
{
    using System.Runtime.Serialization;

    /// <summary>
    /// Defines line-level sale output returned by a PT workflow.
    /// </summary>
    [DataContract]
    public class PTWorkflowSaleLineResult
    {
        /// <summary>
        /// Gets or sets the cart or sales line identifier.
        /// </summary>
        [DataMember]
        public string LineId { get; set; }

        /// <summary>
        /// Gets or sets the commerce product record identifier.
        /// </summary>
        [DataMember]
        public long ProductId { get; set; }

        /// <summary>
        /// Gets or sets the sold quantity.
        /// </summary>
        [DataMember]
        public decimal Quantity { get; set; }

        /// <summary>
        /// Gets or sets the line total amount.
        /// </summary>
        [DataMember]
        public decimal TotalAmount { get; set; }
    }
}
