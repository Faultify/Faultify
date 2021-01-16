using System;
using System.Collections.Generic;
using Faultify.Analyzers.Mutation;
using Mono.Cecil;

namespace Faultify.Analyzers.ConstantAnalyzer
{
    /// <summary>
    ///     Analyzer that searches for possible string constant mutations inside a type definition.
    ///     Mutations such as 'hello' to a GUID like '0f8fad5b-d9cb-469f-a165-70867728950e'.
    /// </summary>
    public class StringConstantMutationAnalyzer : ConstantMutationAnalyzer
    {
        public override string Description =>
            "Analyzer that searches for possible string constant mutations such as 'hello' to a GUID like '0f8fad5b-d9cb-469f-a165-70867728950e'.";

        public override string Name => "String ConstantMutation Analyzer";

        public override IEnumerable<ConstantMutation> AnalyzeMutations(FieldDefinition field, MutationLevel mutationLevel)
        {
            if (field.Constant is string original)
                yield return new ConstantMutation
                {
                    Original = original,
                    ConstantName = field.Name,
                    Replacement = Guid.NewGuid().ToString(),
                    ConstantField = field
                };
        }
    }
}