﻿using System;
using System.Web;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.IO;

namespace Syntinel.Core
{
    public class Slack
    {
        public static SlackMessage Publish(string id, ChannelDbRecord channel, Signal signal)
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
                SendMessage(webHook, message);
                return message;
            }
            else
                throw new Exception("No Target Information Was Provided.");
        }

        public static Cue CreateCue(SlackReply reply)
        {
            if (reply.BodyJson != null)
            {
                // Reply Came Directly From Slack.  Decode the payload
                string body = reply.BodyJson;
                if (body.StartsWith("payload=", StringComparison.OrdinalIgnoreCase))
                {
                    body = body.Replace("payload=", "", StringComparison.OrdinalIgnoreCase);
                    body = body.Replace('+', ' ');
                    body = HttpUtility.UrlDecode(body);
                    reply.Payload = JsonTools.Deserialize<SlackPayload>(body);
                }
            }

            SlackPayload payload = reply?.Payload;
            if (payload == null)
                throw new Exception("Slack Reply Payload Not Found.");

            string callbackId = payload.CallbackId;
            string signalId = callbackId.Split('|')[0];
            string cueId = HttpUtility.HtmlDecode(callbackId.Split('|')[1]);

            Cue cue = new Cue
            {
                Id = signalId,
                CueId = cueId,
                Payload = payload
            };

            foreach (SlackReplyAction actionReply in payload.Actions)
            {
                MultiValueVariable actionVariable = new MultiValueVariable();
                actionVariable.Name = actionReply.Name;

                if (actionReply.Type == "select")
                    foreach (Dictionary<string, string> option in actionReply.SelectedOptions)
                        actionVariable.Values.Add(option["value"]);
                else if (actionReply.Type == "button")
                    actionVariable.Values.Add(actionReply.Value);

                cue.Variables.Add(actionVariable);
            }

            return cue;
        }

        public static string FormatResponse(Cue cue, CueReply reply)
        {
            SignalDbRecord signal = reply.SignalDbRecord;
            StringBuilder sb = new StringBuilder();

            string signalTitle = $"{signal?.Signal?.Name} - {signal?.Signal?.Description}";
            if (String.IsNullOrEmpty(signal?.Signal?.Name))
                signalTitle = signal?.Signal?.Description;
            else if (String.IsNullOrEmpty(signal?.Signal?.Description))
                signalTitle = signal?.Signal?.Name;
            else if (String.IsNullOrEmpty(signal?.Signal?.Name) && String.IsNullOrEmpty(signal?.Signal?.Description))
                signalTitle = null;

            if (!String.IsNullOrWhiteSpace(signalTitle))
                sb.AppendLine($"Action Received From [{cue.Payload?.User?.Name}] To Signal [{signalTitle}]");
            else
                sb.AppendLine($"Action Received From [{cue.Payload?.User?.Name}]");
            sb.AppendLine($"Signal Id : {reply.Id}");
            sb.AppendLine($"Action Id : {reply.ActionId}");
            foreach (MultiValueVariable variable in cue.Variables)
                sb.AppendLine($"{variable.Name} = {String.Join(',', variable.Values)}");

            return sb.ToString();
        }

        public static void SendResponse(string responseUrl, string message)
        {
            string body = "{ 'text': '" + message + "' }";
            Utils.PostMessage(responseUrl, body);
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

            string mainTitle = $"{signal.Name}\n{signal.Description}";
            if (String.IsNullOrEmpty(signal.Name))
                mainTitle = signal.Description;
            else if (String.IsNullOrEmpty(signal.Description))
                mainTitle = signal.Name;

            if (!String.IsNullOrWhiteSpace(mainTitle))
                message.Text = mainTitle;
            else if (signal.Cues == null)
                message.Text = " ";

            if (signal.IncludeId)
            {
                if (String.IsNullOrWhiteSpace(message.Text))
                    message.Text = $"Id : {request.Id}";
                else
                    message.Text += $"\n(Id : {request.Id})";
            }

            if (signal.Cues != null)
            {
                foreach (string key in signal.Cues.Keys)
                {
                    CueOption cue = signal.Cues[key];
                    SlackAttachment attachment = CreateSlackAttachment(request.Id, key, cue);
                    message.Attachments.Add(attachment);
                }
            }

            return message;
        }

        public static SlackAttachment CreateSlackAttachment(string signalId, string cueId, CueOption cue)
        {
            SlackAttachment attachment = new SlackAttachment();

            string cueTitle = $"{cue.Name}\n{cue.Description}";
            if (String.IsNullOrEmpty(cue.Name))
                cueTitle = cue.Description;
            else if (String.IsNullOrEmpty(cue.Description))
                cueTitle = cue.Name;
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

            slackAction.Name = action.Id;
            slackAction.Value = action.DefaultValue;

            switch (action.Type)
            {
                case VariableType.choice:
                    slackAction.Type = SlackActionType.select;
                    slackAction.Options = new List<SlackSelectOption>();
                    foreach (string key in action.Values.Keys)
                    {
                        SlackSelectOption option = new SlackSelectOption();
                        option.Text = action.Values[key];
                        option.Value = key;
                        slackAction.Options.Add(option);
                    }
                    break;

                case VariableType.button:
                    slackAction.Type = SlackActionType.button;
                    slackAction.Text = action.Text;
                    break;
            }

            return slackAction;
        }
    }
}
