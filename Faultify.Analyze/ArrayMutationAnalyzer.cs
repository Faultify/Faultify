using System.Collections.Generic;
using System.Linq;
using Faultify.Analyze.ArrayMutationStrategy;
using Faultify.Analyze.MutationGroups;
using Faultify.Analyze.Mutation;
using Faultify.Core.Extensions;
using Mono.Cecil;
using Mono.Cecil.Cil;
using System;

namespace Faultify.Analyze
{
    /// <summary>
    ///     Analyzer that searches for possible array mutations inside a method definition.
    ///     Limitations:
    ///     - Only one-dimensional arrays.
    ///     - Only arrays that are created dynamically with more than 2 values which are not default values.
    ///     - Only array of the following types: double, float, long, ulong, int, uint, byte, sbyte, short, ushort, char,
    ///     boolean.
    /// </summary>
    public class ArrayMutationAnalyzer : IMutationAnalyzer<ArrayMutation, MethodDefinition>
    {
        public ArrayMutationAnalyzer()
        {
            Mapped = new TypeCollection();
            Mapped.Types.Add(typeof(char));
            Mapped.AddBooleanTypes();
            Mapped.AddIntegerTypes();
        }

        public TypeCollection Mapped { get; }

        public string Description => "Analyzer that searches for possible array mutations.";

        public string Name => "Array Analyzer";

        public IMutationGroup<ArrayMutation> GenerateMutations(MethodDefinition method, MutationLevel mutationLevel)
        {
            // Filter and map arrays
            IEnumerable<ArrayMutation> arrayMutations =
                from instruction
                in method.Body.Instructions
                where instruction.IsDynamicArray() && SupportedTypeCheck(instruction)
                select new ArrayMutation(new DynamicArrayRandomizerStrategy(method), method);

            // Build Mutation Group
            return new MutationGroup<ArrayMutation>
            {
                Name = Name,
                Description = Description,
                Mutations = arrayMutations
            };
        }

        /// <summary>
        ///     Checks if the to be mutated array is of a supported type
        /// </summary>
        /// <param name="newarr"></param>
        /// <returns></returns>
        private bool SupportedTypeCheck(Instruction newarr)
        {
            var type = ((TypeReference) newarr.Operand).ToSystemType();
            return Mapped.Types.Contains(type);
        }
    }
}