namespace Contoso.PerformanceTestingRuntime.CommerceRuntime.Contracts
{
    using System;
    using System.Runtime.Serialization;

    /// <summary>
    /// Defines timing for one measured workflow step.
    /// </summary>
    [DataContract]
    public class PTWorkflowMeasurement
    {
        /// <summary>
        /// Gets or sets the measured step name.
        /// </summary>
        [DataMember]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the UTC step start time.
        /// </summary>
        [DataMember]
        public DateTimeOffset StartedUtc { get; set; }

        /// <summary>
        /// Gets or sets the UTC step end time.
        /// </summary>
        [DataMember]
        public DateTimeOffset EndedUtc { get; set; }

        /// <summary>
        /// Gets or sets the elapsed step duration in milliseconds.
        /// </summary>
        [DataMember]
        public long DurationMilliseconds { get; set; }

        /// <summary>
        /// Gets or sets the measured step status.
        /// </summary>
        [DataMember]
        public string Status { get; set; }

        /// <summary>
        /// Gets or sets the measured step status message.
        /// </summary>
        [DataMember]
        public string StatusMessage { get; set; }
    }
}
