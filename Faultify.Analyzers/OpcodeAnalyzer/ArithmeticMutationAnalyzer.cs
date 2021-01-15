using System.Collections.Generic;
using Mono.Cecil.Cil;

namespace Faultify.Analyzers.OpcodeAnalyzer
{
    /// <summary>
    ///     Analyzer that searches for possible arithmetic mutations inside a method definition.
    ///     Mutations such as '+' to '-', '*', '/', and '%'.
    /// </summary>
    public class ArithmeticMutationAnalyzer : OpCodeMutationAnalyzer
    {
        private static readonly Dictionary<OpCode, IEnumerable<OpCode>> Arithmetic =
            new Dictionary<OpCode, IEnumerable<OpCode>>
            {
                // Opcodes for mutating arithmetic operation: '+' to '-' ,  '*',  '/' , and '%'.
                {OpCodes.Add, new[] {OpCodes.Sub, OpCodes.Mul, OpCodes.Div, OpCodes.Rem}},

                // Opcodes for mutating arithmetic operation: '-' to '+' ,  '*',  '/' , and '%'.
                {OpCodes.Sub, new[] {OpCodes.Add, OpCodes.Mul, OpCodes.Div, OpCodes.Rem}},

                // Opcodes for mutating arithmetic operation: '*' to '+' ,  '-',  '/' , and '%'.
                {OpCodes.Mul, new[] {OpCodes.Add, OpCodes.Sub, OpCodes.Div, OpCodes.Rem}},

                // Opcodes for mutating arithmetic operation: '/' to '+' ,  '-',  '*' , and '%'.
                {OpCodes.Div, new[] {OpCodes.Add, OpCodes.Sub, OpCodes.Mul, OpCodes.Rem}},

                // Opcodes for mutating arithmetic operation: '%' to '+' ,  '-',  '*' , and '/'.
                {OpCodes.Rem, new[] {OpCodes.Add, OpCodes.Sub, OpCodes.Mul, OpCodes.Div}}
            };

        public ArithmeticMutationAnalyzer() : base(Arithmetic)
        {
        }

        public override string Description =>
            "Analyzer that searches for possible arithmetic mutations such as '+' to '-', '*', '/', and '%'.";

        public override string Name => "Arithmetic Analyzer";
    }
}