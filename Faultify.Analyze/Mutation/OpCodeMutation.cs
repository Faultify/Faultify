using Mono.Cecil.Cil;
using System;

namespace Faultify.Analyze.Mutation
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

        private int _lineNumber;
        public int LineNumber {
            get => _lineNumber;
            set
            {
                _lineNumber = value;
            }
        }

        public void Mutate()
        {
            Instruction.OpCode = Replacement;
        }

        public void Reset()
        {
            Instruction.OpCode = Original;
        }

        public string Report
        {
            get
            {
                if (LineNumber != -1)
                {
                    return $"Change operator from: '{Original}' to: '{Replacement}' at line {LineNumber}.";
                }
                else
                {
                    return $"Change operator from: '{Original}' to: '{Replacement}'. !!";
                }
            }
        }
    }
}