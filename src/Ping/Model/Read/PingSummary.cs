using System;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace Ping.Model.Read
{
    [Serializable]
    [DataContract]
    public class PingSummary
    {
        [Key]
        [DataMember]
        public Guid Id { get; set; }

        [DataMember]
        public int TotalResponses { get; set; }

        [DataMember]
        public bool Active { get; set; }

        [DataMember]
        public DateTimeOffset Start { get; set; }

        [DataMember]
        public DateTimeOffset? End { get; set; }

        [DataMember]
        public double PingsPerSecond { get; set; }
    }
}