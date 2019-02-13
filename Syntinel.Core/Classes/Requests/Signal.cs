using System;
using System.Collections.Generic;

namespace Syntinel.Core
{
    public class Signal
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public int MaxReplies { get; set; }
        public Dictionary<string, CueOption> Cues { get; set; }
        public string DefaultCue { get; set; }
        public int DefaultCueTimeout { get; set; }
    }

    public class CueOption
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public Resolver Resolver { get; set; }
        public SignalVariable[] Inputs { get; set; }
        public SignalVariable[] Actions { get; set; }
        public string DefaultAction { get; set; }
    }

    public class Resolver
    {
        public string Name { get; set; }
        public Dictionary<object, object> Config { get; set; }
    }

    public class SignalVariable
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public VariableType Type { get; set; }
        public KeyValuePair<string, string>[] Values { get; set; }
    }

}
