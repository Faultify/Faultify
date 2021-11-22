using System.Collections.Generic;
using System.Reflection.Metadata;
using Faultify.Analyze.AssemblyMutator;
using Faultify.Analyze.Mutation;

namespace Faultify.TestRunner.TestRun
{
    /// <summary>
    ///     Single mutation variant that serves as an identifier for a particular possible mutation.
    /// </summary>
    public readonly struct MutationVariantIdentifier
    {
        public MutationVariantIdentifier(HashSet<string> testNames, string memberName, int mutationId,
            int mutationGroupId)
        {
            TestCoverage = testNames;
            MemberName = memberName;
            MutationId = mutationId;
            MutationGroupId = mutationGroupId;
        }

        public int MutationId { get; }
        public int MutationGroupId { get; }
        public string MemberName { get; }
        public HashSet<string> TestCoverage { get; }
    }


    /// <summary>
    ///     Single mutation variant which contains the executable mutation.
    /// </summary>
    public class MutationVariant
    {
        public MutationVariant(
            bool causesTimeOut,
            AssemblyMutator assembly,
            MutationVariantIdentifier mutationIdentifier,
            MutationAnalyzerInfo mutationAnalyzerInfo,
            EntityHandle memberHandle,
            IMutation mutation,
            string mutatedSource,
            string originalSource
        )
        {
            CausesTimeOut = causesTimeOut;
            Assembly = assembly;
            MutationIdentifier = mutationIdentifier;
            MutationAnalyzerInfo = mutationAnalyzerInfo;
            MemberHandle = memberHandle;
            Mutation = mutation;
            MutatedSource = mutatedSource;
            OriginalSource = originalSource;
        }

        public bool CausesTimeOut { get; set; } = false;

        public AssemblyMutator Assembly { get; set; }
        public MutationVariantIdentifier MutationIdentifier { get; set; }
        public MutationAnalyzerInfo MutationAnalyzerInfo { get; set; }

        public EntityHandle MemberHandle { get; set; }
        public IMutation Mutation { get; set; }
        public string MutatedSource { get; set; }
        public string OriginalSource { get; set; }
    }

    public struct MutationAnalyzerInfo
    {
        public string AnalyzerName { get; set; }
        public string AnalyzerDescription { get; set; }
    }
}