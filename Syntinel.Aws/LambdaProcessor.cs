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

        public override void SendToChannel(ChannelDbType channel, SignalDbRecord signal)
        {
            String lambdaName = $"syntinel-signal-publisher-{channel.Type}";
            Logger.Info($"Sending Signal To {channel.Type} - {channel.Name} ({lambdaName})");

            ChannelRequest request = new ChannelRequest();
            request.Id = signal.Id;
            request.Signal = signal.Signal;
            request.Channel = channel;

            CallMethod(lambdaName, JsonTools.Serialize(request));
        }

        public string CallMethod(string functionName, string json)
        {
            InvokeRequest request = new InvokeRequest
            {
                FunctionName = functionName,
                InvocationType = "Event",
                LogType = "Tail",
                Payload = json
            };

            Task<InvokeResponse> t = client.InvokeAsync(request);
            t.Wait(30000);
            InvokeResponse response = t.Result;

            MemoryStream ps = response.Payload;
            StreamReader reader = new StreamReader(ps);
            string payload = reader.ReadToEnd();

            Console.WriteLine(">>> Calling Function : " + functionName);
            Console.WriteLine(payload);
            Console.WriteLine();
            Console.WriteLine(">>> Status Code : " + response.StatusCode);

            if (!String.IsNullOrWhiteSpace(response.LogResult))
            {
                string logs = System.Text.Encoding.UTF8.GetString(System.Convert.FromBase64String(response.LogResult));
                Console.WriteLine(logs);
            }
            Console.WriteLine(response.FunctionError);

            return payload;
        }

    }
}
