using System;
using System.IO;

using Syntinel.Core;

using Amazon.Lambda.Serialization.Json;

namespace Syntinel.Tester
{
    class Program
    {
        static void Main(string[] args)
        {
            FileStream requestStream = new FileStream(@"/Users/guy/Source/Syntinel.Design/mockups/Status-Request.json", FileMode.Open);
            FileStream replyStream = new FileStream(@"/Users/guy/Source/Syntinel.Design/mockups/Status-Reply.json", FileMode.Open);

            JsonSerializer serializer = new JsonSerializer();
            Status request = serializer.Deserialize<Status>(requestStream);
            StatusReply reply = serializer.Deserialize<StatusReply>(replyStream);


            MemoryStream outStream = new MemoryStream();
            serializer.Serialize<Status>(request, outStream);
            outStream.Flush();
            outStream.Position = 0;
            StreamReader sr = new StreamReader(outStream);
            string outString = sr.ReadToEnd();
            Console.WriteLine("Request");
            Console.WriteLine(outString);
            Console.WriteLine("==============");

            outStream = new MemoryStream();
            serializer.Serialize<StatusReply>(reply, outStream);
            outStream.Flush();
            outStream.Position = 0;
            sr = new StreamReader(outStream);
            outString = sr.ReadToEnd();
            Console.WriteLine("Reply");
            Console.WriteLine(outString);
            Console.WriteLine("==============");
        }
    }
}
