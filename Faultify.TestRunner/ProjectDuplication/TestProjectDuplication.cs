using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Faultify.Analyze;
using Faultify.Analyze.AssemblyMutator;
using Faultify.Core.ProjectAnalyzing;
using Faultify.TestRunner.TestRun;
using NLog;

namespace Faultify.TestRunner.ProjectDuplication
{
    /// <summary>
    ///     A test project duplication.
    /// </summary>
    public class TestProjectDuplication : IDisposable
    {
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

        public TestProjectDuplication(
            FileDuplication testProjectFile,
            IEnumerable<FileDuplication> testProjectReferences,
            int duplicationNumber
        )
        {
            TestProjectFile = testProjectFile;
            TestProjectReferences = testProjectReferences;
            DuplicationNumber = duplicationNumber;
        }

        /// <summary>
        ///     Test project references.
        /// </summary>
        public IEnumerable<FileDuplication> TestProjectReferences { get; set; }

        /// <summary>
        ///     Test project file handle.
        /// </summary>
        public FileDuplication TestProjectFile { get; set; }

        /// <summary>
        ///     Number indicating which duplication this test project is.
        /// </summary>
        public int DuplicationNumber { get; }

        /// <summary>
        ///     Indicates if the test project is currently used by any test runner.
        /// </summary>
        public bool IsInUse { get; set; }

        public void Dispose()
        {
            TestProjectFile.Dispose();
            foreach (var fileDuplication in TestProjectReferences) fileDuplication.Dispose();
        }

        /// <summary>
        ///     Event that notifies when ever this test project is given free by a given test runner.
        /// </summary>
        public event EventHandler<TestProjectDuplication>? TestProjectFreed;

        /// <summary>
        ///     Mark this project as free for any test runner.
        /// </summary>
        public void MarkAsFree()
        {
            IsInUse = false;
            TestProjectFreed?.Invoke(this, this);
        }


        /// <summary>
        ///     Delete the test project completely
        ///     Currently does not work, given that Nunit restricts access to the files awaits been given
        ///     TODO: This is not used
        /// </summary>
        public void DeleteTestProject()
        {
            Directory.Delete(TestProjectFile.Directory, true);
        }

        /// <summary>
        ///     Returns a list of <see cref="MutationVariant" /> that can be executed on this given test project duplication.
        /// </summary>
        /// <param name="mutationIdentifiers"></param>
        /// <param name="mutationLevel"></param>
        /// <returns></returns>
        public IList<MutationVariant> GetMutationVariants(
            IList<MutationVariantIdentifier>? mutationIdentifiers,
            MutationLevel mutationLevel
        )
        {
            List<MutationVariant> foundMutations = new List<MutationVariant>();

            foreach (var reference in TestProjectReferences)
            {
                // Read the reference and its contents
                using Stream stream = reference.OpenReadStream();
                using BinaryReader binReader = new BinaryReader(stream);
                byte[] data = binReader.ReadBytes((int) stream.Length);

                CodeDecompiler? decompiler = new CodeDecompiler(reference.FullFilePath(), new MemoryStream(data));

                // Create assembly mutator and look up the mutations according to the passed identifiers.
                AssemblyMutator assembly = new AssemblyMutator(reference.FullFilePath());
                HashSet<string> toMutateMethods = new HashSet<string>(
                    mutationIdentifiers?.Select(x => x.MemberName) ?? Enumerable.Empty<string>()
                );

                foreach (TypeScope type in assembly.Types)
                {
                    foreach (MethodScope method in type.Methods.Where(method =>
                        toMutateMethods.Contains(method.AssemblyQualifiedName)))
                    {
                        var methodMutationId = 0;

                        foreach (var mutationGroup in method.AllMutations(mutationLevel))
                        {
                            foreach (var mutation in mutationGroup)
                            {
                                MutationVariantIdentifier? mutationIdentifier = mutationIdentifiers?.FirstOrDefault(x =>
                                    x.MutationId == methodMutationId && method.AssemblyQualifiedName == x.MemberName);

                                if (mutationIdentifier?.MemberName != null)
                                {
                                    foundMutations.Add(new MutationVariant(
                                        causesTimeOut: false,
                                        assembly: assembly,
                                        mutationIdentifier: mutationIdentifier.Value,
                                        mutationAnalyzerInfo: new MutationAnalyzerInfo
                                        {
                                            AnalyzerDescription = mutationGroup.Description,
                                            AnalyzerName = mutationGroup.Name,
                                        },
                                        memberHandle: method.Handle,
                                        mutation: mutation,
                                        mutatedSource: "",
                                        originalSource: decompiler.Decompile(method.Handle) // this might not be a good idea
                                    ));
                                }

                                methodMutationId++;
                            }
                        }
                    }
                }
            }

            return foundMutations;
        }

        /// <summary>
        ///     Flush any changes made to the passed list of mutations to the file system.
        /// </summary>
        /// <param name="mutationVariants"></param>
        public void FlushMutations(IList<MutationVariant> mutationVariants)
        {
            HashSet<AssemblyMutator>? assemblies =
                new HashSet<AssemblyMutator>(mutationVariants.Select(x => x.Assembly));
            foreach (AssemblyMutator assembly in assemblies)
            {
                FileDuplication? fileDuplication = TestProjectReferences.FirstOrDefault(x =>
                    assembly.Module.Name == x.Name);
                try
                {
                    using Stream? writeStream = fileDuplication.OpenReadWriteStream();
                    using MemoryStream? stream = new MemoryStream();
                    assembly.Module.Write(stream);
                    writeStream.Write(stream.ToArray());

                    stream.Position = 0;
                    CodeDecompiler? decompiler = new CodeDecompiler(fileDuplication.FullFilePath(), stream);

                    foreach (var mutationVariant in mutationVariants)
                    {
                        if (assembly == mutationVariant.Assembly && string.IsNullOrEmpty(mutationVariant.MutatedSource))
                        {
                            mutationVariant.MutatedSource = decompiler.Decompile(mutationVariant.MemberHandle);
                        }
                    }
                }
                catch (Exception e)
                {
                    _logger.Error(e, e.Message);
                }
                finally
                {
                    fileDuplication.Dispose();
                }
            }

            TestProjectFile.Dispose();
        }
    }
}
