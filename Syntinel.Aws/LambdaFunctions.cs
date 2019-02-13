using System;

using Syntinel.Core;
using Amazon.Lambda.Serialization;
using Amazon.Lambda.Core;

// Allows Lambda Function's JSON Input to be converted into a .NET class
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.Json.JsonSerializer))]

namespace Syntinel.Aws
{
    public class LambdaFunctions
    {
        public string Hello(string input, ILambdaLogger ctx)
        {
            return "Hello From Syntinel Core!";
        }

    }
}
