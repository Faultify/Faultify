using System.Collections.Generic;
using System.Linq;
using Faultify.Analyzers.Mutation;
using Mono.Cecil.Cil;

namespace Faultify.Analyzers.OpcodeAnalyzer
{
    /// <summary>
    ///     Analyzer that searches for possible opcode mutations inside a method definition.
    ///     A list with opcodes definitions can be found here: https://en.wikipedia.org/wiki/List_of_CIL_instructions
    /// </summary>
    public abstract class
        OpCodeMutationAnalyzer : IMutationAnalyzer<OpCodeMutation, Instruction>
    {
        private readonly Dictionary<OpCode, IEnumerable<OpCode>> _mappedOpCodes;

        protected OpCodeMutationAnalyzer(Dictionary<OpCode, IEnumerable<OpCode>> mappedOpCodes)
        {
            _mappedOpCodes = mappedOpCodes;
        }

        public abstract string Description { get; }

        public abstract string Name { get; }

        public IEnumerable<OpCodeMutation> AnalyzeMutations(Instruction scope)
        {
            // Store original opcode for a reset later on.
            var original = scope.OpCode;

            // Try to get the instruction opcode from the mapped mappedOpCodes.
            return _mappedOpCodes.TryGetValue(original, out var mutations)
                ? mutations.Select(mutant => new OpCodeMutation
                    {Original = original, Replacement = mutant, Instruction = scope})
                : Enumerable.Empty<OpCodeMutation>();
        }
    }
}