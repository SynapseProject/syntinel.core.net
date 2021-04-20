using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Amazon;
using Amazon.S3;
using Amazon.Runtime;
using Amazon.Runtime.CredentialManagement;

//using Alphaleonis.Win32.Filesystem;

namespace Zephyr.Filesystem
{
    /// <summary>
    /// A class of static, helper methods around working with ZephyrFiles and ZephyrDirectories.
    /// </summary>
    public static class Utilities
    {
        /// <summary>
        /// Determines the type of URL that was passed in.
        /// Basic Rules :
        /// - All directories must end with a slash (/ or \)
        /// - Implementation type is determined by the Url "root".
        ///   s3://     = Amazon S3 Storage
        ///   \\        = Network Url
        ///   default   = Local
        /// </summary>
        /// <param name="url">The FullName or URL.</param>
        /// <returns></returns>
        public static UrlType GetUrlType(string url)
        {
            UrlType type = UrlType.Unknown;

            if (url != null)
            {
                if (url.StartsWith("s3://", StringComparison.OrdinalIgnoreCase))
                {
                    if (IsDirectory(url))
                        type = UrlType.AwsS3Directory;
                    else
                        type = UrlType.AwsS3File;
                }
                else if (url.StartsWith("\\"))
                {
                    if (IsDirectory(url))
                        type = UrlType.NetworkDirectory;
                    else
                        type = UrlType.NetworkFile;
                }
                else
                {
                    if (IsDirectory(url))
                        type = UrlType.LocalDirectory;
                    else
                        type = UrlType.LocalFile;
                }
            }

            return type;
        }

