using System.Collections.Generic;
using Faultify.Analyze.Groupings;
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
        IEnumerable<IMutationGrouping<IMutation>> AllMutations(MutationLevel mutationLevel);
    }
}