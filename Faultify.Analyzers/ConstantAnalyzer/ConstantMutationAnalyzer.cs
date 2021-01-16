using System.Collections.Generic;
using Faultify.Analyzers.Mutation;
using Mono.Cecil;

namespace Faultify.Analyzers.ConstantAnalyzer
{
    /// <summary>
    ///     Analyzer that searches for possible constant mutations inside a type definition, this class is the parent class to
    ///     all constant analyzers.
    /// </summary>
    public abstract class ConstantMutationAnalyzer : IMutationAnalyzer<ConstantMutation, FieldDefinition>
    {
        public abstract string Description { get; }

        public abstract string Name { get; }

        public abstract IEnumerable<ConstantMutation> AnalyzeMutations(FieldDefinition field, MutationLevel mutationLevel);
    }
}