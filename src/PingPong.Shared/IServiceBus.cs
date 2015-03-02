using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PingPong.Shared
{
    public interface IServiceBus
    {
        void Publish(object msg);
    }
}
