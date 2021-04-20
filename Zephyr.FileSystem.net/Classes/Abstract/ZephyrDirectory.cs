using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Zephyr.Filesystem
{
    /// <summary>
    /// Base class for all implementations of a "Directory" object.  It implements methods common to all directories where it can and
    /// abstracts methods that are specific to the implementation itself.
    /// </summary>
    public abstract class ZephyrDirectory
    {
        /// <summary>
        /// The full name or URL of the directory.
        /// </summary>
        public abstract String FullName { get; set; }

        /// <summary>
        /// The name of the directory.
        /// </summary>
        public abstract String Name { get; }

        /// <summary>
        /// The full name or URL of the parent directory.
        /// </summary>
        public abstract String Parent { get; }

        /// <summary>
        /// The protocol, drive or share of the directory (s3://, C:\, \\localhost\c$, etc...)
        /// </summary>
        public abstract String Root { get; }

        /// <summary>
        /// Determines if a directory exists.
        /// </summary>
        public abstract bool Exists { get; }

        /// <summary>
        /// Is the ZephyrDirectory Implementation empty.
        /// </summary>
        /// <returns>Returns true if the ZephyrDirectory contains no Directories or Files.</returns>
        public bool IsEmpty { get { return (GetDirectories().Count() == 0 && GetFiles().Count() == 0); } }



        /// <summary>
        /// Creates a ZephyrDirectory object.
        /// </summary>
        /// <param name="failIfExists">Throws an error if the directory already exists.</param>
        /// <param name="callbackLabel">Optional "label" to be passed into the callback method.</param>
        /// <param name="callback">Optional method that is called for logging purposes.</param>
        /// <returns>An instance of a ZephyrDirectory Implementation.</returns>
        public abstract ZephyrDirectory Create(bool failIfExists = false, bool verbose = true, String callbackLabel = null, Action<string, string> callback = null);

        /// <summary>
        /// Deletes a ZephyrDirecory object.
        /// </summary>
        /// <param name="recurse">Remove all objects in the directory as well.  If set to "false", directory must be empty or an exception will be thrown.</param>
        /// <param name="stopOnError">Stop deleting objects in the directory if an error is encountered.</param>
        /// <param name="verbose">Log each object that is deleted from the directory.</param>
        /// <param name="callbackLabel">Optional "label" to be passed into the callback method.</param>
        /// <param name="callback">Optional method that is called for logging purposes.</param>
        public abstract void Delete(bool recurse = true, bool stopOnError = true, bool verbose = true, String callbackLabel = null, Action<string, string> callback = null);

        /// <summary>
        /// Creates a ZephyrFile implementation of the same implementation type as the ZephyrDirectory calling it.
        /// </summary>
        /// <param name="fullName">Full name or URL of the file to be created.</param>
        /// <param name="callbackLabel">Optional "label" to be passed into the callback method.</param>
        /// <param name="callback">Optional method that is called for logging purposes.</param>
        /// <returns>A ZephyrFile implementation.</returns>
        public abstract ZephyrFile CreateFile(string fullName, bool verbose = true, String callbackLabel = null, Action<string, string> callback = null);

        /// <summary>
        /// Creates a ZephyrDirectory implementation of the same implementation type as the ZephyrDirectory calling it.
        /// </summary>
        /// <param name="fullName">Full name or URL of the directory to be created.</param>
        /// <param name="callbackLabel">Optional "label" to be passed into the callback method.</param>
        /// <param name="callback">Optional method that is called for logging purposes.</param>
        /// <returns>A ZephyrDirectory implementation.</returns>
        public abstract ZephyrDirectory CreateDirectory(string fullName, bool verbose = true, String callbackLabel = null, Action<string, string> callback = null);

        /// <summary>
        /// Returns a list of all Directories which are direct children of the ZephyrDirectory.
        /// </summary>
        /// <returns>An enumeration of ZephyrDirectory implementations matching the calling ZephyrDirectory implementation.</returns>
        public abstract IEnumerable<ZephyrDirectory> GetDirectories();

        /// <summary>
        /// Returns a list of all Files which are direct children of the ZephyrDirectory.
        /// </summary>
        /// <returns>An enumeration of ZephyrFile implementations.</returns>
        public abstract IEnumerable<ZephyrFile> GetFiles();

        /// <summary>
        /// Combines strings into a path based on the implementation type. 
        /// </summary>
        /// <param name="paths">An array of strings to combine.</param>
        /// <returns>The combined paths.</returns>
        public abstract String PathCombine(params string[] paths);

        /// <summary>
        /// Method to copy the contents of the ZephyrDirectory into another ZephyrDirectory.  
        /// It works by using the base "Stream" property and "Create" methods each implementation must create.
        /// Thus, the ZephyrDirectories do not have to be of the same implementation type.
        /// </summary>
        /// <param name="target">The destination ZephyrDirectory.</param>
        /// <param name="recurse">Copy all sub-directories and all files under this directory.</param>
        /// <param name="overwrite">Should files copied overwrite existing files of the same name.  Directories will be merged.</param>
        /// <param name="stopOnError">Stop copying when an error is encountered.</param>
        /// <param name="verbose">Log each file and directory copied.</param>
        /// <param name="callbackLabel">Optional "label" to be passed into the callback method.</param>
        /// <param name="callback">Optional method that is called for logging purposes.</param>
        public void CopyTo(ZephyrDirectory target, bool recurse = true, bool overwrite = true, bool stopOnError = true, bool verbose = true, String callbackLabel = null, Action<string, string> callback = null)
        {
            if (this.Exists)
            {
                foreach (ZephyrDirectory childDir in GetDirectories())
                {
                    try
                    {
                        String targetChildDirName = target.PathCombine(target.FullName, $"{childDir.Name}/");
                        ZephyrDirectory targetChild = target.CreateDirectory(targetChildDirName);
                        targetChild.Create(verbose: verbose);
                        if (recurse)
                            childDir.CopyTo(targetChild, recurse, overwrite, verbose, stopOnError, callbackLabel, callback);
                    }
                    catch (Exception e)
                    {
                        Logger.Log(e.Message, callbackLabel, callback);
                        if (stopOnError)
                            throw;
                    }
                }

                foreach (ZephyrFile file in GetFiles())
                {
                    try
                    {
                        String targetFileName = target.PathCombine(target.FullName, file.Name);
                        ZephyrFile targetFile = target.CreateFile(targetFileName, verbose, callbackLabel, callback);
                        file.CopyTo(targetFile, overwrite, true, stopOnError, verbose, callbackLabel, callback);
                    }
                    catch (Exception e)
                    {
                        Logger.Log(e.Message, callbackLabel, callback);
                        if (stopOnError)
                            throw;
                    }
                }

                if (verbose)
                    Logger.Log($"Copied Directory [{this.FullName}] to [{target.FullName}].", callbackLabel, callback);
            }
            else
            {
                string message = $"[{this.FullName}] Does Not Exist.";
                Logger.Log(message, callbackLabel, callback);
                if (stopOnError)
                    throw new Exception(message);
            }
        }

        /// <summary>
        /// Method to move the contents of the ZephyrDirectory to another ZephyrDirectory.  
        /// It works by using the base "Stream" property and "Create"methods each implementation must create.
        /// Thus, the ZephyrDirectories do not have to be of the same implementation type.
        /// </summary>
        /// <param name="target">The destination ZephyrDirectory.</param>
        /// <param name="overwrite">Should files copied overwrite existing files of the same name.  Directories will be merged.</param>
        /// <param name="stopOnError">Stop moving when an error is encountered.</param>
        /// <param name="verbose">Log each file and directory moved.</param>
        /// <param name="callbackLabel">Optional "label" to be passed into the callback method.</param>
        /// <param name="callback">Optional method that is called for logging purposes.</param>
        public void MoveTo(ZephyrDirectory target, bool overwrite = true, bool stopOnError = true, bool verbose = true, String callbackLabel = null, Action<string, string> callback = null)
        {
            if (this.Exists)
            {
                foreach (ZephyrDirectory childDir in GetDirectories())
                {
                    try
                    {
                        String targetChildDirName = target.PathCombine(target.FullName, $"{childDir.Name}/");
                        ZephyrDirectory targetChild = target.CreateDirectory(targetChildDirName);
                        targetChild.Create();
                        childDir.MoveTo(targetChild, overwrite, stopOnError, verbose, callbackLabel, callback);
                        childDir.Delete(verbose: false);
                    }
                    catch (Exception e)
                    {
                        Logger.Log(e.Message, callbackLabel, callback);
                        if (stopOnError)
                            throw;
                    }
                }

                foreach (ZephyrFile file in GetFiles())
                {
                    try
                    {
                        String targetFileName = target.PathCombine(target.FullName, file.Name);
                        ZephyrFile targetFile = target.CreateFile(targetFileName);
                        file.MoveTo(targetFile, overwrite, true, stopOnError, verbose, callbackLabel, callback);
                    }
                    catch (Exception e)
                    {
                        Logger.Log(e.Message, callbackLabel, callback);
                        if (stopOnError)
                            throw;
                    }
                }

                if (verbose)
                    Logger.Log($"Moved Directory [{this.FullName}] to [{target.FullName}].", callbackLabel, callback);
            }
            else
            {
                string message = $"[{this.FullName}] Does Not Exist.";
                Logger.Log(message, callbackLabel, callback);
                if (stopOnError)
                    throw new Exception(message);
            }
        }

        /// <summary>
        /// Deletes the contents of the ZephyrDirectory, but leaves the main directory intact.
        /// </summary>
        /// <param name="stopOnError">Stop deleting when an error is encountered.</param>
        /// <param name="verbose">Log each file and directory deleted.</param>
        /// <param name="callbackLabel">Optional "label" to be passed into the callback method.</param>
        /// <param name="callback">Optional method that is called for logging purposes.</param>
        public void Purge(bool stopOnError = true, bool verbose = true, String callbackLabel = null, Action<string, string> callback = null)
        {
            foreach (ZephyrDirectory dir in GetDirectories())
                dir.Delete(true, stopOnError, verbose, callbackLabel, callback);

            foreach (ZephyrFile file in GetFiles())
                file.Delete(stopOnError, verbose, callbackLabel, callback);
        }


    }
}
