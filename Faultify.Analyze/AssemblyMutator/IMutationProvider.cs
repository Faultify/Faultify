using System.Collections.Generic;
using Faultify.Analyze.Mutation;
using Faultify.Analyze.MutationGroups;

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
