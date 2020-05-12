using System;
using Amazon;

namespace Syntinel.Aws
{
    public class LambdaConfig
    {
        // Global Configurations
        public RegionEndpoint Region { get; internal set; }
        public string RegionName
        {
            get { return Region.DisplayName; }
            set { Region = RegionEndpoint.GetBySystemName(value); }
        }
        public string DefaultReporter { get; set; }

        // Database Configurations
        public string SignalsTable { get; set; }
        public string ReportersTable { get; set; }
        public string ChannelsTable { get; set; }
        public string RouterTable { get; set; }

        // Lambda Configurations
        public string ChannelPublisherPrefix { get; set; }
        public string ChannelSubscriberPrefix { get; set; }
        public string ResolverPrefix { get; set; }


        // Default Constructor
        public LambdaConfig()
        {
            RegionName = GetVariable("AWS_REGION", RegionEndpoint.USEast1.DisplayName);

            DefaultReporter = GetVariable("Syntinel_DefaultReporter", "_default");

            SignalsTable = GetVariable("Syntinel_SignalsTableName", "syntinel-signals");
            ReportersTable = GetVariable("Syntinel_ReportersTableName", "syntinel-reporters");
            ChannelsTable = GetVariable("Syntinel_ChannelsTableName", "syntinel-channels");
            RouterTable = GetVariable("Syntinel_RouterTableName", "syntinel-router");

            ChannelPublisherPrefix = GetVariable("Syntinel_ChannelPublisherPrefix", "syntinel-signal-publisher");
            ChannelSubscriberPrefix = GetVariable("Syntinel_ChannelSubscriberPrefix", "syntinel-cue-subscriber");
            ResolverPrefix = GetVariable("Syntinel_ResolverPrefix", "syntinel-resolver");
        }

        private static string GetVariable(string variable, string defaultValue = null)
        {
            string value = System.Environment.GetEnvironmentVariable(variable);
            if (String.IsNullOrWhiteSpace(value))
                return defaultValue;
            else
                return value;
        }
    }
}
