using System;
using CommonDomain;
using Newtonsoft.Json;

namespace PingPong.Shared
{
    public class SnapShot : IMemento
    {
        public SnapShot()
        {
        }

        public SnapShot(dynamic data)
        {
            Data = JsonConvert.SerializeObject(data);
        }

        public string Data { get; set; }
        public Guid Id { get; set; }
        public int Version { get; set; }

        public dynamic GetObject()
        {
            return JsonConvert.DeserializeObject(Data);
        }
    }
}