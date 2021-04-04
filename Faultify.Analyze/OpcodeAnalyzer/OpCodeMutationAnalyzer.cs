using System;
using System.Collections.Generic;
using System.Linq;
using Faultify.Analyze.Mutation;
using Faultify.Analyze.MutationGroups;
using Mono.Cecil.Cil;

namespace Faultify.Analyze.OpcodeAnalyzer
{
    /// <summary>
    ///     Analyzer that searches for possible opcode mutations inside a method definition.
    ///     A list with opcodes definitions can be found here: https://en.wikipedia.org/wiki/List_of_CIL_instructions
    /// </summary>
    public abstract class
        OpCodeMutationAnalyzer : IMutationAnalyzer<OpCodeMutation, Instruction>
    {
        private readonly Dictionary<OpCode, IEnumerable<(MutationLevel, OpCode)>> _mappedOpCodes;

        protected OpCodeMutationAnalyzer(Dictionary<OpCode, IEnumerable<(MutationLevel, OpCode)>> mappedOpCodes)
        {
            _mappedOpCodes = mappedOpCodes;
        }

        public abstract string Description { get; }
        public abstract string Name { get; }

        public IMutationGroup<OpCodeMutation> GenerateMutations(Instruction scope, MutationLevel mutationLevel)
        {
            var original = scope.OpCode;
            IEnumerable<OpCodeMutation> mutations;

            try
            {
                var targets = _mappedOpCodes[original];
                mutations =
                    from target
                    in targets
                    where mutationLevel.HasFlag(target.Item1)
                    select new OpCodeMutation
                    {
                        Original = original,
                        Replacement = target.Item2,
                        Instruction = scope
                    };
            } 
            catch
            {
                Console.Error.WriteLine($"Could not find key in Dictionary: {original}."); // TODO: Use proper logging.
                mutations = Enumerable.Empty<OpCodeMutation>();
            }

            return new MutationGroup<OpCodeMutation>
            {
                Name = Name,
                Description = Description,
                Mutations = mutations
            };
        }
    }
}