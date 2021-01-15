using System.Collections.Generic;
using Mono.Cecil.Cil;

namespace Faultify.Analyzers.OpcodeAnalyzer
{
    /// <summary>
    ///     Analyzer that searches for possible bitwise mutations inside a method definition.
    ///     Mutations such as such as 'and' and 'xor'.
    /// </summary>
    public class BitwiseMutationAnalyzer : OpCodeMutationAnalyzer
    {
        private static readonly Dictionary<OpCode, IEnumerable<OpCode>> Bitwise =
            new Dictionary<OpCode, IEnumerable<OpCode>>
            {
                // Opcodes for mutation bitwise operator: '|' to '&' , and '^'. 
                {OpCodes.Or, new[] {OpCodes.And, OpCodes.Xor}},

                // Opcodes for mutation bitwise operator: '&' to '|' , and '^'. 
                {OpCodes.And, new[] {OpCodes.Or, OpCodes.Xor}},

                // Opcodes for mutation bitwise operator: '^' to '|' , and '&'. 
                {OpCodes.Xor, new[] {OpCodes.Or, OpCodes.And}}
            };

        public BitwiseMutationAnalyzer() : base(Bitwise)
        {
        }

        public override string Description =>
            "Analyzer that searches for possible bitwise mutations such as 'or' to 'and' and 'xor'.";

        public override string Name => "Bitwise Analyzer";
    }
}