using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pong.Messages
{
    public class BaseMessage
    {
        public Guid AggregateId { get; set; }
    }
}
