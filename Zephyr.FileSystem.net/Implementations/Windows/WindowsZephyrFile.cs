using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;


namespace Zephyr.Filesystem
{
    /// <summary>
    /// The implementation of ZephyrFile using Windows-based FileSystem.
    /// </summary>
    public class WindowsZephyrFile : ZephyrFile
    {
        /// <summary>
        /// The name of the file in Windows.
        /// </summary>
        public override string Name
        {
            get
            {
                return Path.GetFileName(this.FullName);
            }
        }

        /// <summary>
        /// The Fullname or URL of the file in Windows.
        /// </summary>
        public override string FullName { get; set; }

        /// <summary>
        /// Does the directory exist.
        /// </summary>
        public override bool Exists { get { return File.Exists(FullName); } }

        /// <summary>
        /// Creates an empty WindowsZephyrFile object.
        /// </summary>
        public WindowsZephyrFile() : base() { }

        /// <summary>
        /// Creates an instance of WindowsZephyrFile representing the Fullname / URL passed in.
        /// </summary>
        /// <param name="fullName">The Fullname or URL of the file.</param>
        public WindowsZephyrFile(string fullName) : base(fullName) { }

        /// <summary>
        /// Implementation of the ZephyrFile Open method in Windows.
        /// </summary>
        /// <param name="access">Specifies to open Stream with "Read" or "Write" access.</param>
        /// <param name="callbackLabel">Optional "label" to be passed into the callback method.</param>
        /// <param name="callback">Optional method that is called for logging purposes.</param>
        /// <returns>The open Stream for the WindowsZephyrFile.</returns>
        public override System.IO.Stream Open(AccessType access, bool verbose = true, String callbackLabel = null, Action<string, string> callback = null)
        {
            if (!IsOpen)
            {
                this.Stream = File.Open(FullName, System.IO.FileMode.OpenOrCreate, access == AccessType.Read ? System.IO.FileAccess.Read : System.IO.FileAccess.Write);
                if (verbose)
                    Logger.Log($"File Stream [{FullName}] Has Been Opened.", callbackLabel, callback);
            }
            else
            {
                if (verbose)
                    Logger.Log($"File Stream [{FullName}] Is Already Open.", callbackLabel, callback);
            }
            return this.Stream;
        }

        /// <summary>
        /// Implementation of the ZephyrFile Close method in Windows.
        /// </summary>
        /// <param name="callbackLabel">Optional "label" to be passed into the callback method.</param>
        /// <param name="callback">Optional method that is called for logging purposes.</param>
        public override void Close(bool verbose = true, String callbackLabel = null, Action<string, string> callback = null)
        {
            if (IsOpen)
            {
                this.Stream.Close();
                if (verbose)
                    Logger.Log($"File Stream [{FullName}] Has Been Closed.", callbackLabel, callback);
            }
            else
            {
                if (verbose)
                    Logger.Log($"File Stream [{FullName}] Is Already Closed.", callbackLabel, callback);
            }

            this.Stream = null;

        }

        /// <summary>
        /// Implementation of the ZephyrFile Flush method in Windows.
        /// </summary>
        /// <param name="callbackLabel">Optional "label" to be passed into the callback method.</param>
        /// <param name="callback">Optional method that is called for logging purposes.</param>
        public override void Flush(bool verbose = true, String callbackLabel = null, Action<string, string> callback = null)
        {
            if (IsOpen)
            {
                this.Stream.Flush();
                if (verbose)
                    Logger.Log($"File Stream [{FullName}] Has Been Flushed.", callbackLabel, callback);
            }
        }

        /// <summary>
        /// Implementation of the ZephyrFile Create method in Windows.
        /// </summary>
        /// <param name="overwrite">Will overwrite the file if it already exists.</param>
        /// <param name="callbackLabel">Optional "label" to be passed into the callback method.</param>
        /// <param name="callback">Optional method that is called for logging purposes.</param>
        /// <returns>An instance of a WindowsZephyrFile.</returns>
        public override ZephyrFile Create(bool overwrite = true, bool verbose = true, String callbackLabel = null, Action<string, string> callback = null)
        {
            try
            {
                if (this.Exists && !overwrite)
                    throw new Exception($"File [{this.FullName}] Already Exists.");

                this.Stream = File.Open(FullName, System.IO.FileMode.OpenOrCreate, System.IO.FileAccess.Write);
                if (verbose)
                    Logger.Log($"File [{FullName}] Was Created.", callbackLabel, callback);
                return this;
            }
            catch (Exception e)
            {
                Logger.Log($"ERROR - {e.Message}", callbackLabel, callback);
                throw;
            }
        }

        /// <summary>
        /// Implementation of the ZephyrFile CreateFile method in Windows.
        /// </summary>
        /// <param name="fullName">Full name or URL of the file to be created.</param>
        /// <param name="callbackLabel">Optional "label" to be passed into the callback method.</param>
        /// <param name="callback">Optional method that is called for logging purposes.</param>
        /// <returns>A WindowsZephyrFile implementation.</returns>
        public override ZephyrFile CreateFile(string fileName, bool verbose = true, String callbackLabel = null, Action<string, string> callback = null)
        {
            return new WindowsZephyrFile(fileName);
        }

        /// <summary>
        /// Implementation of the ZephyrDirectory CreateDirectory method in Windows.
        /// </summary>
        /// <param name="dirName">Full name or URL of the directory to be created.</param>
        /// <param name="callbackLabel">Optional "label" to be passed into the callback method.</param>
        /// <param name="callback">Optional method that is called for logging purposes.</param>
        /// <returns>A WindowsZephyrDirectory implementation.</returns>
        public override ZephyrDirectory CreateDirectory(string dirName, bool verbose = true, String callbackLabel = null, Action<string, string> callback = null)
        {
            return new WindowsZephyrDirectory(dirName);
        }

        /// <summary>
        /// Implementation of the ZephyrDirectory Delete method in Windows.
        /// </summary>
        /// <param name="stopOnError">Throw an exception when an error occurs.</param>
        /// <param name="verbose">Log details of file deleted.</param>
        /// <param name="callbackLabel">Optional "label" to be passed into the callback method.</param>
        /// <param name="callback">Optional method that is called for logging purposes.</param>
        public override void Delete(bool stopOnError = true, bool verbose = true, String callbackLabel = null, Action<string, string> callback = null)
        {
            try
            {
                FileInfo fileInfo = new FileInfo(FullName);
                if (fileInfo.Exists)
                {
                    if (IsOpen)
                        Close();
                    fileInfo.Delete();
                }

                if (verbose)
                    Logger.Log($"File [{FullName}] Was Deleted.", callbackLabel, callback);
            }
            catch (Exception e)
            {
                Logger.Log(e.Message, callbackLabel, callback);
                if (stopOnError)
                    throw;
            }
        }
    }
}
