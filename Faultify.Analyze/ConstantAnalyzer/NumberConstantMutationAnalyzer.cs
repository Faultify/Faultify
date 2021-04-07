using System;
using System.Collections.Generic;
using Faultify.Analyze.Mutation;
using Faultify.Analyze.MutationGroups;
using Mono.Cecil;

namespace Faultify.Analyze.ConstantAnalyzer
{
    /// <summary>
    ///     Analyzer that searches for possible integer constant mutations inside a type definition.
    ///     Mutations such as '5' to a random int like '971231'.
    ///     Supports: int, double, long, short, sbyte, uint, ulong, ushort, byte and float.
    ///     It is possible to choose what types to mutate by initiating the class with a hashset of types
    /// </summary>
    public class NumberConstantMutationAnalyzer : ConstantMutationAnalyzer
    {
        private readonly RandomValueGenerator _rng = new RandomValueGenerator();

        public override string Description =>
            "Analyzer that searches for possible number constant mutations such as '5' to a random int like '971231'.";

        public override string Name => "Number ConstantMutation Analyzer";


        public override IMutationGroup<ConstantMutation> GenerateMutations(FieldDefinition field,
            MutationLevel mutationLevel)
        {
            var constantMutation = new ConstantMutation
            {
                Original = field.Constant,
                ConstantName = field.Name,
                Replacement = null,
                ConstantField = field
            };

            Type type = field.Constant.GetType();

            if (TypeChecker.IsConstantType(type)) {
                constantMutation.Replacement = _rng.GenerateValueForField(type, field.Constant);
            }

            var mutations = new List<ConstantMutation>{ constantMutation };

            return new MutationGroup<ConstantMutation>
            {
                Name = Name,
                Description = Description,
                Mutations = mutations
            };
        }
    }
}