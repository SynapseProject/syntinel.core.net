using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Util;

namespace Zephyr.Filesystem
{
    /// <summary>
    /// The implementation of ZephyrDirectory using Amazon S3 storage.
    /// </summary>
    public class AwsS3ZephyrDirectory : ZephyrDirectory
    {
        private string UrlPattern = @"^(s3:\/\/)(.*?)\/(.*)$";        // Gets Root, Bucket Name and Object Key
        private string NamePattern = @"^(s3:\/\/.*\/)(.*?)\/$";       // Gets Parent Name and Name

        private AwsClient _client = null;

        private string _fullName;

        /// <summary>
        /// The Amazon S3 Bucket Name
        /// </summary>
        public string BucketName { get; internal set; }

        /// <summary>
        /// The Amazon S3 ObjectKey
        /// </summary>
        public string ObjectKey { get; internal set; }

        /// <summary>
        /// The fullname / url of the directory in Amazon S3.
        /// </summary>
        public override string FullName
        {
            get { return _fullName; }
            set
            {
                _fullName = value;
                Match match = Regex.Match(value, UrlPattern, RegexOptions.IgnoreCase);
                if (match.Success)
                {
                    BucketName = match.Groups[2].Value;
                    ObjectKey = match.Groups[3].Value;
                }
            }
        }

        /// <summary>
        /// The name of the directory in Amazon S3.
        /// </summary>
        public override string Name
        {
            get
            {
                String name = null;
                Match match = Regex.Match(FullName, NamePattern, RegexOptions.IgnoreCase);
                if (match.Success)
                    name = match.Groups[2].Value;
                return name;
            }
        }

        /// <summary>
        /// The full path / url of the parent directory in Amazon S3.
        /// </summary>
        public override string Parent
        {
            get
            {
                String parent = null;
                Match match = Regex.Match(FullName, NamePattern, RegexOptions.IgnoreCase);
                if (match.Success)
                    parent = match.Groups[1].Value;
                return parent;
            }
        }

        /// <summary>
        /// The root or protocol for the Amazon S3 directory.
        /// </summary>
        public override string Root
        {
            get
            {
                String name = null;
                Match match = Regex.Match(FullName, UrlPattern, RegexOptions.IgnoreCase);
                if (match.Success)
                    name = match.Groups[1].Value;
                return name;
            }
        }

        /// <summary>
        /// Implementation of the ZephyrDirectory Exists method in Amazon S3 Storage.
        /// </summary>
        public override bool Exists
        {
            get
            {
                bool exists = false;
                if (_client == null)
                    throw new Exception($"AWSClient Not Set.");

                GetObjectMetadataRequest request = new GetObjectMetadataRequest()
                {
                    BucketName = this.BucketName,
                    Key = this.ObjectKey
                };

                try
                {
                    if (!String.IsNullOrWhiteSpace(this.ObjectKey))
                    {
                        Task<GetObjectMetadataResponse> task = _client.Client.GetObjectMetadataAsync(request);
                        task.Wait();
                        GetObjectMetadataResponse response = task.Result;
                        exists = true;
                    }
                    else if (!String.IsNullOrEmpty(this.BucketName))
                    {
                        Task<bool> task = AmazonS3Util.DoesS3BucketExistV2Async(_client.Client, this.BucketName);
                        task.Wait();
                        exists = task.Result;
                    }

                }
                catch (AmazonS3Exception e)
                {
                    if (!(e.StatusCode == System.Net.HttpStatusCode.NotFound || ("NoSuchKey".Equals(e.ErrorCode, StringComparison.OrdinalIgnoreCase))))
                        throw;
                }
                catch (Exception e)
                {
                    if (e.InnerException.GetType() == typeof(AmazonS3Exception))
                    {
                        AmazonS3Exception ie = (AmazonS3Exception)e.InnerException;
                        if (!(ie.StatusCode == System.Net.HttpStatusCode.NotFound || ("NoSuchKey".Equals(ie.ErrorCode, StringComparison.OrdinalIgnoreCase))))
                            throw;
                    }
                    else
                        throw;
                }
                return exists;
            }
        }


