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
        public static LambdaConfig config = new LambdaConfig();
        public AmazonLambdaClient client = new AmazonLambdaClient(config.Region);

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

                InvokeResponse response = AWSUtilities.CallLambdaMethod(client, lambdaName, JsonTools.Serialize(request));
            } 
            catch (Exception e)
            {
                status.Code = StatusCode.Failure;
                status.Message = e.Message;
            }

            return status;
        }

        public override void SendToResolver(Resolver resolver, ResolverRequest request)
        {
            String lambdaName = $"syntinel-resolver-{resolver.Name}";
            Logger.Info($"Sending Requst To {resolver.Name} ({lambdaName})");
            InvokeResponse response = AWSUtilities.CallLambdaMethod(client, lambdaName, JsonTools.Serialize(request));
        }
    }
}
