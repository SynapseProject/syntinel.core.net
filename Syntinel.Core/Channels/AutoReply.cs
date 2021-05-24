using System;
using System.Collections.Generic;

namespace Syntinel.Core
{
    public class AutoReply
    {
        public static Cue CreateCue(string id, ChannelDbRecord channel, Signal signal)
        {
            ChannelRequest request = new ChannelRequest
            {
                Id = id,
                Channel = channel,
                Signal = signal
            };

            return CreateCue(request);
        }

        public static Cue CreateCue(ChannelRequest request)
        {
            string defaultCue = request.Signal.DefaultCue;
            if (string.IsNullOrWhiteSpace(defaultCue))
                throw new Exception("DefaultCue not specified in Signal.");
            CueOption defaultCueOption = request.Signal.Cues[defaultCue];
            string id = defaultCueOption.DefaultId;
            if (string.IsNullOrWhiteSpace(id))
                throw new Exception("DefaultId not specified in Signal.");
            List<string> values = new List<string>();
            values.Add(defaultCueOption.DefaultValue);

            Cue cue = new Cue();
            cue.Id = request.Id;
            cue.CueId = defaultCue;

            MultiValueVariable var = new MultiValueVariable();
            var.Name = id;
            var.Values = values;
            cue.Variables.Add(var);

            return cue;

        }

    }
}
