using System.Collections.Generic;
using Faultify.Analyzers.Mutation;
using Mono.Cecil;

namespace Faultify.Analyzers.ConstantAnalyzer
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

        public NumberConstantMutationAnalyzer()
        {
            Mapped = new TypeCollection();
            Mapped.AddIntegerTypes();
        }

        public override string Description =>
            "Analyzer that searches for possible number constant mutations such as '5' to a random int like '971231'.";

        public override string Name => "Number ConstantMutation Analyzer";

        public TypeCollection Mapped { get; }

        public override IEnumerable<ConstantMutation> AnalyzeMutations(FieldDefinition field, MutationLevel mutationLevel)
        {
            var constantMutation = new ConstantMutation
            {
                Original = field.Constant,
                ConstantName = field.Name,
                Replacement = null,
                ConstantField = field
            };

            if (Mapped.Types.TryGetValue(field.Constant.GetType(), out var fieldType))
            {
                constantMutation.Replacement = _rng.GenerateValueForField(fieldType, field.Constant);

                yield return constantMutation;
            }
        }
    }
}