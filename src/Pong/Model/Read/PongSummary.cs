using System;
using System.ComponentModel.DataAnnotations;

namespace Pong.Model.Read
{
    public class PongSummary
    {
        [Key]
        public Guid Id { get; set; }
        public Guid PingId { get; set; }
        public int Count { get; set; }
        public DateTimeOffset RequestTime { get; set; }
    }
}
