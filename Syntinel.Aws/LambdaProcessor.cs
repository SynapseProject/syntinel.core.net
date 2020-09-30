using System;
using System.Threading.Tasks;
using System.IO;
using System.Collections.Generic;
using Syntinel.Core;

using Amazon;
using Amazon.Lambda;
using Amazon.Lambda.Model;

namespace Syntinel.Aws
{
    public class LambdaProcessor : Processor
    {
        public LambdaConfig Config { get; internal set; } 
        public AmazonLambdaClient Client { get; internal set; }

        public LambdaProcessor(IDatabaseEngine engine, LambdaConfig config, ILogger logger = null) : base (engine, logger)
        {
            this.Config = config;
            this.Client = new AmazonLambdaClient(Config.Region);
        }

        public override SignalStatus SendToChannel(ChannelDbRecord channel, SignalDbRecord signal)
        {
            SignalStatus status = new SignalStatus
            {
                ChannelType = channel.Type,
                Code = StatusCode.Success,
                Message = "Success"
            };

            try
            {
                String lambdaName = $"{Config.ChannelPublisherPrefix}-{channel.Type}";
                Logger.Info($"Sending Signal To {channel.Type} - {channel.Name} ({lambdaName})");

                ChannelRequest request = new ChannelRequest
                {
                    Id = signal.Id,
                    Signal = signal.Signal,
                    Channel = channel
                };
                string requestStr = JsonTools.Serialize(request);
                Logger.Info(requestStr);
                InvokeResponse response = AWSUtilities.CallLambdaMethod(Client, lambdaName, requestStr);
            } 
            catch (Exception e)
            {
                status.Code = StatusCode.Failure;
                status.Message = e.Message;
            }

            return status;
        }

        public override void SendToCueProcessor(CueRequest request)
        {
            string lambdaName = Config.ProcessCueLambda;
            Logger.Info($"Sending Cue To {lambdaName}");
            string requestStr = JsonTools.Serialize(request);
            Logger.Info(requestStr);
            InvokeResponse response = AWSUtilities.CallLambdaMethod(Client, lambdaName, requestStr);
        }
    }
}
