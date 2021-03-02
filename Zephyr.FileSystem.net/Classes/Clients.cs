using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Amazon;
using Amazon.Runtime;

namespace Zephyr.Filesystem
{
    /// <summary>
    /// A container class which holds any connection "clients" an individual implementation of ZephyrFile and/or ZephyrDirectory might
    /// need to accomplish its tasks.  This class is primarily used in the Utilities classes, when the implementation type is determined
    /// by the URL / Filename passed in.
    /// </summary>
    public class Clients
    {
        /// <summary>
        /// A wrapper client around the AmazonS3Client class, used to perform actions against Amazon S3 buckets.
        /// </summary>
        public AwsClient aws { get; set; } = null;

        /// <summary>
        /// Wrapper method around the AwsClient constructor.
        /// </summary>
        /// <param name="endpoint">The region to connect.</param>
        /// <returns></returns>
        public AwsClient AwsInitialize(RegionEndpoint endpoint = null)
        {
            aws = new AwsClient(endpoint);
            return aws;
        }

        /// <summary>
        /// Wrapper method around the AwsClient constructor.
        /// </summary>
        /// <param name="creds">The AWS Credentials to use.</param>
        /// <param name="endpoint">The region to connect.</param>
        /// <returns></returns>
        public AwsClient AwsInitialize(AWSCredentials creds, RegionEndpoint endpoint = null)
        {
            aws = new AwsClient(creds, endpoint);
            return aws;
        }

        /// <summary>
        /// Wrapper method around the AwsClient constructor.
        /// </summary>
        /// <param name="accessKey">The AWS Access Key Id</param>
        /// <param name="secretAccessKey">The AWS Secret Access Key</param>
        /// <param name="endpoint">The region to connect.</param>
        /// <returns></returns>
        public AwsClient AwsInitialize(string accessKey, string secretAccessKey, RegionEndpoint endpoint = null)
        {
            aws = new AwsClient(accessKey, secretAccessKey, endpoint);
            return aws;
        }

        /// <summary>
        /// Wrapper method around the AwsClient constructor.
        /// </summary>
        /// <param name="accessKey">The AWS Access Key Id</param>
        /// <param name="secretAccessKey">The AWS Secret Access Key</param>
        /// <param name="sessionToken">The AWS Session Token.</param>
        /// <param name="endpoint">The region to connect.</param>
        /// <returns></returns>
        public AwsClient AwsInitialize(string accessKey, string secretAccessKey, string sessionToken, RegionEndpoint endpoint = null)
        {
            aws = new AwsClient(accessKey, secretAccessKey, sessionToken, endpoint);
            return aws;
        }

        /// <summary>
        /// Wrapper method around the AwsClient constructor.
        /// </summary>
        /// <param name="profileName">The name of the AWS profile to get credentials from.</param>
        /// <param name="endpoint">The region to connect.</param>
        /// <returns></returns>
        public AwsClient AwsInitialize(string profileName, RegionEndpoint endpoint = null)
        {
            aws = new AwsClient(profileName, endpoint);
            return aws;
        }

    }
}
