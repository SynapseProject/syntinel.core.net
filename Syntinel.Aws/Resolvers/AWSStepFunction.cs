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

        [JsonProperty(PropertyName = "useDefaultName")]
        public bool UseDefaultName { get; set; } = false;

        [JsonProperty(PropertyName = "executionName")]
        public string ExecutionName { get; set; }
    }

    class StepFunctionArn
    {
        private string _arn;
        private string[] _parts;

        public string Arn { get { return _arn; } set { _arn = value; _parts = _arn.Split(':'); } }
        public string Region { get { return _parts[3]; } }
        public string Account { get { return _parts[4]; } }
        public string Name { get { return _parts[6]; } }
        public string ExecutionName { get { return _parts[7]; } }

        public StepFunctionArn(string arn)
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
            StepFunctionArn arn = new StepFunctionArn(config.Arn);
            RegionEndpoint region = RegionEndpoint.GetBySystemName(System.Environment.GetEnvironmentVariable("AWS_REGION"));
            if (!String.IsNullOrEmpty(arn.Region))
                region = RegionEndpoint.GetBySystemName(arn.Region);

            AmazonStepFunctionsClient client = new AmazonStepFunctionsClient(region);   // Set Region

            StartExecutionRequest exeRequest = new StartExecutionRequest
            {
                StateMachineArn = config.Arn,
                Input = JsonTools.Serialize(request)
            };

            if (config.UseDefaultName == false)
            {
                if (String.IsNullOrWhiteSpace(config.ExecutionName))
                    exeRequest.Name = $"syntinel-{request.Id}-{request.ActionId}";
                else
                    exeRequest.Name = config.ExecutionName;
            }

            Task<StartExecutionResponse> t = client.StartExecutionAsync(exeRequest);
            t.Wait(30000);
            StartExecutionResponse response = t.Result;

            if (response.HttpStatusCode == System.Net.HttpStatusCode.OK)
            {
                StepFunctionArn eArn = new StepFunctionArn(response.ExecutionArn);
                status.NewStatus = StatusType.SentToResolver;
                status.Message = $"Request Sent To Step Function [{eArn.Name}].  Execution Name [{eArn.ExecutionName}].";
            }
            else
            {
                status.NewStatus = StatusType.Error;
                status.Message = $"Error Sending To Step Function [{arn.Name}].  {response.HttpStatusCode}";
            }
        }
        catch (Exception e)
        {
            status.NewStatus = StatusType.Error;
            status.Message = e.Message;
        }

        return status;
    }
}

