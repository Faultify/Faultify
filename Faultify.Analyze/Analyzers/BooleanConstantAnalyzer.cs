﻿using System;
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
    [Obsolete("Use ConstantAnalyzer", true)]
    public class BooleanConstantAnalyzer : ConstantAnalyzer
    {
        private readonly RandomValueGenerator _rng = new RandomValueGenerator();

        public new string Description =>
            "Analyzer that searches for possible boolean constant mutations such as 'true' to 'false'.";

        public new string Name => "Boolean ConstantMutation Analyzer";

        public new IMutationGroup<ConstantMutation> GenerateMutations(FieldDefinition field, MutationLevel mutationLevel)
        {
            var constantMutation = new ConstantMutation
            {
                Original = field.Constant,
                ConstantName = field.Name,
                Replacement = null,
                ConstantField = field
            };

            Type type = field.Constant.GetType();

            if (type == typeof(bool))
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