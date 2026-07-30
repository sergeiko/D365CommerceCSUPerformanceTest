namespace Contoso.PerformanceTestingRuntime.CommerceRuntime.Contracts
{
    using System;
    using System.Collections.ObjectModel;
    using System.Runtime.Serialization;

    /// <summary>
    /// Defines whole-workflow timing details.
    /// </summary>
    [DataContract]
    public class PTWorkflowPerformance
    {
        /// <summary>
        /// Gets or sets the UTC workflow start time.
        /// </summary>
        [DataMember]
        public DateTimeOffset StartedUtc { get; set; }

        /// <summary>
        /// Gets or sets the UTC workflow end time.
        /// </summary>
        [DataMember]
        public DateTimeOffset EndedUtc { get; set; }

        /// <summary>
        /// Gets or sets the elapsed workflow duration in milliseconds.
        /// </summary>
        [DataMember]
        public long DurationMilliseconds { get; set; }

        /// <summary>
        /// Gets or sets timing details for measured workflow steps.
        /// </summary>
        [DataMember]
        public Collection<PTWorkflowMeasurement> Measurements { get; set; }
    }
}
