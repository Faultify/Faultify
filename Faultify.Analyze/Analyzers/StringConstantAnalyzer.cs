using System;
using System.Collections.Generic;
using Faultify.Analyze.Mutation;
using Faultify.Analyze.MutationGroups;
using Mono.Cecil;
using Mono.Cecil.Cil;

namespace Faultify.Analyze.Analyzers
{
    /// <summary>
    ///     Analyzer that searches for possible string constant mutations inside a type definition.
    ///     Mutations such as 'hello' to a GUID like '0f8fad5b-d9cb-469f-a165-70867728950e'.
    /// </summary>
    [Obsolete("Use ConstantAnalyzer", true)]
    public class StringConstantAnalyzer : ConstantAnalyzer
    {
        private readonly RandomValueGenerator _rng = new RandomValueGenerator();

        public new string Description =>
            "Analyzer that searches for possible string constant mutations such as 'hello' to a GUID like '0f8fad5b-d9cb-469f-a165-70867728950e'.";

        public new string Name => "String ConstantMutation Analyzer";

        public new IMutationGroup<ConstantMutation> GenerateMutations(FieldDefinition field, MutationLevel mutationLevel, IDictionary<Instruction, SequencePoint> debug = null)
        {
            var constantMutation = new ConstantMutation
            {
                Original = field.Constant,
                ConstantName = field.Name,
                Replacement = null,
                ConstantField = field
            };

            Type type = field.Constant.GetType();

            if (type == typeof(string))
            {
                constantMutation.Replacement = _rng.GenerateValueForField(type, field.Constant);
            }

            var mutations = new List<ConstantMutation> { constantMutation };

            return new MutationGroup<ConstantMutation>
            {
                Name = Name,
                Description = Description,
                Mutations = mutations
            };
        }
    }
}