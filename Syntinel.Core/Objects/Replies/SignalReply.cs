using System;

namespace Syntinel.Core
{
    public class SignalReply
    {
        public StatusCode StatusCode { get; set; }
        public string Id { get; set; }
        public DateTime Time { get; set; }
        public SignalStatus[] Results { get; set; }
    }

    public class SignalStatus
    {
        public StatusCode Code { get; set; }
        public string Channel { get; set; }
        public string Type { get; set; }
        public string Message { get; set; }
    }
}
