using System;
using System.Collections.Generic;

namespace Syntinel.Core
{
    public class Status
    {
        public string Id { get; set; }
        public string ActionId { get; set; }
        public StatusType NewStatus { get; set; }
        public bool CloseSignal { get; set; }
        public bool IsValidReply { get; set; }
        public Dictionary<object, object> Data { get; set; }
    }
}
