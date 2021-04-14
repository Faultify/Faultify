using System.Collections.Generic;
using Mono.Cecil.Cil;

namespace Faultify.Analyze.Analyzers
{
    /// <summary>
    ///     Analyzer that searches for possible arithmetic mutations inside a method definition.
    ///     Mutations such as '+' to '-', '*', '/', and '%'.
    /// </summary>
    public class ArithmeticAnalyzer : OpCodeAnalyzer
    {
        private static readonly Dictionary<OpCode, IEnumerable<(MutationLevel, OpCode)>> Arithmetic =
            new Dictionary<OpCode, IEnumerable<(MutationLevel, OpCode)>>
            {
                // Opcodes for mutating arithmetic operation: '+' to '-' ,  '*',  '/' , and '%'.
                {
                    OpCodes.Add, new[]
                    {
                        (MutationLevel.Simple, OpCodes.Sub),
                        (MutationLevel.Medium, OpCodes.Mul),
                        (MutationLevel.Detailed, OpCodes.Div),
                        (MutationLevel.Detailed, OpCodes.Rem)
                    }
                },

                // Opcodes for mutating arithmetic operation: '-' to '+' ,  '*',  '/' , and '%'.
                {
                    OpCodes.Sub, new[]
                    {
                        (MutationLevel.Simple, OpCodes.Add),
                        (MutationLevel.Medium, OpCodes.Mul),
                        (MutationLevel.Detailed, OpCodes.Div),
                        (MutationLevel.Detailed, OpCodes.Rem)
                    }
                },

                // Opcodes for mutating arithmetic operation: '*' to '+' ,  '-',  '/' , and '%'.
                {
                    OpCodes.Mul, new[]
                    {
                        (MutationLevel.Simple, OpCodes.Add),
                        (MutationLevel.Medium, OpCodes.Sub),
                        (MutationLevel.Detailed, OpCodes.Div),
                        (MutationLevel.Detailed, OpCodes.Rem)
                    }
                },

                // Opcodes for mutating arithmetic operation: '/' to '+' ,  '-',  '*' , and '%'.
                {
                    OpCodes.Div, new[]
                    {
                        (MutationLevel.Simple, OpCodes.Add),
                        (MutationLevel.Medium, OpCodes.Mul),
                        (MutationLevel.Detailed, OpCodes.Sub),
                        (MutationLevel.Detailed, OpCodes.Rem)
                    }
                },

                // Opcodes for mutating arithmetic operation: '%' to '+' ,  '-',  '*' , and '/'.
                {
                    OpCodes.Rem, new[]
                    {
                        (MutationLevel.Simple, OpCodes.Add),
                        (MutationLevel.Medium, OpCodes.Mul),
                        (MutationLevel.Detailed, OpCodes.Div),
                        (MutationLevel.Detailed, OpCodes.Sub)
                    }
                }
            };

        public ArithmeticAnalyzer() : base(Arithmetic)
        {
        }

        public override string Description =>
            "Analyzer that searches for possible arithmetic mutations such as '+' to '-', '*', '/', and '%'.";

        public override string Name => "Arithmetic Analyzer";
    }
}