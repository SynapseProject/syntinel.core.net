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

        public override SignalStatus PublishSignal(ChannelDbType channel, SignalDbRecord signal)
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
            } catch (Exception e)
            {
                status.Code = StatusCode.Failure;
                status.Message = e.Message;
            }

            return status;
        }
    }
}
