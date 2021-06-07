using System;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.IO;
using System.Web;
using System.Collections.Generic;

using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json.Converters;


namespace Syntinel.Core
{
    public class SmtpConfig
    {
        [JsonProperty(PropertyName = "from", NullValueHandling = NullValueHandling.Ignore)]
        public string From { get; set; }

        [JsonProperty(PropertyName = "fromDisplayName", NullValueHandling = NullValueHandling.Ignore)]
        public string FromDisplayName { get; set; } = "Syntinel";

        [JsonProperty(PropertyName = "subject", NullValueHandling = NullValueHandling.Ignore)]
        public string Subject { get; set; }

        [JsonProperty(PropertyName = "html", NullValueHandling = NullValueHandling.Ignore)]
        public bool IsHtml { get; set; }


        [JsonProperty(PropertyName = "server", NullValueHandling = NullValueHandling.Ignore)]
        public string Server { get; set; }

        [JsonProperty(PropertyName = "port", NullValueHandling = NullValueHandling.Ignore)]
        public int Port { get; set; }

        [JsonProperty(PropertyName = "username", NullValueHandling = NullValueHandling.Ignore)]
        public string Username { get; set; }

        [JsonProperty(PropertyName = "password", NullValueHandling = NullValueHandling.Ignore)]
        public string Password { get; set; }

        [JsonProperty(PropertyName = "deliveryMethod")]
        [JsonConverter(typeof(StringEnumConverter))]
        public SmtpDeliveryMethod DeliveryMethod { get; set; } = SmtpDeliveryMethod.Network;

        [JsonProperty(PropertyName = "ssl", NullValueHandling = NullValueHandling.Ignore)]
        public bool EnableSSL { get; set; }

        [JsonProperty(PropertyName = "useDefaultCreds", NullValueHandling = NullValueHandling.Ignore)]
        public bool UseDefaultCreds { get; set; }
    }

    public class Smtp
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
            SmtpConfig config = JsonTools.Convert<SmtpConfig>(request?.Channel?.Config);
            String message = "<h1>Hello World</h1><br/><h2>Hello Guy</h2>";
            Signal signal = request.Signal;

            // Setup Smtp Client
            SmtpClient smtp = new SmtpClient
            {
                Host = config.Server,
                Port = config.Port,
                EnableSsl = config.EnableSSL,
                DeliveryMethod = config.DeliveryMethod,
                UseDefaultCredentials = config.UseDefaultCreds
            };

            if (config.UseDefaultCreds)
                smtp.Credentials = System.Net.CredentialCache.DefaultNetworkCredentials;
            else
            {
                string user = String.IsNullOrWhiteSpace(config.Username) ? config.From : config.Username;
                smtp.Credentials = new NetworkCredential(user, config.Password);
            }

            using (MailMessage email = new MailMessage())
            {
                email.Subject = config.Subject;
                email.Body = message;
                email.IsBodyHtml = config.IsHtml;
                email.From = new MailAddress(config.From, config.FromDisplayName);

                // Add Recipients From Target Field, Split On Semi-Colons
                string[] recipents = request.Channel.Target.Split(';', StringSplitOptions.RemoveEmptyEntries);
                foreach (string recipient in recipents)
                {
                    string to = recipient.Trim();
                    email.To.Add(to);
                }

                smtp.Send(email);
                Console.WriteLine("Message Sent");

            }
        }


        public static Cue CreateCue(Dictionary<string, object> reply)
        {
            Cue cue = new Cue();

            string signalId = (string)reply["signalId"];
            string cueId = (string)reply["cueId"];

            cue.Id = signalId;
            cue.CueId = cueId;

            foreach (string key in reply.Keys)
            {
                if (key != "signalId" && key != "cueId")
                {
                    MultiValueVariable variable = new MultiValueVariable();
                    variable.Name = key;
                    // TODO: Might have to split value by semicolons for multi-value field types.
                    variable.Values.Add((string)reply[key]);
                    cue.Variables.Add(variable);
                }
            }

            return cue;
        }
    }
}
