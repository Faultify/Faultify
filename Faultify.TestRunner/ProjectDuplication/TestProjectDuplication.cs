using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Faultify.Analyze;
using Faultify.Analyze.AssemblyMutator;
using Faultify.Core.ProjectAnalyzing;
using Faultify.TestRunner.TestRun;

namespace Faultify.TestRunner.ProjectDuplication
{
    /// <summary>
    /// A test project duplication. 
    /// </summary>
    public class TestProjectDuplication : IDisposable
    {
        /// <summary>
        /// Test project references.
        /// </summary>
        public IEnumerable<FileDuplication> TestProjectReferences { get; set; }

        /// <summary>
        /// Test project file handle. 
        /// </summary>
        public FileDuplication TestProjectFile { get; set; }

        /// <summary>
        /// Number indicating which duplication this test project is. 
        /// </summary>
        public int DuplicationNumber { get; }

        /// <summary>
        /// Indicates if the test project is currently used by any test runner.
        /// </summary>
        public bool IsInUse { get; set; } = false;

        /// <summary>
        /// Event that notifies when ever this test project is given free by a given test runner. 
        /// </summary>
        public event EventHandler<TestProjectDuplication> TestProjectFreed;

        public TestProjectDuplication(FileDuplication testProjectFile, IEnumerable<FileDuplication> testProjectReferences, int duplicationNumber)
        {
            TestProjectFile = testProjectFile;
            TestProjectReferences = testProjectReferences;
            DuplicationNumber = duplicationNumber;
        }

        /// <summary>
        /// Mark this project as free for any test runner.
        /// </summary>
        public void FreeTestProject()
        {
            IsInUse = false;
            TestProjectFreed?.Invoke(this, this);
        }

        /// <summary>
        /// Returns a list of <see cref="MutationVariant"/> that can be executed on this given test project duplication.
        /// </summary>
        /// <param name="mutationIdentifiers"></param>
        /// <returns></returns>
        public IList<MutationVariant> GetMutationVariants(IList<MutationVariantIdentifier> mutationIdentifiers, MutationLevel mutationLevel)
        {
            var foundMutations = new List<MutationVariant>();

            foreach (var reference in TestProjectReferences)
            {
                // Read the reference its contents
                using var stream = reference.OpenReadStream();
                using BinaryReader binReader = new BinaryReader(stream);
                var data = binReader.ReadBytes((int) stream.Length);

                var decompiler = new CodeDecompiler(reference.FullFilePath(), new MemoryStream(data));

                // Create assembly mutator and look up the mutations according to the passed identifiers.
                AssemblyMutator assembly = new AssemblyMutator(new MemoryStream(data));

                foreach (var type in assembly.Types)
                {
                    var toMutateMethods = new HashSet<string>(
                        mutationIdentifiers.Select(x => x.MemberName)
                    );

                    foreach (var method in type.Methods)
                    {
                        if (!toMutateMethods.Contains(method.Name))
                            continue;

                        int methodMutationId = 0;

                        foreach (var group in method.AllMutations(mutationLevel))
                        foreach (var mutation in group)
                        {
                            MutationVariantIdentifier mutationIdentifier = mutationIdentifiers.FirstOrDefault(x =>
                                x.MutationId == methodMutationId && method.Name == x.MemberName);
                            
                            if (mutationIdentifier.MemberName != null)
                            {
                                foundMutations.Add(new MutationVariant()
                                {
                                    Assembly = assembly,
                                    CausesTimeOut = false,
                                    MemberHandle = method.Handle,
                                    OriginalSource = decompiler.Decompile(method.Handle),
                                    MutatedSource = "",
                                    Mutation = mutation,
                                    MutationAnalyzerInfo = new MutationAnalyzerInfo()
                                    {
                                        AnalyzerDescription = group.AnalyzerDescription,
                                        AnalyzerName = group.AnalyzerName
                                    },
                                    MutationIdentifier = mutationIdentifier
                                });
                            }

                            methodMutationId++;
                        }
                    }
                }
            }

            return foundMutations;
        }

        /// <summary>
        /// Flush any changes made to the passed list of mutations to the file system.
        /// </summary>
        /// <param name="mutationVariants"></param>
        public void FlushMutations(IList<MutationVariant> mutationVariants)
        {
            var assemblies = new HashSet<AssemblyMutator>(mutationVariants.Select(x => x.Assembly));

            foreach (var assembly in assemblies)
            {
                var fileDuplication = TestProjectReferences.FirstOrDefault(x =>
                    assembly.Module.Name == x.Name);

                using var writeStream = fileDuplication.OpenReadWriteStream();
                using MemoryStream ms = new MemoryStream();
                assembly.Module.Write(ms);
                writeStream.Write(ms.ToArray());

                ms.Position = 0;
                var decompiler = new CodeDecompiler(fileDuplication.FullFilePath(), ms);

                foreach (var mutationVariant in mutationVariants)
                {
                    if (assembly == mutationVariant.Assembly && string.IsNullOrEmpty(mutationVariant.MutatedSource))
                    {
                        mutationVariant.MutatedSource = decompiler.Decompile(mutationVariant.MemberHandle);
                    }
                }
                fileDuplication.Dispose();
            }

            TestProjectFile.Dispose();
        }

        public void Dispose()
        {
            TestProjectFile.Dispose();
            foreach (var fileDuplication in TestProjectReferences)
            {
                fileDuplication.Dispose();
            }
        }
    }
}