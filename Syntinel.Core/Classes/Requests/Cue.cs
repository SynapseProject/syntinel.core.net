using System;

namespace Syntinel.Core
{
    public class Cue
    {
        public string Id { get; set; }
        public string CueId { get; set; }
        public CueVariable[] Variables { get; set; }
    }

    public class CueVariable
    {
        public String Name { get; set; }
        public String[] Values { get; set; }
    }
}
