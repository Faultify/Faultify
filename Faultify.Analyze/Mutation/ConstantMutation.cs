using Mono.Cecil;

namespace Faultify.Analyze.Mutation
{
    /// <summary>
    ///     Constant mutation that can be performed or reverted.
    /// </summary>
    public class ConstantMutation : IMutation
    {
        /// <summary>
        ///     The name of the constant.
        /// </summary>
        public string ConstantName { get; set; }

        /// <summary>
        ///     The original constant value.
        /// </summary>
        public object Original { get; set; }

        /// <summary>
        ///     The replacement for the ConstantMutation value.
        /// </summary>
        public object Replacement { get; set; }

        /// <summary>
        ///     Reference to the constant field that can be mutated.
        /// </summary>
        public FieldDefinition ConstantField { get; set; }

        public void Mutate()
        {
            ConstantField.Constant = Replacement;
        }

        public void Reset()
        {
            ConstantField.Constant = Original;
        }

        public string Report => $"Change constant '{ConstantName}' from: '{Original}' to: '{Replacement}'.";
    }
}
