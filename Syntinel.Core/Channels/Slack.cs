using System;
using System.Net;
using System.Text;
using System.IO;

namespace Syntinel.Core
{
    public class Slack
    {
        public static SlackMessage Publish(string id, ChannelDbType channel, Signal signal)
        {
            ChannelRequest request = new ChannelRequest
            {
                Id = id,
                Channel = channel,
                Signal = signal
            };

            return Publish(request);
        }

        public static SlackMessage Publish(ChannelRequest request)
        {
            SlackMessage message = CreateSlackMessage(request);

            String webHook = request?.Channel?.Target;
            if (webHook != null)
            {
                // TODO : Re-enable after done testing.
                //SendMessage(webHook, message);
                return message;
            }
            else
                throw new Exception("No Target Information Was Provided.");
        }

        public static void SendMessage(string webHook, SlackMessage message)
        {
            string body = JsonTools.Serialize(message);
            Utils.PostMessage(webHook, body);
        }

        public static SlackMessage CreateSlackMessage(ChannelRequest request)
        {
            SlackMessage message = new SlackMessage();
            Signal signal = request.Signal;

            String webHook = request?.Channel?.Target;

            string mainTitle = $"{signal.Name} : {signal.Description}";
            message.Text = mainTitle;

            foreach (string key in signal.Cues.Keys)
            {
                CueOption cue = signal.Cues[key];
                SlackAttachment attachment = CreateSlackAttachment(request.Id, key, cue);
                message.Attachments.Add(attachment);
            }

            return message;
        }

        public static SlackAttachment CreateSlackAttachment(string signalId, string cueId, CueOption cue)
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

        public static SlackAction CreateSlackAction(SignalVariable action)
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
