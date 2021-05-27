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
            {
                // If only one Cue is present in the signal message, use that as the default.
                if (request.Signal.Cues.Count == 1)
                {
                    foreach (string key in request.Signal.Cues.Keys)
                    {
                        defaultCue = key;
                        break;
                    }
                } else
                    throw new Exception("DefaultCue could not be determined.");
            }
            CueOption defaultCueOption = request.Signal.Cues[defaultCue];
            string id = defaultCueOption.DefaultId;
            string value = defaultCueOption.DefaultValue;
            if (string.IsNullOrWhiteSpace(id))
            {
                // Get Default Id (and value) from action if only one exists
                if (defaultCueOption.Actions != null && defaultCueOption.Actions.Count == 1)
                {
                    id = defaultCueOption.Actions[0].Id;
                    if (string.IsNullOrWhiteSpace(value))
                        value = defaultCueOption.Actions[0].DefaultValue;

                } else
                    throw new Exception("DefaultId could not be determined.");
            }
            List<string> values = new List<string>();
            values.Add(value);

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
