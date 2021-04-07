using System;
using System.Collections.Generic;
using Faultify.Analyze.Mutation;
using Faultify.Analyze.MutationGroups;
using Mono.Cecil;

namespace Faultify.Analyze.Analyzers
{
    /// <summary>
    ///     Analyzer that searches for possible string constant mutations inside a type definition.
    ///     Mutations such as 'hello' to a GUID like '0f8fad5b-d9cb-469f-a165-70867728950e'.
    /// </summary>
    public class StringConstantAnalyzer : ConstantAnalyzer
    {
        public override string Description =>
            "Analyzer that searches for possible string constant mutations such as 'hello' to a GUID like '0f8fad5b-d9cb-469f-a165-70867728950e'.";

        public override string Name => "String ConstantMutation Analyzer";

        public override IMutationGroup<ConstantMutation> GenerateMutations(FieldDefinition field, MutationLevel mutationLevel)
        {

            var mutations = new List<ConstantMutation>();

            if (field.Constant is string original)
            {
                mutations.Add(new ConstantMutation
                {
                    Original = original,
                    ConstantName = field.Name,
                    Replacement = Guid.NewGuid().ToString(),
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