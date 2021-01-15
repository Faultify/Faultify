using System.Collections.Generic;
using Faultify.Analyzers.Groupings;
using Faultify.Analyzers.Mutation;

namespace Faultify.Analyzers.AssemblyMutator
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
        IEnumerable<IMutationGrouping<IMutation>> AllMutations();
    }
}