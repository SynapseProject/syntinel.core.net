using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Syntinel.Core;

using Amazon;
using Amazon.EC2;
using Amazon.EC2.Model;

namespace Syntinel.Aws.Resolvers
{
    public class Ec2Utils
    {

        public static Core.Status SetInstanceState(ResolverRequest request, int timeout = 30000)
        {
            Core.Status status = new Core.Status();

            List<string> instances = (List<string>)request.Config["instances"];
            string action = null;
            foreach (CueVariable var in request.Variables)
                if (var.Name == "action")
                    action = var.Values[0];

            if (String.IsNullOrWhiteSpace(action))
                throw new Exception("Required Variable [action] Was Not Found.");

            Ec2InstanceState state = Enum.Parse<Ec2InstanceState>(action, true);
            List<InstanceStatus> states = SetInstanceState(instances, state, timeout);

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

        public static List<InstanceStatus> SetInstanceState(List<string> instances, Ec2InstanceState state, int timeout = 30000)
        {
            AmazonEC2Client client = new AmazonEC2Client(RegionEndpoint.USEast1);

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
}
