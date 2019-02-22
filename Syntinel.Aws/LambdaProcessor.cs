using System;
using System.Threading.Tasks;
using System.IO;
using Syntinel.Core;

using Amazon;
using Amazon.Lambda;
using Amazon.Lambda.Model;

namespace Syntinel.Aws
{
    public class LambdaProcessor : Processor
    {
        public AmazonLambdaClient client = new AmazonLambdaClient(RegionEndpoint.USEast1);

        public LambdaProcessor(IDatabaseEngine engine, ILogger logger = null) : base (engine, logger)
        {
        }

        public override SignalStatus SendToChannel(ChannelDbType channel, SignalDbRecord signal)
        {
            SignalStatus status = new SignalStatus
            {
                Channel = channel.Name,
                Type = channel.Type,
                Code = StatusCode.Success,
                Message = "Success"
            };

            try
            {
                String lambdaName = $"syntinel-signal-publisher-{channel.Type}";
                Logger.Info($"Sending Signal To {channel.Type} - {channel.Name} ({lambdaName})");

                ChannelRequest request = new ChannelRequest
                {
                    Id = signal.Id,
                    Signal = signal.Signal,
                    Channel = channel
                };

                InvokeResponse response = CallLambdaMethod(lambdaName, JsonTools.Serialize(request));
            } catch (Exception e)
            {
                status.Code = StatusCode.Failure;
                status.Message = e.Message;
            }

            return status;
        }

        public InvokeResponse CallLambdaMethod(string functionName, string json, bool waitForReply = false)
        {
            string invocationType = waitForReply ? "RequestResponse" : "Event";

            InvokeRequest request = new InvokeRequest
            {
                FunctionName = functionName,
                InvocationType = invocationType,
                LogType = "Tail",
                Payload = json
            };

            Task<InvokeResponse> t = client.InvokeAsync(request);
            t.Wait(30000);
            InvokeResponse response = t.Result;

            return response;
        }

        public static string GetPayload(InvokeResponse response)
        {
            MemoryStream ps = response.Payload;
            StreamReader reader = new StreamReader(ps);
            string payload = reader.ReadToEnd();
            return payload;
        }

        public static string GetLogs(InvokeResponse response)
        {
            string logs = null;
            if (!String.IsNullOrWhiteSpace(response.LogResult))
            {
                logs = System.Text.Encoding.UTF8.GetString(System.Convert.FromBase64String(response.LogResult));
            }

            return logs;
        }
    }
}
