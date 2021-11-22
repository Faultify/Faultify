using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Faultify.Core.ProjectAnalyzing;

namespace Faultify.TestRunner.ProjectDuplication
{
    public class TestProjectDuplicator
    {
        private readonly string _testDirectory;

        public TestProjectDuplicator(string testDirectory)
        {
            _testDirectory = testDirectory;
        }

        public List<TestProjectDuplication> MakeInitialCopies(IProjectInfo testProject, int count)
        {
            var dirInfo = new DirectoryInfo(_testDirectory);

            // Remove useless folders.
            foreach (var directory in dirInfo.GetDirectories("*"))
            {
                var match = Regex.Match(directory.Name,
                    "(^cs$|^pl$|^rt$|^de$|^en$|^es$|^fr$|^it$|^ja$|^ko$|^ru$|^zh-Hans$|^zh-Hant$|^tr$|^pt-BR$|^test-duplication-\\d+$)");

                if (match.Captures.Count != 0) Directory.Delete(directory.FullName, true);
            }

            var testProjectDuplications = new List<TestProjectDuplication>();

            // Start the initial copy
            var allFiles = Directory.GetFiles(_testDirectory, "*.*", SearchOption.AllDirectories).ToList();
            var newDirInfo = Directory.CreateDirectory(Path.Combine(_testDirectory, "test-duplication-0"));


            foreach (var file in allFiles)
                try
                {
                    var mFile = new FileInfo(file);

                    if (mFile.Directory.FullName == newDirInfo.Parent.FullName)
                    {
                        var newPath = Path.Combine(newDirInfo.FullName, mFile.Name);
                        mFile.MoveTo(newPath);
                    }
                    else
                    {
                        var path = mFile.FullName.Replace(newDirInfo.Parent.FullName, "");
                        var newPath = new FileInfo(Path.Combine(newDirInfo.FullName, path.Trim('\\')));

                        if (!Directory.Exists(newPath.DirectoryName)) Directory.CreateDirectory(newPath.DirectoryName);

                        mFile.MoveTo(newPath.FullName, true);
                    }
                }
                catch (Exception e)
                {
                    throw e;
                }

            var initialCopies = testProject.ProjectReferences
                .Select(x => new FileDuplication(newDirInfo.FullName, Path.GetFileNameWithoutExtension(x) + ".dll"));
            testProjectDuplications.Add(new TestProjectDuplication(
                new FileDuplication(newDirInfo.FullName, Path.GetFileName(testProject.AssemblyPath)),
                initialCopies,
                0
            ));

            // Copy the initial copy N times.
            Parallel.ForEach(Enumerable.Range(1, count), i =>
            {
                var duplicatedDirectoryPath = Path.Combine(_testDirectory, $"test-duplication-{i}");
                CopyFilesRecursively(newDirInfo, Directory.CreateDirectory(duplicatedDirectoryPath));
                var duplicatedAsseblies = testProject.ProjectReferences
                    .Select(x =>
                        new FileDuplication(duplicatedDirectoryPath, Path.GetFileNameWithoutExtension(x) + ".dll"));

                testProjectDuplications.Add(
                    new TestProjectDuplication(
                        new FileDuplication(duplicatedDirectoryPath, Path.GetFileName(testProject.AssemblyPath)),
                        duplicatedAsseblies,
                        i
                    )
                );
            });

            return testProjectDuplications;
        }

        private static void CopyFilesRecursively(DirectoryInfo source, DirectoryInfo target)
        {
            foreach (var dir in source.GetDirectories())
                CopyFilesRecursively(dir, target.CreateSubdirectory(dir.Name));
            foreach (var file in source.GetFiles())
                file.CopyTo(Path.Combine(target.FullName, file.Name));
        }
    }
}