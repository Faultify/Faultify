using System.Collections.Generic;
using Mono.Cecil.Cil;

namespace Faultify.Analyze.Analyzers
{
    /// <summary>
    ///     Analyzer that searches for possible bitwise mutations inside a method definition.
    ///     Mutations such as such as 'and' and 'xor'.
    /// </summary>
    public class BitwiseAnalyzer : OpCodeAnalyzer
    {
        private static readonly Dictionary<OpCode, IEnumerable<(MutationLevel, OpCode)>> Bitwise =
            new Dictionary<OpCode, IEnumerable<(MutationLevel, OpCode)>>
            {
                // Opcodes for mutation bitwise operator: '|' to '&' , and '^'. 
                {
                    OpCodes.Or, new[]
                    {
                        (MutationLevel.Simple, OpCodes.And),
                        (MutationLevel.Medium, OpCodes.Xor),
                    }
                },

                // Opcodes for mutation bitwise operator: '&' to '|' , and '^'. 
                {
                    OpCodes.And, new[]
                    {
                        (MutationLevel.Simple, OpCodes.Or),
                        (MutationLevel.Medium, OpCodes.Xor),
                    }
                },

                // Opcodes for mutation bitwise operator: '^' to '|' , and '&'. 
                {
                    OpCodes.Xor, new[]
                    {
                        (MutationLevel.Simple, OpCodes.Or),
                        (MutationLevel.Medium, OpCodes.And),
                    }
                },
            };

        public BitwiseAnalyzer() : base(Bitwise) { }

        public override string Description =>
            "Analyzer that searches for possible bitwise mutations such as 'or' to 'and' and 'xor'.";

        public override string Name => "Bitwise Analyzer";
    }
}
