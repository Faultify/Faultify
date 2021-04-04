using System.Collections.Generic;
using Faultify.Analyze.MutationGroups;
using Faultify.Analyze.Mutation;

namespace Faultify.Analyze.AssemblyMutator
{
    /// <summary>
    ///     Defines an interface for mutation providers.
    /// </summary>
    internal interface IMutationProvider
    {
        /// <summary>
        ///     Returns all possible mutations.
        /// </summary>
        /// <returns></returns>
        IEnumerable<IMutationGroup<IMutation>> AllMutations(MutationLevel mutationLevel);
    }
}