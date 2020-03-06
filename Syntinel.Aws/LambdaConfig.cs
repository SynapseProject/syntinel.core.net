using System;
using Amazon;

namespace Syntinel.Aws
{
    public class LambdaConfig
    {
        public RegionEndpoint Region { get; internal set; }
        public string RegionName
        {
            get { return Region.DisplayName; }
            set
            {
                try { Region = RegionEndpoint.GetBySystemName(value); }
                catch { Region = RegionEndpoint.USEast1; }
            }
        }

        // Default Constructor
        public LambdaConfig()
        {
            RegionName = System.Environment.GetEnvironmentVariable("AWS_REGION");
        }
    }
}
