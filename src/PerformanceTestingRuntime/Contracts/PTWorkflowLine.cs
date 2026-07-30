namespace Contoso.PerformanceTestingRuntime.CommerceRuntime.Contracts
{
    using System.Runtime.Serialization;

    /// <summary>
    /// Defines a product line for a PT workflow.
    /// </summary>
    [DataContract]
    public class PTWorkflowLine
    {
        /// <summary>
        /// Gets or sets the commerce product record identifier.
        /// </summary>
        [DataMember]
        public long ProductId { get; set; }

        /// <summary>
        /// Gets or sets the quantity.
        /// </summary>
        [DataMember]
        public decimal Quantity { get; set; }
    }
}
