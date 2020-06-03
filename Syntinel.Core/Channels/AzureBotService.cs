using System;
using System.Net;
using System.Collections.Generic;

namespace Syntinel.Core
{
    public class AzureBotService
    {
        public string ConversationId { get; internal set; }
        public string BearerToken { get; internal set; }
        public string BaseUrl = @"https://directline.botframework.com/v3/directline/conversations";


        public AzureBotServiceMessage Publish(string id, ChannelDbRecord channel, Signal signal)
        {
            ChannelRequest request = new ChannelRequest
            {
                Id = id,
                Channel = channel,
                Signal = signal
            };

            return Publish(request);
        }

        public AzureBotServiceMessage Publish(ChannelRequest request)
        {
            BearerToken = (String)request.Channel.Config["bearerToken"];
            ConversationId = GetConversationId();
            string target = request.Channel.Target;
            string channelType = target.Substring(target.IndexOf('@') + 1);

            AzureBotServiceMessage message = new AzureBotServiceMessage();
            message.From.Id = request.Id;
            message.Text = $"notify {target}";

            if (channelType?.ToLower() == "slack")
                message.Value = Slack.CreateSlackMessage(request);
            else if (channelType?.ToLower() == "msteams")
                message.Value = Teams.CreateAdaptiveCardMessage(request);
            else
                throw new Exception($"Unknown ChannelType [{channelType}].");

            SendMessage(message);
            return message;

        }

        public string GetConversationId()
        {
            Dictionary<string, string> headers = new Dictionary<string, string>
            {
                { "Authorization", $"Bearer {BearerToken}" }
            };

            WebResponse response = Utils.PostMessage(BaseUrl, null, headers);
            String payload = Utils.GetPayload(response);

            ConversationIdReply reply = JsonTools.Deserialize<ConversationIdReply>(payload);
            return reply.ConversationId;
        }

        public void SendMessage(AzureBotServiceMessage message)
        {
            Dictionary<string, string> headers = new Dictionary<string, string>
            {
                { "Authorization", $"Bearer {BearerToken}" }
            };

            String body = JsonTools.Serialize(message);
            String url = $"{BaseUrl}/{ConversationId}/activities";
            WebResponse response = Utils.PostMessage(url, body, headers);
        }

    }
}
