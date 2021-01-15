using System.Collections.Generic;
using Faultify.Analyzers.OpcodeAnalyzer;
using Mono.Cecil.Cil;

namespace Faultify.Analyzers
{
    /// <summary>
    ///     Analyzer that searches for possible boolean branching mutations inside a method definition.
    ///     Mutations such as 'if(condition)' to 'if(!condition)'.
    /// </summary>
    public class BooleanBranchMutationAnalyzer : OpCodeMutationAnalyzer
    {
        private static readonly Dictionary<OpCode, IEnumerable<OpCode>> Bitwise =
            new Dictionary<OpCode, IEnumerable<OpCode>>
            {
                // Opcodes for mutating 'if(condition)' to 'if(!condition)' or unconditional conditions.
                {OpCodes.Brtrue, new[] {OpCodes.Brfalse}},
                {OpCodes.Brtrue_S, new[] {OpCodes.Brfalse_S}},

                // Opcodes for mutating 'if(!condition)' to 'if(condition)' or unconditional conditions.
                {OpCodes.Brfalse, new[] {OpCodes.Brtrue}},
                {OpCodes.Brfalse_S, new[] {OpCodes.Brtrue_S}}
            };

        public BooleanBranchMutationAnalyzer() : base(Bitwise)
        {
        }

        public override string Description =>
            "Analyzer that searches for possible boolean branch mutations such as such as 'if(condition)' to 'if(!condition).";

        public override string Name => "Boolean Branch Analyzer";
    }
}