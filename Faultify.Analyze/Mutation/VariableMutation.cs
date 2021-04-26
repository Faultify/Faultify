using Mono.Cecil.Cil;

namespace Faultify.Analyze.Mutation
{
    public class VariableMutation : IMutation
    {
        /// <summary>
        ///     The original variable value.
        /// </summary>
        public object Original { get; set; }

        /// <summary>
        ///     The replacement for the variable value.
        /// </summary>
        public object Replacement { get; set; }

        /// <summary>
        ///     The replacement for the variable value.
        /// </summary>
        public int LineNumber { get; set; }

        /// <summary>
        ///     Reference to the variable instruction that can be mutated.
        /// </summary>
        public Instruction Variable { get; set; }

        public void Mutate()
        {
            Variable.Operand = Replacement;
        }

        public void Reset()
        {
            Variable.Operand = Original;
        }


        public string Report
        {
            get
            {
                if (LineNumber == -1)
                {
                    return $"Change variable from: '{Original}' to: '{Replacement}'.";
                }

                return $"Change variable from: '{Original}' to: '{Replacement}'. In line {LineNumber}";
            }
        }
    }
}
