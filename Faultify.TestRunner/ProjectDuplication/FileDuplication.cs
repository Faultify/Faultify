using System;
using System.IO;

namespace Faultify.TestRunner.ProjectDuplication
{
    /// <summary>
    ///     Wrapper over duplicated testproject files.
    ///     TODO: Implement memory mapped files.
    /// </summary>
    public class FileDuplication : IDisposable
    {
        private FileStream _fileStream;

        public FileDuplication(string directory, string name)
        {
            Directory = directory;
            Name = name;
        }

        /// <summary>
        ///     Name of the file.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        ///     Directory in which the file is located.
        /// </summary>
        public string Directory { get; set; }

        public void Dispose()
        {
            if (_fileStream != null) _fileStream.Close();
            _fileStream = null;
        }

        /// <summary>
        ///     Retrieves the full file path.
        /// </summary>
        /// <returns></returns>
        public string FullFilePath()
        {
            return Path.Combine(Directory, Name);
        }

        /// <summary>
        ///     Returns whether write mode for the file stream is enabled.
        /// </summary>
        /// <returns></returns>
        public bool WriteModesEnabled()
        {
            return _fileStream.CanWrite;
        }

        /// <summary>
        ///     Returns whether read mode for the file stream is enabled.
        /// </summary>
        /// <returns></returns>
        public bool ReadModesEnabled()
        {
            return _fileStream.CanRead;
        }

        /// <summary>
        ///     Opens up a write access to the file and returns the stream.
        /// </summary>
        /// <returns></returns>
        public Stream OpenReadWriteStream()
        {
            if (_fileStream == null || ReadModesEnabled()) EnableReadWriteOnly();
            return _fileStream;
        }


        /// <summary>
        ///     Opens up a read access to the file and returns the stream.
        /// </summary>
        /// <returns></returns>
        public Stream OpenReadStream()
        {
            if (_fileStream == null || WriteModesEnabled()) EnableReadOnly();

            return _fileStream;
        }

        /// <summary>
        ///     Enables write modes and closes any earlier initialized streams.
        /// </summary>
        public void EnableReadWriteOnly()
        {
            Dispose();

            _fileStream = new FileStream(FullFilePath(), FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite);
        }

        /// <summary>
        ///     Enables read modes and closes any earlier initialized streams.
        /// </summary>
        public void EnableReadOnly()
        {
            Dispose();

            _fileStream = new FileStream(FullFilePath(), FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
        }

        ///// <summary>
        /////     Copying the file
        ///// </summary>
        //public FileDuplication CopyFile(int i)
        //{
        //    string fileName = "clone" + i.ToString() + "_" + Name;
        //    string copyDestination = Directory + "\\..";
        //    string copyDirectory = Path.Combine(copyDestination, fileName);
        //    File.Create(copyDirectory);

        //    string sourceFile = Path.Combine(Directory + "\\..", fileName);


        //    string sourcePath = Path.Combine(Directory + "\\..", Name);
        //    try
        //    {
        //    File.Copy(sourceFile, copyDirectory, true);
        //    } catch (Exception e)
        //    {
        //        Console.WriteLine(e.StackTrace);
        //    }
        //    if (System.IO.Directory.Exists(sourcePath))
        //    {
        //        string[] files = System.IO.Directory.GetFiles(sourcePath);

        //        foreach (string s in files)
        //        {
        //            fileName = Path.GetFileName(s);
        //            copyDirectory = Path.Combine(Directory, fileName);
        //            File.Copy(s, copyDirectory, true);
        //        }
        //    }
        //    else
        //    {
        //        Console.WriteLine("Source path does not exist!");
        //    }
        //    return new FileDuplication(Directory, fileName);
        //}
    }
}