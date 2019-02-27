using System;
using System.IO;
using System.Threading.Tasks;

using Amazon;
using Amazon.Lambda;
using Amazon.Lambda.Model;

namespace Syntinel.Aws
{
    public class AWSUtilities
    {
        public static InvokeResponse CallLambdaMethod(AmazonLambdaClient client, string functionName, string json, bool waitForReply = false)
        {
            string invocationType = waitForReply ? "RequestResponse" : "Event";

            InvokeRequest request = new InvokeRequest
            {
                FunctionName = functionName,
                InvocationType = invocationType,
                LogType = "Tail",
                Payload = json
            };

            Task<InvokeResponse> t = client.InvokeAsync(request);
            t.Wait(30000);
            InvokeResponse response = t.Result;

            return response;
        }

        public static string GetPayload(InvokeResponse response)
        {
            MemoryStream ps = response.Payload;
            StreamReader reader = new StreamReader(ps);
            string payload = reader.ReadToEnd();
            return payload;
        }

        public static string GetLogs(InvokeResponse response)
        {
            string logs = null;
            if (!String.IsNullOrWhiteSpace(response.LogResult))
            {
                logs = System.Text.Encoding.UTF8.GetString(System.Convert.FromBase64String(response.LogResult));
            }

            return logs;
        }
    }
}
