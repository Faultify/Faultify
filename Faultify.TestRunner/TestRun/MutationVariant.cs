using System.Collections.Generic;
using Faultify.Analyzers.AssemblyMutator;
using Faultify.Analyzers.Groupings;
using Faultify.Analyzers.Mutation;

namespace Faultify.TestRunner.TestRun
{
    /// <summary>
    ///     Single mutation variant.
    /// </summary>
    public class MutationVariant
    {
        public MutationVariant(IMutation mutation, HashSet<string> testNames, IMutationGrouping<IMutation> parentGroup,
            AssemblyMutator assembly, FaultifyMethodDefinition method)
        {
            TestCoverage = testNames;
            Assembly = assembly;
            Method = method;
            ParentGroup = parentGroup;
            Mutation = mutation;
        }

        public bool CausesTimeOut { get; set; } = false;

        public HashSet<string> TestCoverage { get; set; }
        public IMutationGrouping<IMutation> ParentGroup { get; }
        public AssemblyMutator Assembly { get; }
        public FaultifyMethodDefinition Method { get; }
        public IMutation Mutation { get; set; }
        public string MutatedSource { get; set; }
        public string OriginalSource { get; set; }
    }
}