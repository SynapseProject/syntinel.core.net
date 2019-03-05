using System;
using System.IO;
using System.Collections.Generic;
using System.Threading;

using Syntinel.Core;
using Syntinel.Aws;
using Syntinel.Aws.Resolvers;

using Amazon;
using Amazon.EC2.Model;

//using Amazon.Lambda.Serialization.Json;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Syntinel.Tester
{
    public class Program
    {
        public static void Main(string[] args)
        {
            List<string> instances = new List<string>
            {
                "i-009cffa33db569230"
            };

            ResolverRequest request = new ResolverRequest();
            CueVariable var = new CueVariable
            {
                Name = "action"
            };
            var.Values.Add("stop");
            request.Variables = new List<CueVariable>();
            request.Variables.Add(var);

            request.Config = new Dictionary<string, object>
            {
                { "instances", instances }
            };

            Status status = Ec2Utils.SetInstanceState(request);
            Console.WriteLine(JsonTools.Serialize(status, true));
        }
    }
}
