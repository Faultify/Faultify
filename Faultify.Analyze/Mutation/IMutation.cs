namespace Faultify.Analyze.Mutation
{
    /// <summary>
    ///     Mutation that can be performed or reverted.
    /// </summary>
    public interface IMutation
    {
        /// <summary>
        ///     Mutates the the bytecode to its mutated version.
        /// </summary>
        void Mutate();

        /// <summary>
        ///     Reverts the mutation to its original.
        /// </summary>
        void Reset();
    }
}