        /// <summary>
        /// In Windows, it is impossible to determine by name along if a URL is a directory or a file with no extension.  Thus
        /// the decision was made that all directory url's MUST end with a forward or backward slash (/ or \).  This removes any
        /// ambiguity.   This standard should be carried forward to all other implementation types (Aws, Azure, FTP, etc...)
        /// </summary>
        /// <param name="url">The full path to the object.</param>
        /// <returns></returns>
        public static bool IsDirectory(string url)
        {
            bool rc = false;
            if (url != null)
                rc = (url.EndsWith("/") || url.EndsWith(@"\"));
            return rc;
        }

        /// <summary>
        /// In Windows, it is impossible to determine by name along if a URL is a directory or a file with no extension.  Thus
        /// the decision was made that all directory url's MUST end with a forward or backward slash (/ or \).  This removes any
        /// ambiguity.   This standard should be carried forward to all other implementation types (Aws, Azure, FTP, etc...)
        /// </summary>
        /// <param name="url">The full path to the object.</param>
        /// <returns></returns>
        public static bool IsFile(string url)
        {
            return !IsDirectory(url);
        }

        /// <summary>
        /// Gets a ZephyrFile implementation matching the URL type passed in.
        /// </summary>
        /// <param name="url">The Fullname or URL of the file.</param>
        /// <param name="clients">A collection of connection clients.</param>
        /// <returns>A ZephyrFile implementation.</returns>
        public static ZephyrFile GetZephyrFile(string url, Clients clients = null)
        {
            ZephyrFile file = null;
            UrlType type = GetUrlType(url);
            switch (type)
            {
                case UrlType.LocalFile:
                    file = new WindowsZephyrFile(url);
                    break;
                case UrlType.NetworkFile:
                    file = new WindowsZephyrFile(url);
                    break;
                case UrlType.AwsS3File:
                    file = new AwsS3ZephyrFile(clients?.aws, url);
                    break;
            }

            return file;
        }

        /// <summary>
        /// Gets a ZephyrFile implementation matching the URL type passed in.
        /// </summary>
        /// <param name="url">The Fullname or URL of the directory.</param>
        /// <param name="clients">A collection of connection clients.</param>
        /// <returns>A ZephyrFile implementation.</returns>
        public static ZephyrDirectory GetZephyrDirectory(string url, Clients clients = null)
        {
            ZephyrDirectory dir = null;
            UrlType type = GetUrlType(url);
            switch (type)
            {
                case UrlType.LocalDirectory:
                    dir = new WindowsZephyrDirectory(url);
                    break;
                case UrlType.NetworkDirectory:
                    dir = new WindowsZephyrDirectory(url);
                    break;
                case UrlType.AwsS3Directory:
                    dir = new AwsS3ZephyrDirectory(clients?.aws, url);
                    break;
                default:
                    throw new Exception($"Url [{url}] Is Not A Known Directory Type.");
            }

            return dir;
        }

        /// <summary>
        /// Static method to create a ZephyrFile whose implementation if based on the Fullname / URL passed in.
        /// </summary>
        /// <param name="fileName">The Fullname or URL of the file.</param>
        /// <param name="clients">A collection of connection clients.</param>
        /// <param name="overwrite">Will overwrite the file if it already exists.</param>
        /// <param name="callbackLabel">Optional "label" to be passed into the callback method.</param>
        /// <param name="callback">Optional method that is called for logging purposes.</param>
        /// <returns>A ZephyrFile instance.</returns>
        public static ZephyrFile CreateFile(string fileName, Clients clients = null, bool overwrite = true, bool verbose = true, String callbackLabel = null, Action<string, string> callback = null)
        {
            ZephyrFile file = Utilities.GetZephyrFile(fileName, clients);
            return file.Create(overwrite, verbose, callbackLabel, callback);
        }

        /// <summary>
        /// Static method to create a ZephyrDirectory whose implementation is based on the Fullname / URL passed in.
        /// </summary>
        /// <param name="dirName">The Fullname or URL of the directory.</param>
        /// <param name="clients">A collection of connection clients.</param>
        /// <param name="failIfExists">Throws an error if the directory already exists.</param>
        /// <param name="callbackLabel">Optional "label" to be passed into the callback method.</param>
        /// <param name="callback">Optional method that is called for logging purposes.</param>
        /// <returns>A ZephyrDirectory instance.</returns>
        public static ZephyrDirectory CreateDirectory(string dirName, Clients clients = null, bool failIfExists = false, bool verbose = true, String callbackLabel = null, Action<string, string> callback = null)
        {
            ZephyrDirectory dir = Utilities.GetZephyrDirectory(dirName, clients);
            return dir.Create(failIfExists, verbose, callbackLabel, callback);
        }

        /// <summary>
        /// Static method to delete a ZephyrFile or ZephyrDirectory based on the Fullname / URL passed in.
        /// </summary>
        /// <param name="name">The Fullname or URL of the file or directory.</param>
        /// <param name="clients">A collection of connection clients.</param>
        /// <param name="recurse">Remove all objects in the directory as well.  If set to "false", directory must be empty or an exception will be thrown.</param>
        /// <param name="stopOnError">Stop deleting objects in the directory if an error is encountered.</param>
        /// <param name="verbose">Log each object that is deleted from the directory.</param>
        /// <param name="callbackLabel">Optional "label" to be passed into the callback method.</param>
        /// <param name="callback">Optional method that is called for logging purposes.</param>
        public static void Delete(string name, Clients clients = null, bool recurse = true, bool stopOnError = true, bool verbose = true, String callbackLabel = null, Action<string, string> callback = null)
        {
            if (Utilities.IsDirectory(name))
            {
                ZephyrDirectory dir = Utilities.GetZephyrDirectory(name, clients);
                dir.Delete(recurse, stopOnError, verbose, callbackLabel, callback);
            }
            else
            {
                ZephyrFile file = Utilities.GetZephyrFile(name, clients);
                file.Delete(stopOnError, verbose, callbackLabel, callback);
            }
        }

        /// <summary>
        /// Static method to determine if a file or directory exists based on the Fullname/URL passed in.
        /// </summary>
        /// <param name="name">The Fullname or URL of the file or directory.</param>
        /// <param name="clients">A collection of connection clients.</param>
        /// <returns>Whether or now the file or directory already exists.</returns>
        public static bool Exists(string name, Clients clients = null)
        {
            if (Utilities.IsDirectory(name))
            {
                ZephyrDirectory dir = Utilities.GetZephyrDirectory(name, clients);
                return dir.Exists;
            }
            else
            {
                ZephyrFile file = Utilities.GetZephyrFile(name, clients);
                return file.Exists;
            }
        }

        public static AwsClient InitAwsClient(RegionEndpoint region = null, string accessKey = null, string secretKey = null)
        {
            AwsClient client = null;

            bool hasAccessKey = (!String.IsNullOrWhiteSpace(accessKey));
            bool hasSecretKey = (!String.IsNullOrWhiteSpace(secretKey));
            bool hasRegion = (region != null);

            if (hasAccessKey && hasSecretKey)
            {
                if (hasRegion)
                    client = new AwsClient(accessKey, secretKey, region);
                else
                    client = new AwsClient(accessKey, secretKey);
            }
            else if (hasRegion)
                client = new AwsClient(region);
            else
                client = new AwsClient();     // Pull All Details From Environemnt Variables / Credentails Files

            return client;
        }

    }
}
