using System;
using System.Collections.Generic;
using System.IO;

namespace Zephyr.Filesystem
{
    /// <summary>
    /// The implementation of ZephyrDirectory using Windows-based Filesystems.
    /// </summary>
    public class WindowsZephyrDirectory : ZephyrDirectory
    {
        private DirectoryInfo dirInfo = null;

        /// <summary>
        /// Creates an empty instance of a WindowsZephyrDirectory
        /// </summary>
        public WindowsZephyrDirectory() { }

        /// <summary>
        /// Creates an instance of a WindowsZephyrDirectory representing the Fullname or URL passed in.
        /// </summary>
        /// <param name="fullPath">The Fullname or URL of the directory.</param>
        public WindowsZephyrDirectory(string fullPath)
        {
            FullName = fullPath;
        }

        /// <summary>
        /// The Fullname or URL to the Windows Directory.
        /// </summary>
        public override String FullName
        {
            get { return $"{dirInfo.FullName}\\"; }
            set { dirInfo = new DirectoryInfo(value); }
        }

        /// <summary>
        /// The name of the Windows Directory.
        /// </summary>
        public override String Name { get { return dirInfo?.Name; } }

        /// <summary>
        /// The parent of the Windows Directory.
        /// </summary>
        public override String Parent { get { return $"{dirInfo?.Parent?.FullName}\\"; } }


        /// <summary>
        /// The root or protocol for the Windows Directory (Drive Letter or Network Server/Share)
        /// </summary>
        public override String Root { get { return dirInfo?.Root?.FullName; } }

        /// <summary>
        /// Implementation of the ZephyrDirectory Exists method using Windows FileSystem.
        /// </summary>
        public override bool Exists { get { return Directory.Exists(FullName); } }


        /// <summary>
        /// Implementation of the ZephyrDirectory Create method using Windows FileSystem.
        /// </summary>
        /// <param name="failIfExists">Throws an error if the directory already exists.</param>
        /// <param name="callbackLabel">Optional "label" to be passed into the callback method.</param>
        /// <param name="callback">Optional method that is called for logging purposes.</param>
        /// <returns>A WindowsZephyrDictionary Instance.</returns>
        public override ZephyrDirectory Create(bool failIfExists = false, bool verbose = true, String callbackLabel = null, Action<string, string> callback = null)
        {
            if (!Directory.Exists(FullName))
                Directory.CreateDirectory(FullName);
            else if (failIfExists)
                throw new Exception($"Directory [{FullName}] Already Exists.");
            if (verbose)
                Logger.Log($"Directory [{FullName}] Was Created.", callbackLabel, callback);
            return this;
        }

        /// <summary>
        /// Implementation of the ZephyrDirectory CreateFile method using Windows FileSystem.
        /// </summary>
        /// <param name="fullName">Full name or URL of the file to be created.</param>
        /// <param name="callbackLabel">Optional "label" to be passed into the callback method.</param>
        /// <param name="callback">Optional method that is called for logging purposes.</param>
        /// <returns>A WindowsZephyrFile implementation.</returns>
        public override ZephyrFile CreateFile(string fullName, bool verbose = true, String callbackLabel = null, Action<string, string> callback = null)
        {
            return new WindowsZephyrFile(fullName);
        }

        /// <summary>
        /// Implementation of the ZephyrDirectory CreateFile method using Windows FileSystem.
        /// </summary>
        /// <param name="fullName">Full name or URL of the directory to be created.</param>
        /// <param name="callbackLabel">Optional "label" to be passed into the callback method.</param>
        /// <param name="callback">Optional method that is called for logging purposes.</param>
        /// <returns>A WindowsZephyrDirectory implementation.</returns>
        public override ZephyrDirectory CreateDirectory(string fullName, bool verbose = true, String callbackLabel = null, Action<string, string> callback = null)
        {
            return new WindowsZephyrDirectory(fullName);
        }

        /// <summary>
        /// Implementation of the ZephyrDirectory Delete method using Windows FileSystem.
        /// </summary>
        /// <param name="recurse">Remove all objects in the directory as well.  If set to "false", directory must be empty or an exception will be thrown.</param>
        /// <param name="stopOnError">Stop deleting objects in the directory if an error is encountered.</param>
        /// <param name="verbose">Log each object that is deleted from the directory.</param>
        /// <param name="callbackLabel">Optional "label" to be passed into the callback method.</param>
        /// <param name="callback">Optional method that is called for logging purposes.</param>
        public override void Delete(bool recurse = true, bool stopOnError = true, bool verbose = true, String callbackLabel = null, Action<string, string> callback = null)
        {
            try
            {
                DirectoryInfo dirInfo = new DirectoryInfo(FullName);

                if (dirInfo.Exists)
                {
                    if (!recurse)
                    {
                        int dirs = dirInfo.GetDirectories().Length;
                        int files = dirInfo.GetFiles().Length;
                        if (dirs > 0 || files > 0)
                            throw new Exception($"Directory [{FullName}] is not empty.");
                    }
                    dirInfo.Delete(recurse);
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

            dirInfo = null;     // TODO : Why did I do this?
        }

        /// <summary>
        /// Implementation of the ZephyrDirectory GetDirectories method using Windows FileSystem.
        /// </summary>
        /// <returns>An enumeration of WindowsZephyrDirectory objects.</returns>
        public override IEnumerable<ZephyrDirectory> GetDirectories()
        {
            String[] directories = Directory.GetDirectories(FullName);

            List<ZephyrDirectory> synDirs = new List<ZephyrDirectory>();
            foreach (string dir in directories)
            {
                ZephyrDirectory synDir = new WindowsZephyrDirectory(Path.Combine(FullName, dir));
                synDirs.Add(synDir);
            }

            return synDirs;
        }

        /// <summary>
        /// Implementation of the ZephyrDirectory GetDirectories method using Windows FileSystem.
        /// </summary>
        /// <returns>An enumeration of WindowsZephyrFile objects.</returns>
        public override IEnumerable<ZephyrFile> GetFiles()
        {
            String[] files = Directory.GetFiles(FullName);
            List<ZephyrFile> synFiles = new List<ZephyrFile>();
            foreach (string file in files)
            {
                ZephyrFile synFile = new WindowsZephyrFile(Path.Combine(FullName, file));
                synFiles.Add(synFile);
            }

            return synFiles;
        }

        /// <summary>
        /// Implementation of the ZephyrDirectory PathCombine method using Windows FileSystem.
        /// </summary>
        /// <param name="paths">An array of strings to combine.</param>
        /// <returns>The combined paths.</returns>
        public override string PathCombine(params string[] paths)
        {
            List<string> fixedPaths = new List<string>();
            foreach (string path in paths)
            {
                if (path == "/" || path == "\\")
                    fixedPaths.Add($"_{path}");     // Windows doesn't allow blank directory names, replace with underscore.
                else
                    fixedPaths.Add(path);
            }

            return Path.Combine(fixedPaths.ToArray());
        }
    }
}
