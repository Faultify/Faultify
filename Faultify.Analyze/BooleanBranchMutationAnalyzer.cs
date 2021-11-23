using System.Collections.Generic;
using Faultify.Analyze.OpcodeAnalyzer;
using Mono.Cecil.Cil;

namespace Faultify.Analyze
{
    /// <summary>
    ///     Analyzer that searches for possible boolean branching mutations inside a method definition.
    ///     Mutations such as 'if(condition)' to 'if(!condition)'.
    /// </summary>
    public class BooleanBranchMutationAnalyzer : OpCodeMutationAnalyzer
    {
        private static readonly Dictionary<OpCode, IEnumerable<(MutationLevel, OpCode)>> Bitwise =
            new()
            {
                // Opcodes for mutating 'if(condition)' to 'if(!condition)' or unconditional conditions.
                { OpCodes.Brtrue, new[] { (MutationLevel.Simple, OpCodes.Brfalse) } },
                { OpCodes.Brtrue_S, new[] { (MutationLevel.Simple, OpCodes.Brfalse_S) } },

                // Opcodes for mutating 'if(!condition)' to 'if(condition)' or unconditional conditions.
                { OpCodes.Brfalse, new[] { (MutationLevel.Simple, OpCodes.Brtrue) } },
                { OpCodes.Brfalse_S, new[] { (MutationLevel.Simple, OpCodes.Brtrue_S) } }
            };

        public BooleanBranchMutationAnalyzer() : base(Bitwise)
        {
        }

        public override string Description =>
            "Analyzer that searches for possible boolean branch mutations such as such as 'if(condition)' to 'if(!condition).";

        public override string Name => "Boolean Branch Analyzer";
    }
}