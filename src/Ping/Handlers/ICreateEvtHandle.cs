using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ping.Handlers
{
    public interface ICreateEvtHandle
    {
        IHandle<T> Create<T>();
       
    }
}
