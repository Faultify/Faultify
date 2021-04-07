using System.Collections.Generic;
using System.Linq;
using Faultify.Analyze.ArrayMutationStrategy;
using Faultify.Analyze.MutationGroups;
using Faultify.Analyze.Mutation;
using Faultify.Core.Extensions;
using Mono.Cecil;
using Mono.Cecil.Cil;
using System;

namespace Faultify.Analyze.Analyzers
{
    /// <summary>
    ///     Analyzer that searches for possible array mutations inside a method definition.
    ///     Limitations:
    ///     - Only one-dimensional arrays.
    ///     - Only arrays that are created dynamically with more than 2 values which are not default values.
    ///     - Only array of the following types: double, float, long, ulong, int, uint, byte, sbyte, short, ushort, char,
    ///     boolean.
    /// </summary>
    public class ArrayAnalyzer : IAnalyzer<ArrayMutation, MethodDefinition>
    {

        public string Description => "Analyzer that searches for possible array mutations.";

        public string Name => "Array Analyzer";

        public IMutationGroup<ArrayMutation> GenerateMutations(MethodDefinition method, MutationLevel mutationLevel)
        {
            // Filter and map arrays
            IEnumerable<ArrayMutation> arrayMutations =
                from instruction
                in method.Body.Instructions
                where instruction.IsDynamicArray() && isArrayType(instruction)
                select new ArrayMutation(new DynamicArrayRandomizerStrategy(method), method);

            // Build Mutation Group
            return new MutationGroup<ArrayMutation>
            {
                Name = Name,
                Description = Description,
                Mutations = arrayMutations
            };
        }

        private bool isArrayType(Instruction newarr)
        {
            // Cast generic operand into its system type
            var type = ((TypeReference)newarr.Operand).ToSystemType();
            return TypeChecker.IsArrayType(type);
        }
    }
}