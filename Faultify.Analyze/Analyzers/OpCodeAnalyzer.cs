using System;
using System.Collections.Generic;
using System.Linq;
using Faultify.Analyze.Mutation;
using Faultify.Analyze.MutationGroups;
using Mono.Cecil.Cil;
using NLog;

namespace Faultify.Analyze.Analyzers
{
    /// <summary>
    ///     Analyzer that searches for possible opcode mutations inside a method definition.
    ///     A list with opcodes definitions can be found here: https://en.wikipedia.org/wiki/List_of_CIL_instructions
    /// </summary>
    public abstract class OpCodeAnalyzer : IAnalyzer<OpCodeMutation, Instruction>
    {
        private readonly Dictionary<OpCode, IEnumerable<(MutationLevel, OpCode)>> _mappedOpCodes;
        public abstract string Description { get; }
        public abstract string Name { get; }
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

        protected OpCodeAnalyzer(Dictionary<OpCode, IEnumerable<(MutationLevel, OpCode)>> mappedOpCodes)
        {
            _mappedOpCodes = mappedOpCodes;
        }

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
            catch (Exception e)
            {
                _logger.Error(e, $"Could not find key in Dictionary: {original}.");
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