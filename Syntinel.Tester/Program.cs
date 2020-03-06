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
            TextReader reader = new StreamReader(new FileStream(@"/Users/guy/Source/Syntinel.Design/samples/Resolvers/Ec2Utils-SetInstanceState.json", FileMode.Open));
            string objectString = reader.ReadToEnd();

            ResolverRequest request = JsonTools.Deserialize<ResolverRequest>(objectString);
            Console.WriteLine(JsonTools.Serialize(request, true));

            Status status = Ec2Utils.SetInstanceState(request, RegionEndpoint.USEast1);
            Console.WriteLine(JsonTools.Serialize(status, true));
        }
    }
}
