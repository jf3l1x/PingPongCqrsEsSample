using System;
using System.ComponentModel.DataAnnotations;

namespace Ping.Shared.Model.Read
{
    [Serializable]
    public class PingSummary
    {
        [Key]
        public Guid Id { get; set; }

        public int TotalResponses { get; set; }

        public bool Active { get; set; }

        public DateTimeOffset Start { get; set; }

        public DateTimeOffset? End { get; set; }

        public double PingsPerSecond { get; set; }
    }
}