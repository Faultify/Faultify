using System.Collections.Generic;
using Faultify.Analyze.Mutation;
using Faultify.Analyze.MutationGroups;
using Mono.Cecil;

namespace Faultify.Analyze.Analyzers
{
    /// <summary>
    ///     Analyzer that searches for possible boolean constant mutations inside a type definition.
    ///     Mutations such as 'true' to 'false'.
    /// </summary>
    public class BooleanConstantAnalyzer : ConstantAnalyzer
    {
        public override string Description =>
            "Analyzer that searches for possible boolean constant mutations such as 'true' to 'false'.";

        public override string Name => "Boolean ConstantMutation Analyzer";

        public override IMutationGroup<ConstantMutation> GenerateMutations(FieldDefinition field, MutationLevel mutationLevel)
        {

            var mutations = new List<ConstantMutation>();

            if (field.Constant is bool original)
            {
                mutations.Add(new ConstantMutation
                {
                    Original = original,
                    ConstantName = field.Name,
                    Replacement = !original,
                    ConstantField = field
                });

            }

            return new MutationGroup<ConstantMutation>
            {
                Name = Name,
                Description = Description,
                Mutations = mutations
            };

        }
    }
}