        /// <summary>
        /// Creates an empty AmazonS3ZephyrDirectory
        /// </summary>
        /// <param name="client">The client class used to connect to Amazon.</param>
        public AwsS3ZephyrDirectory(AwsClient client) { _client = client; }

        /// <summary>
        /// Creates an AmazonS3ZephyrDirectory representing the url passed in.
        /// </summary>
        /// <param name="client">The client class used to connect to Amazon.</param>
        /// <param name="fullName">The Fullname or URL for the Amazon S3 directory.</param>
        public AwsS3ZephyrDirectory(AwsClient client, string fullName)
        {
            _client = client;
            FullName = fullName;
        }

        /// <summary>
        /// Creates an AmazonS3ZephyrDirectory representing the bucketName and objectKey passed in.
        /// </summary>
        /// <param name="client">The client class used to connect to Amazon.</param>
        /// <param name="bucketName">The Amazon S3 bucket name.</param>
        /// <param name="key">The Amazon S3 object key.</param>
        public AwsS3ZephyrDirectory(AwsClient client, string bucketName, string key)
        {
            _client = client;
            FullName = $"s3://{bucketName}/{key}";
        }

        /// <summary>
        /// Implementation of the ZephyrDirectory Create method in Amazon S3 Storage.
        /// </summary>
        /// <param name="failIfExists">Throws an error if the directory already exists.</param>
        /// <param name="callbackLabel">Optional "label" to be passed into the callback method.</param>
        /// <param name="callback">Optional method that is called for logging purposes.</param>
        /// <returns>An AmazonS3ZephyrDictionary Instance.</returns>
        public override ZephyrDirectory Create(bool failIfExists = false, bool verbose = true, string callbackLabel = null, Action<string, string> callback = null)
        {
            if (_client == null)
                throw new Exception($"AWSClient Not Set.");

            if (this.Exists && failIfExists)
                throw new Exception($"Directory [{FullName}] Already Exists.");

            PutObjectRequest request = new PutObjectRequest
            {
                BucketName = BucketName,
                Key = ObjectKey,
                ServerSideEncryptionMethod = _client.SSEMethod,
                StorageClass = _client.StorageClass
            };

            Task<PutObjectResponse> task = _client.Client.PutObjectAsync(request);
            task.Wait();
            PutObjectResponse response = task.Result;

            if (verbose)
                Logger.Log($"Directory [{FullName}] Was Created.", callbackLabel, callback);
            return this;
        }

        /// <summary>
        /// Creates an AmazonS3ZephyrFile implementation using the Fullname / URL passed in.
        /// </summary>
        /// <param name="fullName">Full name or URL of the file to be created.</param>
        /// <param name="callbackLabel">Optional "label" to be passed into the callback method.</param>
        /// <param name="callback">Optional method that is called for logging purposes.</param>
        /// <returns>An AmazonS3ZephyrFile implementation.</returns>
        public override ZephyrFile CreateFile(string fullName, bool verbose = true, String callbackLabel = null, Action<string, string> callback = null)
        {
            return new AwsS3ZephyrFile(_client, fullName);
        }

        /// <summary>
        /// Creates an AmazonS3ZephyrDirectory implementation using the Fullname / URL passed in.
        /// </summary>
        /// <param name="fullName">Full name or URL of the directory to be created.</param>
        /// <param name="callbackLabel">Optional "label" to be passed into the callback method.</param>
        /// <param name="callback">Optional method that is called for logging purposes.</param>
        /// <returns>An AmazonS3ZephyrDirectory implementation.</returns>
        public override ZephyrDirectory CreateDirectory(string fullName, bool verbose = true, String callbackLabel = null, Action<string, string> callback = null)
        {
            return new AwsS3ZephyrDirectory(_client, fullName);
        }

