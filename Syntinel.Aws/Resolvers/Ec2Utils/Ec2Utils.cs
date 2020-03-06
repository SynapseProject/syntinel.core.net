using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Syntinel.Core;

using Amazon;
using Amazon.EC2;
using Amazon.EC2.Model;

using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Syntinel.Aws.Resolvers
{
    public static class Ec2Utils
    {

        public static Core.Status SetInstanceState(ResolverRequest request, RegionEndpoint region, int timeout = 30000)
        {
            Core.Status status = new Core.Status();

            String configString = JsonTools.Serialize(request.Config);
            Ec2UtilsConfig config = JsonTools.Deserialize<Ec2UtilsConfig>(configString);
            string action = null;
            foreach (CueVariable var in request.Variables)
                if (var.Name == "action")
                    action = var.Values[0];

            if (String.IsNullOrWhiteSpace(action))
                throw new Exception("Required Variable [action] Was Not Found.");

            Ec2InstanceState state = Enum.Parse<Ec2InstanceState>(action, true);
            List<InstanceStatus> states = SetInstanceState(config.Instances, state, region, timeout);

            status.Id = request.Id;
            status.ActionId = request.ActionId;
            status.Data = new Dictionary<object, object>
            {
                {"result", states}
            };
            status.NewStatus = Core.StatusType.Completed;
            status.CloseSignal = true;

            return status;
        }

        public static List<InstanceStatus> SetInstanceState(List<string> instances, Ec2InstanceState state, RegionEndpoint region, int timeout = 30000)
        {
            AmazonEC2Client client = new AmazonEC2Client(region);

            switch (state)
            {
                case Ec2InstanceState.Stop:
                    StopInstancesRequest stopRequest = new StopInstancesRequest(instances);

                    Task<StopInstancesResponse> stopTask = client.StopInstancesAsync(stopRequest);
                    stopTask.Wait(timeout);
                    StopInstancesResponse stopResponse = stopTask.Result;
                    break;

                case Ec2InstanceState.Hibernate:
                    StopInstancesRequest hibernateRequest = new StopInstancesRequest(instances);
                    hibernateRequest.Hibernate = true;

                    Task<StopInstancesResponse> hibTask = client.StopInstancesAsync(hibernateRequest);
                    hibTask.Wait(timeout);
                    StopInstancesResponse hibernateResponse = hibTask.Result;
                    break;

                case Ec2InstanceState.Start:
                    StartInstancesRequest startRequest = new StartInstancesRequest(instances);

                    Task<StartInstancesResponse> startTask = client.StartInstancesAsync(startRequest);
                    startTask.Wait(timeout);
                    StartInstancesResponse startResponse = startTask.Result;
                    break;

                case Ec2InstanceState.Reboot:
                    RebootInstancesRequest rebootRequest = new RebootInstancesRequest(instances);

                    Task<RebootInstancesResponse> rebootTask = client.RebootInstancesAsync(rebootRequest);
                    rebootTask.Wait(timeout);
                    RebootInstancesResponse rebootResponse = rebootTask.Result;
                    break;

                case Ec2InstanceState.Terminate:
                    TerminateInstancesRequest terminateRequest = new TerminateInstancesRequest(instances);

                    Task<TerminateInstancesResponse> termTask = client.TerminateInstancesAsync(terminateRequest);
                    termTask.Wait(timeout);
                    TerminateInstancesResponse terminateResponse = termTask.Result;
                    break;

                default:
                    throw new Exception($"Unknown Instance Type [{state}] Received.");
            }

            return GetInstanceStatus(instances);
        }

        public static List<InstanceStatus> GetInstanceStatus(List<string> instances, int timeout = 30000)
        {
            AmazonEC2Client client = new AmazonEC2Client(RegionEndpoint.USEast1);
            DescribeInstanceStatusRequest request = new DescribeInstanceStatusRequest();
            request.InstanceIds = instances;
            request.IncludeAllInstances = true;

            Task<DescribeInstanceStatusResponse> task = client.DescribeInstanceStatusAsync(request);
            task.Wait(timeout);
            DescribeInstanceStatusResponse reply = task.Result;

            return reply.InstanceStatuses;
        }
    }

    public class Ec2UtilsConfig
    {
        [JsonProperty(PropertyName = "instances")]
        public List<string> Instances { get; set; }
    }
}
