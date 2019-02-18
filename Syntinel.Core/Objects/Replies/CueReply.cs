using System;

namespace Syntinel.Core
{
    public class CueReply
    {
        public StatusCode StatusCode { get; set; }
        public string Id { get; set; }
        public string ActionId { get; set; }
        public DateTime Time { get; set; }
    }
}
