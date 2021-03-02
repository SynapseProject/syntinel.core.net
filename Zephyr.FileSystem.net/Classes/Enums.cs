using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zephyr.Filesystem
{
    /// <summary>
    /// Specified whether a ZephyrFile should be open for Read or Write access.
    /// </summary>
    public enum AccessType
    {
        Read,                   // Opens ZephyrFile Stream for Read Access
        Write                   // Opens ZephyrFile Stream For Write Access
    }

    /// <summary>
    /// Supported ZephyrFile and ZephyrDirectory types
    /// </summary>
    public enum UrlType
    {
        Unknown,                // File or Directory Type is not known.
        LocalFile,              // A file local to the system (C:\Temp\MyFile.txt)
        LocalDirectory,         // A directory local to the system (C:\Temp\MyDirectory\)
        NetworkFile,            // A network file (\\myserver\myshare$\Dir001\MyFile.txt)
        NetworkDirectory,       // A network directory (\\myserver\myshare$\Dir001\)
        AwsS3File,              // An Amazon S3 File (s3://mybucket/dir001/MyFile.txt)
        AwsS3Directory          // An Amazon S3 Directory (s3://mybucket/dir001/MyDirectory/)
    }
}
