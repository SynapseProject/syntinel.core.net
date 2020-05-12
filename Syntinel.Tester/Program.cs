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
            DynamoDbEngine db = new DynamoDbEngine();
            ReporterDbRecord rec = db.Get<ReporterDbRecord>("_default");

            string[] ids = { "wu2-p3-0392", "cloudenvironment3" };
            RouterDbRecord router = db.Get<RouterDbRecord>(ids);

            rec.LoadChannels(db, router);
            Console.WriteLine($"{rec.Id} : {rec.Description}");
        }
    }
}
