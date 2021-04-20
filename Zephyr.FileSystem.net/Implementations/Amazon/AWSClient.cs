using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Amazon;
using Amazon.S3;
using Amazon.Runtime;
using Amazon.Runtime.CredentialManagement;

namespace Zephyr.Filesystem
{
    /// <summary>
    /// A wrapper class around an AmazonS3Client object, used to connect to Amazon S3 storage and work with files and directories.
    /// </summary>
    public class AwsClient
    {
        public ServerSideEncryptionMethod SSEMethod = ServerSideEncryptionMethod.None;
        public S3StorageClass StorageClass = S3StorageClass.Standard;

        /// <summary>
        /// The AmazonS3Client used to connect to Amazon S3 Storage
        /// </summary>
        internal AmazonS3Client Client { get; set; }

        /// <summary>
        /// Constructor using implicit Credentails from config or environment variables.
        /// </summary>
        /// <param name="endpoint">The region to connect.</param>
        public AwsClient(RegionEndpoint endpoint = null) { Initialize(endpoint); }

        /// <summary>
        /// Constructor using AWSCredentials object.
        /// </summary>
        /// <param name="creds">The AWS Credentails to use.</param>
        /// <param name="endpoint">The region to connect.</param>
        public AwsClient(AWSCredentials creds, RegionEndpoint endpoint = null) { Initialize(creds, endpoint); }

        /// <summary>
        /// Constructor using BasicAWSCredentails object.
        /// </summary>
        /// <param name="accessKey">The AWS Access Key Id.</param>
        /// <param name="secretAccessKey">The secret AWS Access Key.</param>
        /// <param name="endpoint">The region to connect.</param>
        public AwsClient(string accessKey, string secretAccessKey, RegionEndpoint endpoint = null) { Initialize(accessKey, secretAccessKey, endpoint); }

        /// <summary>
        /// Constructor using SessionAWSCredentails object.
        /// </summary>
        /// <param name="accessKey">The AWS Access Key Id.</param>
        /// <param name="secretAccessKey">The secret AWS Access Key.</param>
        /// <param name="sessionToken">The AWS Session token.</param>
        /// <param name="endpoint">The region to connect.</param>
        public AwsClient(string accessKey, string secretAccessKey, string sessionToken, RegionEndpoint endpoint = null) { Initialize(accessKey, secretAccessKey, sessionToken, endpoint); }

        /// <summary>
        /// Constructor using CredentialProfileStoreChain to establish AWSCredentials.
        /// </summary>
        /// <param name="profileName">The name of the AWS profile to get credentials from.</param>
        /// <param name="endpoint">The region to connect.</param>
        public AwsClient(string profileName, RegionEndpoint endpoint = null) { Initialize(profileName, endpoint); }

        /// <summary>
        /// Initialize S3Client using implicit Credentials from config or profile.
        /// </summary>
        /// <param name="endpoint">The region to connect to.</param>
        private void Initialize(RegionEndpoint endpoint = null)
        {
            if (Client != null)
                Client = null;

            if (endpoint == null)
                Client = new AmazonS3Client();
            else
                Client = new AmazonS3Client(endpoint);
        }

        /// <summary>
        /// Initialize S3Client using AWSCredentials object.
        /// </summary>
        /// <param name="creds">The AWSCredentails object.</param>
        /// <param name="endpoint">The region to connect to.</param>
        private void Initialize(AWSCredentials creds, RegionEndpoint endpoint = null)
        {
            if (Client != null)
                Client = null;

            if (endpoint == null)
                Client = new AmazonS3Client(creds);
            else
                Client = new AmazonS3Client(creds, endpoint);
        }

        /// <summary>
        /// Initialize S3Client using a BasicAWSCredentials object.
        /// </summary>
        /// <param name="accessKey">AWS Access Key Id</param>
        /// <param name="secretAccessKey">AWS Secret Access Key</param>
        /// <param name="endpoint">The region to connect to.</param>
        private void Initialize(string accessKey, string secretAccessKey, RegionEndpoint endpoint = null)
        {
            BasicAWSCredentials creds = new BasicAWSCredentials(accessKey, secretAccessKey);
            Initialize(creds, endpoint);
        }

        /// <summary>
        /// Creates a SessionAWSCredentails object to initialize the AmazonS3Client.
        /// </summary>
        /// <param name="accessKey">The AWS Access Key Id.</param>
        /// <param name="secretAccessKey">The secret AWS Access Key.</param>
        /// <param name="sessionToken">The AWS Session token.</param>
        /// <param name="endpoint">The region to connect.</param>
        private void Initialize(string accessKey, string secretAccessKey, string sessionToken, RegionEndpoint endpoint = null)
        {
            SessionAWSCredentials sessionCreds = new SessionAWSCredentials(accessKey, secretAccessKey, sessionToken);
            Initialize(sessionCreds, endpoint);
        }

        /// <summary>
        /// Establishes AWSCredentials from a ProfileName.
        /// </summary>
        /// <param name="profileName">The name of the AWS profile to get credentials from.</param>
        /// <param name="endpoint">The region to connect.</param>
        private void Initialize(string profileName, RegionEndpoint endpoint = null)
        {
            CredentialProfileStoreChain chain = new CredentialProfileStoreChain();
            AWSCredentials creds = null;
            if (chain.TryGetAWSCredentials(profileName, out creds))
                Initialize(creds, endpoint);
            else
                throw new Exception($"Unable To Retrieve Credentails For Profile [{profileName}]");
        }

        /// <summary>
        /// Clears out the client.
        /// </summary>
        public void Close()
        {
            Client = null;
        }
    }
}
