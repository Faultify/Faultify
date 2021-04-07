using System.Collections.Generic;
using Mono.Cecil.Cil;

namespace Faultify.Analyze.Analyzers
{
    /// <summary>
    ///     Analyzer that searches for possible comparison operator mutations.
    ///     Mutations such as that a condition is made false, like '<' to '>'.
    ///     With conditional operators we only have to find one way to invalidate the condition
    ///     as opposed to the arithmetic operators where each operator has to be replaced.
    ///     This mutator mutates two kinds of instructions 'branch instructions' and 'comparison' institutions.
    ///     The branch instruction like (beq, bgt, blt) are available in various permutations, but in all circumstances it is
    ///     essentially a goto statement.
    ///     Most branch instructions are conditional and based on a comparative expression or a Boolean condition.
    ///     Compare instructions like (ceq, cgt, clt) perform a comparison on the top two values of the evaluation stack.
    ///     The two values are replaced on the evaluation stack with the result of the comparison, which is either true or
    ///     false.
    ///     The comparison should be between related types.
    ///     As a convenience, branch and compare instructions are combinable.
    ///     The combined instruction compares the top two values of the evaluation stack and branches on the result.
    ///     These are called comparative branching instructions.
    ///     Instead of requiring two instructions to perform the test, only one is needed.
    ///     The '<=' Boolean operator in the source code translates to the 'bgt' instruction in the IL (meaning branch on greater).
    /// 
    ///     You might have expected '<=' to translate to 'ble' (branch on less than or equal),
    ///     but it turns out that the compiler will usually optimize control flow by translating a Boolean operator into its IL complement branching instruction.
    ///     Hense it can be the case that on different computers IL code for comparison can be a bit different.
    /// </summary>
    public class ComparisonAnalyzer : OpCodeAnalyzer
    {
        private static readonly Dictionary<OpCode, IEnumerable<(MutationLevel, OpCode)>> Branch =
            new Dictionary<OpCode, IEnumerable<(MutationLevel, OpCode)>>
            {
                // Opcodes for mutating Comparison operator: '==' to '!='(or unordered).
                {OpCodes.Beq, new[] {(MutationLevel.Simple, OpCodes.Bne_Un)}},

                // Opcodes for mutating Comparison operator: '>=' to '<'.
                {OpCodes.Bge, new[] {(MutationLevel.Simple, OpCodes.Blt)}},

                // Opcodes for mutating Comparison operator: '>=' (unsigned or unordered) to '<' (unsigned or unordered).
                {OpCodes.Bge_Un, new[] {(MutationLevel.Simple, OpCodes.Blt_Un)}},

                // Opcodes for mutating Comparison operator: '>' to '<='.
                {OpCodes.Bgt, new[] {(MutationLevel.Simple, OpCodes.Ble)}},

                // Opcodes for mutating Comparison operator: '>' (unsigned or unordered) to '<=' (unsigned or unordered).
                {OpCodes.Bgt_Un, new[] {(MutationLevel.Simple, OpCodes.Ble_Un)}},

                // Opcodes for mutating Comparison operator: '<=' to '>'.
                {OpCodes.Ble, new[] {(MutationLevel.Simple, OpCodes.Bgt)}},

                // Opcodes for mutating Comparison operator: '<=' (unsigned or unordered) to '>' (unsigned or unordered).
                {OpCodes.Ble_Un, new[] {(MutationLevel.Simple, OpCodes.Bgt_Un)}},

                // Opcodes for mutating Comparison operator: '<' to '>='.
                {OpCodes.Blt, new[] {(MutationLevel.Simple, OpCodes.Bge)}},

                // Opcodes for mutating Comparison operator: '<' (unsigned or unordered) to '>=' (unsigned or unordered).
                {OpCodes.Blt_Un, new[] {(MutationLevel.Simple, OpCodes.Bge_Un)}},

                // Opcodes for mutating Comparison operator: '!='(or unordered) to '=='. 
                {OpCodes.Bne_Un, new[] {(MutationLevel.Simple, OpCodes.Beq)}},

                // Opcodes for mutating Comparison operator: '==' to '<'  without branch redirection.
                {OpCodes.Ceq, new[] {(MutationLevel.Simple, OpCodes.Clt)}},

                // Opcodes for mutating Comparison operator: '<' to '>'  without branch redirection.
                {OpCodes.Clt, new[] {(MutationLevel.Simple, OpCodes.Cgt)}},

                // Opcodes for mutating Comparison operator: '<' (unsigned or unordered) to '>' (unsigned or unordered)  without branch redirection.
                {OpCodes.Clt_Un, new[] {(MutationLevel.Simple, OpCodes.Cgt_Un)}},

                // Opcodes for mutating Comparison operator: '>' to '<'  without branch redirection.
                {OpCodes.Cgt, new[] {(MutationLevel.Simple, OpCodes.Clt)}},

                // Opcodes for mutating Comparison operator: '>' (unsigned or unordered) to '<' (unsigned or unordered)  without branch redirection.
                {OpCodes.Cgt_Un, new[] {(MutationLevel.Simple, OpCodes.Clt_Un)}}
            };

        public ComparisonAnalyzer() : base(Branch)
        {
        }

        public override string Description =>
            "Analyzer that searches for possible comparison mutations to invalidate a condition such as '<' to '>'.";

        public override string Name => "Comparison Analyzer";
    }
}