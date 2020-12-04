using System;
using System.Threading.Tasks;
using Newtonsoft.Json;

using Amazon;
using Amazon.StepFunctions;
using Amazon.StepFunctions.Model;

using Syntinel.Core;
using Syntinel.Aws;

public class AWSStepFunctionResolver : IResolver
{
    class Config
    {
        [JsonProperty(PropertyName = "arn")]
        public string Arn { get; set; }

        [JsonProperty(PropertyName = "region")]
        public string Region { get; set; }
    }

    class ExecutionArn
    {
        private string _arn;
        private string[] _parts;

        public string Arn { get { return _arn; } set { _arn = value; _parts = _arn.Split(':'); } }
        public string Region { get { return _parts[3]; } }
        public string Account { get { return _parts[4]; } }
        public string FunctionName { get { return _parts[6]; } }
        public string Name { get { return _parts[7]; } }

        public ExecutionArn(string arn)
        {
            this.Arn = arn;
        }
    }

    public Status ProcessRequest(ResolverRequest request)
    {
        String cueId = request.CueId;
        Config config = JsonTools.Convert<Config>(request.Signal.Cues[cueId].Resolver.Config);
        RegionEndpoint region = RegionEndpoint.GetBySystemName(System.Environment.GetEnvironmentVariable("AWS_REGION"));
        if (!String.IsNullOrEmpty(config.Region))
            region = RegionEndpoint.GetBySystemName(config.Region);

        AmazonStepFunctionsClient client = new AmazonStepFunctionsClient(region);   // Set Region

        StartExecutionRequest exeRequest = new StartExecutionRequest
        {
            StateMachineArn = config.Arn,
            Input = JsonTools.Serialize(request)
        };

        Task<StartExecutionResponse> t = client.StartExecutionAsync(exeRequest);
        t.Wait(30000);
        StartExecutionResponse response = t.Result;

        Status status = new Status();
        status.Id = request.Id;
        status.ActionId = request.ActionId;
        if (response.HttpStatusCode == System.Net.HttpStatusCode.OK)
        {
            ExecutionArn eArn = new ExecutionArn(response.ExecutionArn);
            status.NewStatus = StatusType.SentToResolver;
            status.Message = $"Request Sent To Step Function [{eArn.FunctionName}].  Execution Name [{eArn.Name}].";
        }
        else
        {
            status.NewStatus = StatusType.Error;
            status.Message = $"Error Sending To Step Function [{config.Arn}].  {response.HttpStatusCode}";
        }

        return status;
    }
}

