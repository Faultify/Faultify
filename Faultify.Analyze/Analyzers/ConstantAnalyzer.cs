using System;
using System.Collections.Generic;
using Faultify.Analyze.Mutation;
using Faultify.Analyze.MutationGroups;
using Mono.Cecil;

namespace Faultify.Analyze.Analyzers
{
    /// <summary>
    ///     Analyzer that searches for possible constant mutations inside a type definition, this class is the parent class to
    ///     all constant analyzers.
    /// </summary>
    public class ConstantAnalyzer : IAnalyzer<ConstantMutation, FieldDefinition>
    {
        private readonly RandomValueGenerator _rng = new RandomValueGenerator();

        public string Description =>
            "Analyzer that searches for possible literaal constant mutations such as 'true' to 'false', or '7' to '42'.";

        public string Name => "Boolean ConstantMutation Analyzer";

        public IMutationGroup<ConstantMutation> GenerateMutations(FieldDefinition field, MutationLevel mutationLevel)
        {
            // Make a new mutation list
            List<ConstantMutation> mutations = new List<ConstantMutation>();

            // If the type is valid, create a mutation and add it to the list
            Type type = field.Constant.GetType();

            if (TypeChecker.IsConstantType(type))
            {
                var constantMutation = new ConstantMutation
                {
                    Original = field.Constant,
                    ConstantName = field.Name,
                    Replacement = _rng.GenerateValueForField(type, field.Constant),
                    ConstantField = field
                };
                mutations.Add(constantMutation);
            }

            // Build mutation group
            return new MutationGroup<ConstantMutation>
            {
                Name = Name,
                Description = Description,
                Mutations = mutations
            };
        }
    }
}