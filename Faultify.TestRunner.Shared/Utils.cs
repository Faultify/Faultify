using System.IO;
using System.IO.MemoryMappedFiles;

namespace Faultify.TestRunner.Shared
{
    public static class Utils
    {
        /// <summary>
        ///     Reads the mutation coverage from the <see cref="TestRunnerConstants.CoverageFileName" /> file.
        /// </summary>
        /// <returns></returns>
        public static MutationCoverage ReadMutationCoverageFile()
        {
            using var mmf = MemoryMappedFile.OpenExisting("CoverageFile");
            using var stream = mmf.CreateViewStream();
            using var memoryStream = new MemoryStream();
            stream.CopyTo(memoryStream);
            memoryStream.Position = 0;

            return MutationCoverage.Deserialize(memoryStream.ToArray());
        }

        /// <summary>
        ///     Write the mutation coverage to the <see cref="TestRunnerConstants.CoverageFileName" /> file.
        /// </summary>
        /// <returns></returns>
        public static void WriteMutationCoverageFile(MutationCoverage mutationCoverage)
        {
            using var mmf = MemoryMappedFile.OpenExisting("CoverageFile", MemoryMappedFileRights.ReadWrite);
            WriteMutationCoverageFile(mutationCoverage, mmf);
        }

        /// <summary>
        ///     Write the mutation coverage to the <see cref="TestRunnerConstants.CoverageFileName" /> file.
        /// </summary>
        /// <returns></returns>
        public static void WriteMutationCoverageFile(MutationCoverage mutationCoverage, MemoryMappedFile coverageFile)
        {
            using var stream = coverageFile.CreateViewStream();
            stream.Write(mutationCoverage.Serialize());
            stream.Flush();
        }

        public static MemoryMappedFile CreateCoverageMemoryMappedFile()
        {
            var file = File.Create(TestRunnerConstants.CoverageFileName);
            file.Dispose();

            var fileStream = new FileStream(TestRunnerConstants.CoverageFileName, FileMode.Open, FileAccess.ReadWrite,
                FileShare.ReadWrite);
            return MemoryMappedFile.CreateFromFile(fileStream, "CoverageFile", 20000, MemoryMappedFileAccess.ReadWrite,
                HandleInheritability.None, true);
        }
    }
}