        /// <summary>
        /// Implementation of the ZephyrDirectory Delete method in Amazon S3 Storage.
        /// </summary>
        /// <param name="recurse">Remove all objects in the directory as well.  If set to "false", directory must be empty or an exception will be thrown.</param>
        /// <param name="stopOnError">Stop deleting objects in the directory if an error is encountered.</param>
        /// <param name="verbose">Log each object that is deleted from the directory.</param>
        /// <param name="callbackLabel">Optional "label" to be passed into the callback method.</param>
        /// <param name="callback">Optional method that is called for logging purposes.</param>
        public override void Delete(bool recurse = true, bool stopOnError = true, bool verbose = true, string callbackLabel = null, Action<string, string> callback = null)
        {
            try
            {
                if (_client == null)
                    throw new Exception($"AWSClient Not Set.");

                List<S3Object> objects = ListObjects(this.BucketName, this.ObjectKey);
                if (objects.Count > 0)
                {
                    if (recurse)
                        foreach (S3Object obj in objects)
                            DeleteObject(obj);
                    else
                    {
                        if (objects.Count > 1)
                            throw new Exception($"Directory [{FullName}] is not empty.");
                        DeleteObject(objects[0]);
                    }
                }

                if (verbose)
                    Logger.Log($"Directory [{FullName}] Was Deleted.", callbackLabel, callback);
            }
            catch (Exception e)
            {
                Logger.Log(e.Message, callbackLabel, callback);
                if (stopOnError)
                    throw;
            }
        }

        /// <summary>
        /// Implementation of the ZephyrDirectory GetDirectories method in AmazonS3Storage.
        /// </summary>
        /// <returns>An enumeration of AmazonS3ZephyrDirectory objects.</returns>
        public override IEnumerable<ZephyrDirectory> GetDirectories()
        {
            if (_client == null)
                throw new Exception($"AWSClient Not Set.");

            List<ZephyrDirectory> dirs = new List<ZephyrDirectory>();

            List<S3Object> objects = ListObjects(this.BucketName, this.ObjectKey);
            foreach (S3Object obj in objects)
            {
                String key = obj.Key;
                String compKey = key;
                if (!String.IsNullOrWhiteSpace(this.ObjectKey))
                    compKey = key.Replace(this.ObjectKey, "");
                if ((compKey.IndexOf("/") == (compKey.Length - 1)) && (compKey.Length > 0))
                    dirs.Add(new AwsS3ZephyrDirectory(_client, obj.BucketName, obj.Key));
            }

            return dirs;
        }

        /// <summary>
        /// Implementation of the ZephyrDirectory GetFiles method in AmazonS3Storage.
        /// </summary>
        /// <returns>An enumeration of AmazonS3ZephyrFile objects.</returns>
        public override IEnumerable<ZephyrFile> GetFiles()
        {
            if (_client == null)
                throw new Exception($"AWSClient Not Set.");

            List<ZephyrFile> files = new List<ZephyrFile>();

            List<S3Object> objects = ListObjects(this.BucketName, this.ObjectKey);
            foreach (S3Object obj in objects)
            {
                String key = obj.Key;
                String compKey = key.Replace(this.ObjectKey, "");
                if (!compKey.Contains("/") && compKey.Length > 0)
                    files.Add(new AwsS3ZephyrFile(_client, obj.BucketName, obj.Key));
            }

            return files;
        }

        /// <summary>
        /// Implementation of the ZephyrDirectory PathCombine method in AmazonS3Storage.
        /// </summary>
        /// <param name="paths">An array of strings to combine.</param>
        /// <returns>The combined paths.</returns>
        public override string PathCombine(params string[] paths)
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < paths.Length; i++)
            {
                string path = paths[i]?.Trim();
                if (path == null)
                    continue;
                else if (path.EndsWith("/"))
                    sb.Append(path);
                else if (i == paths.Length - 1)
                    sb.Append(path);
                else
                    sb.Append($"{path}/");
            }

            return sb.ToString();
        }

        private List<S3Object> ListObjects(string bucketName, string prefix)
        {
            ListObjectsV2Request request = new ListObjectsV2Request()
            {
                BucketName = bucketName,
                Prefix = prefix
            };

            Task<ListObjectsV2Response> task = _client.Client.ListObjectsV2Async(request);
            task.Wait();
            ListObjectsV2Response response = task.Result;
            return response.S3Objects;
        }

        private void DeleteObject(S3Object obj)
        {
            DeleteObjectRequest request = new DeleteObjectRequest()
            {
                BucketName = obj.BucketName,
                Key = obj.Key
            };

            Task<DeleteObjectResponse> task = _client.Client.DeleteObjectAsync(request);
            task.Wait();
            DeleteObjectResponse response = task.Result;
        }
    }
}
