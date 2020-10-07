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
        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }

        [JsonProperty(PropertyName = "region")]
        public string Region { get; set; }
    }

    public Status ProcessRequest(ResolverRequest request)
    {
        String cueId = request.CueId;
        Config config = JsonTools.Convert<Config>(request.Signal.Cues[cueId].Resolver.Config);
        RegionEndpoint region = RegionEndpoint.GetBySystemName(System.Environment.GetEnvironmentVariable("AWS_REGION"));
        if (!String.IsNullOrEmpty(config.Region))
            region = RegionEndpoint.GetBySystemName(config.Region);

        AmazonLambdaClient client = new AmazonLambdaClient(region);   // Set Region

        InvokeResponse response = AWSUtilities.CallLambdaMethod(client, config.Name, JsonTools.Serialize(request));

        Status status = new Status();
        status.Id = request.Id;
        status.ActionId = request.ActionId;
        status.NewStatus = StatusType.InProgress;
        status.Message = $"Request Sent To Lambda Function [{config.Name}].";

        return status;
    }
}

