using System;
using Newtonsoft.Json;

using Amazon;
using Amazon.Lambda;
using Amazon.Lambda.Model;

using Syntinel.Core;
using Syntinel.Aws;

public class AWSLambdaResolver : IResolver
{
    class Config
    {
        [JsonProperty(PropertyName = "arn")]
        public string Arn { get; set; }
    }

    class LambdaArn
    {
        private string _arn;
        private string[] _parts;

        public string Arn { get { return _arn; } set { _arn = value; _parts = _arn.Split(':'); } }
        public string Region { get { return _parts[3]; } }
        public string Account { get { return _parts[4]; } }
        public string Name { get { return _parts[6]; } }

        public LambdaArn(string arn)
        {
            this.Arn = arn;
            if (_parts.Length < 7)
                throw new Exception($"Invalid ARN Provided [{arn}].");
        }
    }

    public Status ProcessRequest(ResolverRequest request)
    {
        Status status = new Status();
        status.Id = request.Id;
        status.ActionId = request.ActionId;

        try
        {
            String cueId = request.CueId;
            Config config = JsonTools.Convert<Config>(request.Signal.Cues[cueId].Resolver.Config);
            LambdaArn arn = new LambdaArn(config.Arn);
            RegionEndpoint region = RegionEndpoint.GetBySystemName(System.Environment.GetEnvironmentVariable("AWS_REGION"));
            if (!String.IsNullOrEmpty(arn.Region))
                region = RegionEndpoint.GetBySystemName(arn.Region);

            AmazonLambdaClient client = new AmazonLambdaClient(region);   // Set Region

            InvokeResponse response = AWSUtilities.CallLambdaMethod(client, arn.Arn, JsonTools.Serialize(request));
            status.NewStatus = StatusType.SentToResolver;
            status.Message = $"Request Sent To Lambda Function [{arn.Name}].";
        }
        catch (Exception e)
        {
            status.NewStatus = StatusType.Error;
            status.Message = e.Message;
        }

        return status;
    }
}

