using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Zephyr.Filesystem
{
    /// <summary>
    /// Base class for all implementations of a "File" object.  It implements methods common to all files where it can and
    /// abstracts methods that are specific to the implementation itself.
    /// </summary>
    public abstract class ZephyrFile
    {
        /// <summary>
        /// The full path or URL to the file.
        /// </summary>
        public abstract String FullName { get; set; }

        /// <summary>
        /// The name of the file.
        /// </summary>
        public abstract String Name { get; }

        /// <summary>
        /// Determines if a file exists.
        /// </summary>
        public abstract bool Exists { get; }

        /// <summary>
        /// The Stream object for the file.
        /// </summary>
        public System.IO.Stream Stream { get; internal set; }

        /// <summary>
        /// Is the file Stream Open?
        /// </summary>
        public bool IsOpen { get { return this.Stream == null ? false : (this.Stream.CanRead || this.Stream.CanWrite); } }

        /// <summary>
        /// Is the file Stream Readable?
        /// </summary>
        public bool CanRead { get { return this.Stream == null ? false : this.Stream.CanRead; } }

        /// <summary>
        /// Is the file Stream Writeable?
        /// </summary>
        public bool CanWrite { get { return this.Stream == null ? false : this.Stream.CanWrite; } }

        /// <summary>
        /// Creates an empty ZephyrFile Base Class.
        /// </summary>
        protected ZephyrFile() { }

        /// <summary>
        /// Assigns the FullName field of the Base Class.
        /// </summary>
        /// <param name="fullName"></param>
        protected ZephyrFile(string fullName)
        {
            FullName = fullName;
        }

        /// <summary>
        /// Creates a ZephyrFile object.
        /// </summary>
        /// <param name="overwrite">Will overwrite the file if it already exists.</param>
        /// <param name="callbackLabel">Optional "label" to be passed into the callback method.</param>
        /// <param name="callback">Optional method that is called for logging purposes.</param>
        /// <returns>An instance of a ZephyrFile implementation.</returns>
        public abstract ZephyrFile Create(bool overwrite = true, bool verbose = true, String callbackLabel = null, Action<string, string> callback = null);

        /// <summary>
        /// Deletes a ZephyrFile object.
        /// </summary>
        /// <param name="stopOnError">Throw an exception when an error occurs.</param>
        /// <param name="verbose">Log details of file deleted.</param>
        /// <param name="callbackLabel">Optional "label" to be passed into the callback method.</param>
        /// <param name="callback">Optional method that is called for logging purposes.</param>
        public abstract void Delete(bool stopOnError = true, bool verbose = true, String callbackLabel = null, Action<string, string> callback = null);

        /// <summary>
        /// Creates a ZephyrFile implementation of the same implementation type as the ZephyrFile calling it.
        /// </summary>
        /// <param name="fullName">Full name or URL of the file to be created.</param>
        /// <param name="callbackLabel">Optional "label" to be passed into the callback method.</param>
        /// <param name="callback">Optional method that is called for logging purposes.</param>
        /// <returns>A ZephyrFile implementation.</returns>
        public abstract ZephyrFile CreateFile(string fullName, bool verbose = true, String callbackLabel = null, Action<string, string> callback = null);

        /// <summary>
        /// Creates a ZephyrDirectory implementation of the same implementation type as the ZephyrFile calling it.
        /// </summary>
        /// <param name="fullName">Full name or URL of the directory to be created.</param>
        /// <param name="callbackLabel">Optional "label" to be passed into the callback method.</param>
        /// <param name="callback">Optional method that is called for logging purposes.</param>
        /// <returns>A ZephyrDirectory implementation.</returns>
        public abstract ZephyrDirectory CreateDirectory(string fullName, bool verbose = true, String callbackLabel = null, Action<string, string> callback = null);

        /// <summary>
        /// Opens the underlying Stream associated with the ZephyrFile implementation.
        /// </summary>
        /// <param name="access">Specifies to open Stream with "Read" or "Write" access.</param>
        /// <param name="callbackLabel">Optional "label" to be passed into the callback method.</param>
        /// <param name="callback">Optional method that is called for logging purposes.</param>
        /// <returns>The open Stream for the ZephyrFile.</returns>
        public abstract Stream Open(AccessType access, bool verbose = true, String callbackLabel = null, Action<string, string> callback = null);

        /// <summary>
        /// Closes the underlying Steam associated with the ZephyrFile implementation.
        /// </summary>
        /// <param name="callbackLabel">Optional "label" to be passed into the callback method.</param>
        /// <param name="callback">Optional method that is called for logging purposes.</param>
        public abstract void Close(bool verbose = true, String callbackLabel = null, Action<string, string> callback = null);

        /// <summary>
        /// Flushes the underlying Steam associated with the ZephyrFile implementation.
        /// </summary>
        /// <param name="callbackLabel">Optional "label" to be passed into the callback method.</param>
        /// <param name="callback">Optional method that is called for logging purposes.</param>
        public abstract void Flush(bool verbose = true, String callbackLabel = null, Action<string, string> callback = null);

        /// <summary>
        /// Method to copy the contents of the ZephyrFile into another ZephyrFile.  
        /// It works by using the base "Stream" property and "Create"methods each implementation must create.
        /// Thus, the ZephyrFiles do not have to be of the same implementation type.
        /// </summary>
        /// <param name="file">The destination ZephyrFile.</param>
        /// <param name="overwrite">Should the ZephyrFile overwrite existing ZephyrFile if it exists.</param>
        /// <param name="stopOnError">Throw an exception if the copy fails.</param>
        /// <param name="verbose">Log details of the file being copied.</param>
        /// <param name="callbackLabel">Optional "label" to be passed into the callback method.</param>
        /// <param name="callback">Optional method that is called for logging purposes.</param>
        public void CopyTo(ZephyrFile file, bool overwrite = true, bool createMissingDirectories = true, bool stopOnError = true, bool verbose = true, String callbackLabel = null, Action<string, string> callback = null)
        {
            try
            {
                if (!this.Exists)
                    throw new Exception($"File [{this.FullName}] Does Not Exist.");

                if (file.Exists && !overwrite)
                    throw new Exception($"File [{file.FullName}] Already Exists.");

                String targetDirectory = file.FullName.Substring(0, file.FullName.LastIndexOf(file.Name));
                ZephyrDirectory targetDir = file.CreateDirectory(targetDirectory, verbose);
                if (!targetDir.Exists)
                {
                    if (createMissingDirectories)
                        targetDir.Create(verbose: verbose);
                    else
                        throw new Exception($"Directory [{targetDir.FullName}] Does Not Exist.");
                }

                Stream source = this.Open(AccessType.Read, verbose);
                Stream target = file.Open(AccessType.Write, verbose);

                source.CopyTo(target);

                this.Close(verbose);
                file.Close(verbose);

                if (verbose)
                    Logger.Log($"Copied File [{this.FullName}] to [{file.FullName}].", callbackLabel, callback);

            }
            catch (Exception e)
            {
                Logger.Log($"ERROR - {e.Message}", callbackLabel, callback);
                if (stopOnError)
                    throw;
            }
        }

        /// <summary>
        /// Method to move a ZephyrFile to another Zephyrfile..  
        /// It works by using the base "Stream" property and "Create"methods each implementation must create.
        /// Thus, the ZephyrFiles do not have to be of the same implementation type.
        /// </summary>
        /// <param name="file">The destination ZephyrFile.</param>
        /// <param name="overwrite">Should the ZephyrFile overwrite existing ZephyrFile if it exists.</param>
        /// <param name="createMissingDirectories">Create any missing directories in the path to the file.</param>
        /// <param name="stopOnError">Throw an exception if the move fails.</param>
        /// <param name="verbose">Log details of the file being moved.</param>
        /// <param name="callbackLabel">Optional "label" to be passed into the callback method.</param>
        /// <param name="callback">Optional method that is called for logging purposes.</param>
        public void MoveTo(ZephyrFile file, bool overwrite = true, bool createMissingDirectories = true, bool stopOnError = true, bool verbose = true, String callbackLabel = null, Action<string, string> callback = null)
        {
            try
            {
                if (file.Exists && !overwrite)
                    throw new Exception($"File [{file.FullName}] Already Exists.");

                CopyTo(file, overwrite, createMissingDirectories, stopOnError, false);
                this.Delete(stopOnError: stopOnError, verbose: false);
                if (verbose)
                    Logger.Log($"Moved File [{this.FullName}] to [{file.FullName}].", callbackLabel, callback);
            }
            catch (Exception e)
            {
                Logger.Log($"ERROR - {e.Message}", callbackLabel, callback);
                if (stopOnError)
                    throw;
            }
        }

        /// <summary>
        /// Copies a ZephyrFile into a ZephyrDirectory with the same name as the original ZephyrFile.
        /// </summary>
        /// <param name="dir">The destination directory.</param>
        /// <param name="overwrite">Should the ZephyrFile overwrite existing ZephyrFile if it exists.</param>
        /// <param name="createMissingDirectories">Create any missing directories in the path to the file.</param>
        /// <param name="stopOnError">Throw an exception if the copy fails.</param>
        /// <param name="verbose">Log details of the file being copied.</param>
        /// <param name="callbackLabel">Optional "label" to be passed into the callback method.</param>
        /// <param name="callback">Optional method that is called for logging purposes.</param>
        public void CopyTo(ZephyrDirectory dir, bool overwrite = true, bool createMissingDirectories = true, bool stopOnError = true, bool verbose = true, String callbackLabel = null, Action<string, string> callback = null)
        {
            String targetFilePath = dir.PathCombine(dir.FullName, this.Name);
            ZephyrFile targetFile = dir.CreateFile(targetFilePath);
            CopyTo(targetFile, overwrite, createMissingDirectories, stopOnError, false, callbackLabel, callback);
            if (verbose)
                Logger.Log($"Copied File [{this.FullName}] to [{dir.FullName}].", callbackLabel, callback);
        }

        /// <summary>
        /// Moves a ZephyrFile to a ZephyrDirectory with the same name as the original ZephyrFile.
        /// </summary>
        /// <param name="dir">The destination directory.</param>
        /// <param name="overwrite">Should the ZephyrFile overwrite existing ZephyrFile if it exists.</param>
        /// <param name="createMissingDirectories">Create any missing directories in the path to the file.</param>
        /// <param name="stopOnError">Throw an exception if the move fails.</param>
        /// <param name="verbose">Log details of the file being moved.</param>
        /// <param name="callbackLabel">Optional "label" to be passed into the callback method.</param>
        /// <param name="callback">Optional method that is called for logging purposes.</param>
        public void MoveTo(ZephyrDirectory dir, bool overwrite = true, bool createMissingDirectories = true, bool stopOnError = true, bool verbose = true, String callbackLabel = null, Action<string, string> callback = null)
        {
            CopyTo(dir, createMissingDirectories, overwrite, stopOnError, false, callbackLabel, callback);
            this.Delete(stopOnError: stopOnError, verbose: false);
            if (verbose)
                Logger.Log($"Moved File [{this.FullName}] to [{dir.FullName}].", callbackLabel, callback);
        }

        /// <summary>
        /// Closes, then opens the underlying Steam object of this ZephyrFile.
        /// </summary>
        /// <param name="access">Specifies to open Stream with "Read" or "Write" access.</param>
        /// <param name="callbackLabel">Optional "label" to be passed into the callback method.</param>
        /// <param name="callback">Optional method that is called for logging purposes.</param>
        /// <returns></returns>
        public Stream Reopen(AccessType access, bool verbose = true, String callbackLabel = null, Action<string, string> callback = null)
        {
            if (IsOpen)
                Close(verbose, callbackLabel, callback);

            return Open(access, verbose, callbackLabel, callback);
        }

        /// <summary>
        /// Reads all the lines from the ZephyrFile.
        /// </summary>
        /// <param name="callbackLabel">Optional "label" to be passed into the callback method.</param>
        /// <param name="callback">Optional method that is called for logging purposes.</param>
        /// <returns>An array of strings, one string for each line.</returns>
        public string[] ReadAllLines(bool verbose = true, String callbackLabel = null, Action<string, string> callback = null)
        {
            List<string> lines = new List<string>();

            Reopen(AccessType.Read, verbose, callbackLabel, callback);

            using (StreamReader reader = new StreamReader(this.Stream))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                    lines.Add(line);
            }

            Close(verbose, callbackLabel, callback);

            return lines.ToArray();
        }

        /// <summary>
        /// Reads the entire text of a ZephyrFile.
        /// </summary>
        /// <param name="callbackLabel">Optional "label" to be passed into the callback method.</param>
        /// <param name="callback">Optional method that is called for logging purposes.</param>
        /// <returns>A string containing the entire text of the ZephyrFile.</returns>
        public string ReadAllText(bool verbose = true, String callbackLabel = null, Action<string, string> callback = null)
        {
            string text = null;
            Reopen(AccessType.Read, verbose, callbackLabel, callback);
            StreamReader reader = new StreamReader(this.Stream);
            text = reader.ReadToEnd();

            Close(verbose, callbackLabel, callback);
            return text;
        }

        /// <summary>
        /// Reads the entire content of a ZephyrFile.
        /// </summary>
        /// <param name="callbackLabel">Optional "label" to be passed into the callback method.</param>
        /// <param name="callback">Optional method that is called for logging purposes.</param>
        /// <returns>A byte array containing the contents of the ZephyrFile.</returns>
        public byte[] ReadAllBytes(bool verbose = true, String callbackLabel = null, Action<string, string> callback = null)
        {
            Reopen(AccessType.Read, verbose, callbackLabel, callback);

            // Logic From : https://stackoverflow.com/questions/1080442/how-to-convert-an-stream-into-a-byte-in-c
            byte[] readBuffer = new byte[4096];
            int totalBytesRead = 0;
            int bytesRead;

            while ((bytesRead = this.Stream.Read(readBuffer, totalBytesRead, readBuffer.Length - totalBytesRead)) > 0)
            {
                totalBytesRead += bytesRead;
                if (totalBytesRead == readBuffer.Length)
                {
                    int nextByte = Stream.ReadByte();
                    if (nextByte != -1)
                    {
                        byte[] temp = new byte[readBuffer.Length * 2];
                        Buffer.BlockCopy(readBuffer, 0, temp, 0, readBuffer.Length);
                        Buffer.SetByte(temp, totalBytesRead, (byte)nextByte);
                        readBuffer = temp;
                        totalBytesRead++;
                    }
                }
            }

            byte[] buffer = readBuffer;
            if (readBuffer.Length != totalBytesRead)
            {
                buffer = new byte[totalBytesRead];
                Buffer.BlockCopy(readBuffer, 0, buffer, 0, totalBytesRead);
            }

            Close();

            return buffer;
        }

        /// <summary>
        /// Writes all "lines" to the ZephyrFile.
        /// </summary>
        /// <param name="lines">A string array of lines to write.</param>
        /// <param name="callbackLabel">Optional "label" to be passed into the callback method.</param>
        /// <param name="callback">Optional method that is called for logging purposes.</param>
        public void WriteAllLines(string[] lines, bool verbose = true, String callbackLabel = null, Action<string, string> callback = null)
        {
            Reopen(AccessType.Write, verbose, callbackLabel, callback);

            StreamWriter writer = new StreamWriter(this.Stream);
            foreach (string line in lines)
                writer.WriteLine(line);

            writer.Flush();
            Flush(verbose, callbackLabel, callback);
            Close(verbose, callbackLabel, callback);
        }

        /// <summary>
        /// Writes a string to a ZephyrFile.
        /// </summary>
        /// <param name="text">The text to write.</param>
        /// <param name="callbackLabel">Optional "label" to be passed into the callback method.</param>
        /// <param name="callback">Optional method that is called for logging purposes.</param>
        public void WriteAllText(string text, bool verbose = true, String callbackLabel = null, Action<string, string> callback = null)
        {
            Reopen(AccessType.Write, verbose, callbackLabel, callback);

            StreamWriter writer = new StreamWriter(this.Stream);
            writer.Write(text);
            writer.Flush();
            Flush(verbose, callbackLabel, callback);
            Close(verbose, callbackLabel, callback);
        }

        /// <summary>
        /// Writes the contents of a byte array to a ZephyrFile.
        /// </summary>
        /// <param name="bytes">The bytes to write.</param>
        /// <param name="callbackLabel">Optional "label" to be passed into the callback method.</param>
        /// <param name="callback">Optional method that is called for logging purposes.</param>
        public void WriteAllBytes(byte[] bytes, bool verbose = true, String callbackLabel = null, Action<string, string> callback = null)
        {
            Reopen(AccessType.Write, verbose, callbackLabel, callback);
            this.Stream.Write(bytes, 0, bytes.Length);
            Flush(verbose, callbackLabel, callback);
            Close(verbose, callbackLabel, callback);
        }
    }
}

