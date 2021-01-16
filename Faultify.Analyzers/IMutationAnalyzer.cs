using System.Collections.Generic;
using Faultify.Analyzers.Mutation;

namespace Faultify.Analyzers
{
    /// <summary>
    ///     Interface for analyzers that search for possible source code mutations on byte code level.
    /// </summary>
    /// <typeparam name="TMutation">The type of the returned metadata.</typeparam>
    /// <typeparam name="TScope"></typeparam>
    public interface IMutationAnalyzer<TMutation, in TScope> where TMutation : IMutation
    {
        /// <summary>
        ///     Name of the mutator.
        /// </summary>
        string Description { get; }

        /// <summary>
        ///     Name of the mutator.
        /// </summary>
        string Name { get; }

        /// <summary>
        ///     Analyzes possible mutations in the given scope.
        ///     Returns the mutation that can be either executed or reverted.
        /// </summary>
        IEnumerable<TMutation> AnalyzeMutations(TScope scope, MutationLevel mutationLevel);
    }
}