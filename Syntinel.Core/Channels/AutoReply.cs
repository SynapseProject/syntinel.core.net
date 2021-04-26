using System;
using System.Collections.Generic;

namespace Syntinel.Core
{
    public class AutoReply
    {
        public static void Publish(string id, ChannelDbRecord channel, Signal signal)
        {
            ChannelRequest request = new ChannelRequest
            {
                Id = id,
                Channel = channel,
                Signal = signal
            };

            Publish(request);
        }

        public static void Publish(ChannelRequest request)
        {
            string defaultCue = request.Signal.DefaultCue;
            CueOption defaultCueOption = request.Signal.Cues[defaultCue];
            string id = defaultCueOption.DefaultId;
            List<string> values = new List<string>();
            values.Add(defaultCueOption.DefaultValue);

            Cue cue = new Cue();
            cue.Id = request.Id;
            cue.CueId = defaultCue;

            MultiValueVariable var = new MultiValueVariable();
            var.Name = id;
            var.Values = values;
            cue.Variables.Add(var);


        }

    }
}
