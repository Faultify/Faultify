using Mono.Cecil.Cil;

namespace Faultify.Analyzers.Mutation
{
    /// <summary>
    ///     Opcode mutation that can be performed or reverted.
    /// </summary>
    public class OpCodeMutation : IMutation
    {
        /// <summary>
        ///     The original opcode.
        /// </summary>
        public OpCode Original;

        /// <summary>
        ///     The replacement for the original opcode.
        /// </summary>
        public OpCode Replacement;

        /// <summary>
        ///     Reference to the instruction line in witch the opcode can be mutated.
        /// </summary>
        public Instruction Instruction { get; set; }

        public void Mutate()
        {
            Instruction.OpCode = Replacement;
        }

        public void Reset()
        {
            Instruction.OpCode = Original;
        }

        public override string ToString()
        {
            return $"Change operator from: '{Original}' to: '{Replacement}'.";
        }
    }
}