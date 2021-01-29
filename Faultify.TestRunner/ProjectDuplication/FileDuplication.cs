using System;
using System.IO;
using System.IO.MemoryMappedFiles;

namespace Faultify.TestRunner.ProjectDuplication
{
    /// <summary>
    /// Wrapper over duplicated testproject files.
    /// TODO: Implement memory mapped files.
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
        /// Name of the file.
        /// </summary>
        public string Name { get; set; }
        
        /// <summary>
        /// Directory in which the file is located.
        /// </summary>
        public string Directory { get; set; }
        
        /// <summary>
        /// Retrieves the full file path.
        /// </summary>
        /// <returns></returns>
        public string FullFilePath()
        {
            return Path.Combine(Directory, Name);
        }
        
        /// <summary>
        /// Returns whether write mode for the file stream is enabled.
        /// </summary>
        /// <returns></returns>
        public bool WriteModesEnabled()
        {
            return _fileStream.CanWrite;
        }

        /// <summary>
        /// Returns whether read mode for the file stream is enabled.
        /// </summary>
        /// <returns></returns>
        public bool ReadModesEnabled()
        {
            return _fileStream.CanRead;
        }

        /// <summary>
        /// Opens up a write access to the file and returns the stream.
        /// </summary>
        /// <returns></returns>
        public Stream OpenReadWriteStream()
        {
            if (_fileStream == null ||  ReadModesEnabled())
            {
                EnableReadWriteOnly();
            }
            return _fileStream;
        }


        /// <summary>
        /// Opens up a read access to the file and returns the stream.
        /// </summary>
        /// <returns></returns>
        public Stream OpenReadStream()
        {
            if (_fileStream == null || WriteModesEnabled())
            {
                EnableReadOnly();
            }

            return _fileStream;
        }

        /// <summary>
        /// Enables write modes and closes any earlier initialized streams. 
        /// </summary>
        public void EnableReadWriteOnly()
        {
            this.Dispose();

            _fileStream = new FileStream(FullFilePath(), FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite);
        }

        /// <summary>
        /// Enables read modes and closes any earlier initialized streams. 
        /// </summary>
        public void EnableReadOnly()
        {
            this.Dispose();

            _fileStream = new FileStream(FullFilePath(), FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
        }

        public void Dispose()
        {
            _fileStream = null;
        }
    }
}