using System;
using System.Net;
using System.Text;
using System.IO;

namespace Syntinel.Core
{
    public class SlackPublisher
    {
        public string Id { get; internal set; }
        public ChannelDbType Channel { get; internal set; }
        public Signal Signal { get; set; }

        public void PublishSlackMessage(string id, ChannelDbType channel, Signal signal)
        {
            ChannelRequest request = new ChannelRequest();
            request.Id = id;
            request.Channel = channel;
            request.Signal = signal;

            PublishSlackMessage(request);
        }

        public void PublishSlackMessage(ChannelRequest request)
        {
            SlackMessage message = new SlackMessage();
            Signal signal = request.Signal;

            String webHook = request?.Channel?.Target;
            if (webHook != null)
            {
                string mainTitle = $"{signal.Name} : {signal.Description}";
                message.Text = mainTitle;

                foreach (string key in signal.Cues.Keys)
                {
                    CueOption cue = signal.Cues[key];
                    SlackAttachment attachment = CreateSlackAttachment(request.Id, key, cue);
                    message.Attachments.Add(attachment);
                }



                Console.WriteLine(JsonTools.Serialize(message, true));
                SendMessage(webHook, message);
            }
            else
                throw new Exception("No Target Information Was Provided.");
        }

        public void SendMessage(string webHook, SlackMessage message)
        {
            string body = JsonTools.Serialize(message);
            HttpWebRequest request =(HttpWebRequest)WebRequest.Create(webHook);
            request.Method = "POST";
            request.ContentType = "application/json";
            request.GetRequestStream().Write(Encoding.ASCII.GetBytes(body));

            WebResponse response = request.GetResponse();

            StreamReader reader = new StreamReader(response.GetResponseStream());
            String payload = reader.ReadToEnd();
            reader.Close();
            Console.WriteLine(payload);


        }

        public SlackAttachment CreateSlackAttachment(string signalId, string cueId, CueOption cue)
        {
            SlackAttachment attachment = new SlackAttachment();

            string cueTitle = $"{cue.Name} : {cue.Description}";
            attachment.Text = cueTitle;
            attachment.CallbackId = $"{signalId}|{cueId}";

            foreach (SignalVariable action in cue.Actions)
            {
                SlackAction slackAction = CreateSlackAction(action);
                attachment.Actions.Add(slackAction);
            }


            return attachment;
        }

        public SlackAction CreateSlackAction(SignalVariable action)
        {
            SlackAction slackAction = new SlackAction();

            slackAction.Name = action.Name;
            slackAction.Value = action.DefaultValue;

            switch (action.Type)
            {
                case VariableType.Choice:
                    slackAction.Type = SlackActionType.select;
                    foreach (string key in action.Values.Keys)
                    {
                        SlackSelectOption option = new SlackSelectOption();
                        option.Text = action.Values[key];
                        option.Value = key;
                        slackAction.Options.Add(option);
                    }
                    break;

                case VariableType.Button:
                    slackAction.Type = SlackActionType.button;
                    slackAction.Text = action.Description;
                    break;
            }

            return slackAction;
        }
    }
}
