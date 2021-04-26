using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Faultify.Core.ProjectAnalyzing;

namespace Faultify.TestRunner.ProjectDuplication
{
    public class TestProjectDuplicator
    {
        private readonly string _testDirectory;

        private DirectoryInfo? _newDirInfo;

        private List<string>? _allFiles;

        private IProjectInfo? _projectInfo;

        public TestProjectDuplicator(string testDirectory)
        {
            _testDirectory = testDirectory;
        }

        /// <summary>
        ///     Create the very first duplication of the project that will be used for coverage calculations
        ///     and as a reference for all other duplications
        /// </summary>
        /// <param name="testProject"></param>
        /// <returns> the initial duplication</returns>
        public TestProjectDuplication MakeInitialCopy(IProjectInfo testProject)
        {
            DirectoryInfo? dirInfo = new DirectoryInfo(_testDirectory);
            _projectInfo = testProject;
            // Remove useless folders.
            foreach (var directory in dirInfo.GetDirectories("*"))
            {
                Match? match = Regex.Match(directory.Name,
                    "(^cs$|^pl$|^rt$|^de$|^en$|^es$|^fr$|^it$|^ja$|^ko$|^ru$|^zh-Hans$|^zh-Hant$|^tr$|^pt-BR$|^test-duplication-\\d+$)");

                if (match.Captures.Count != 0) Directory.Delete(directory.FullName, true);
            }

            List<TestProjectDuplication>? testProjectDuplications = new List<TestProjectDuplication>();

            // Start the initial copy
            _allFiles = Directory.GetFiles(_testDirectory, "*.*", SearchOption.AllDirectories).ToList();
            _newDirInfo = Directory.CreateDirectory(Path.Combine(_testDirectory, "test-duplication-0"));


            foreach (var file in _allFiles)
            {
                try
                {
                    FileInfo? mFile = new FileInfo(file);

                    if (mFile.Directory.FullName == _newDirInfo.Parent.FullName)
                    {
                        string? newPath = Path.Combine(_newDirInfo.FullName, mFile.Name);
                        mFile.MoveTo(newPath);
                    }
                    else
                    {
                        string? path = mFile.FullName.Replace(_newDirInfo.Parent.FullName, "");
                        FileInfo? newPath = new FileInfo(Path.Combine(_newDirInfo.FullName, path.Trim('\\')));

                        if (!Directory.Exists(newPath.DirectoryName)) Directory.CreateDirectory(newPath.DirectoryName);

                        mFile.MoveTo(newPath.FullName, true);
                    }
                }
                catch (Exception e)
                {
                    throw e;
                }
            }

            IEnumerable<FileDuplication>? initialCopies = testProject.ProjectReferences
                .Select(x => new FileDuplication(_newDirInfo.FullName, Path.GetFileNameWithoutExtension(x) + ".dll"));
            TestProjectDuplication? testProjectDuplication = new TestProjectDuplication(
                new FileDuplication(_newDirInfo.FullName, Path.GetFileName(testProject.AssemblyPath)),
                initialCopies,
                0
            );

            return testProjectDuplication;
        }

        private static void CopyFilesRecursively(DirectoryInfo source, DirectoryInfo target)
        {
            foreach (var dir in source.GetDirectories())
                CopyFilesRecursively(dir, target.CreateSubdirectory(dir.Name));
            foreach (var file in source.GetFiles())
                file.CopyTo(Path.Combine(target.FullName, file.Name));
        }

        /// <summary>
        ///     Create a copy based on the initial duplication's data
        /// </summary>
        /// <param name="i">the ID of the duplication</param>
        /// <returns> The newly created duplication </returns>
        public TestProjectDuplication MakeCopy(int i)
        {
            if (_newDirInfo == null || _projectInfo == null) throw new NullReferenceException();
            
            string duplicatedDirectoryPath = Path.Combine(_testDirectory, $"test-duplication-{i + 1}");
            
            CopyFilesRecursively(_newDirInfo, Directory.CreateDirectory(duplicatedDirectoryPath));
            
            IEnumerable<FileDuplication> duplicatedAssemblies = _projectInfo
                .ProjectReferences
                .Select(x =>
                    new FileDuplication(duplicatedDirectoryPath, Path.GetFileNameWithoutExtension(x) + ".dll"));

            return
                new TestProjectDuplication(
                    new FileDuplication(
                        duplicatedDirectoryPath,
                        Path.GetFileName(_projectInfo.AssemblyPath)),
                    duplicatedAssemblies,
                    i);
        }
    }
}
