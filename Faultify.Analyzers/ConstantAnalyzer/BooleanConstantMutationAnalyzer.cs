using System.Collections.Generic;
using Faultify.Analyzers.Mutation;
using Mono.Cecil;

namespace Faultify.Analyzers.ConstantAnalyzer
{
    /// <summary>
    ///     Analyzer that searches for possible boolean constant mutations inside a type definition.
    ///     Mutations such as 'true' to 'false'.
    /// </summary>
    public class BooleanConstantMutationAnalyzer : ConstantMutationAnalyzer
    {
        public override string Description =>
            "Analyzer that searches for possible boolean constant mutations such as 'true' to 'false'.";

        public override string Name => "Boolean ConstantMutation Analyzer";

        public override IEnumerable<ConstantMutation> AnalyzeMutations(FieldDefinition field, MutationLevel mutationLevel)
        {
            if (field.Constant is bool original)
                yield return new ConstantMutation
                {
                    Original = original,
                    ConstantName = field.Name,
                    Replacement = !original,
                    ConstantField = field
                };
        }
    }
}