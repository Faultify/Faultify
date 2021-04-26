using Mono.Cecil;

namespace Faultify.Analyze.ArrayMutationStrategy
{
    /// <summary>
    ///     Interface for mutation strategies that mutate an array and are able to revert it back to original.
    /// </summary>
    public interface IArrayMutationStrategy
    {
        /// <summary>
        ///     Mutates the array according to the implemented strategy.
        /// </summary>
        public void Mutate();

        /// <summary>
        ///     Resets the array back to original according to the implemented strategy.
        /// </summary>
        /// <param name="methodBody"></param>
        /// <param name="methodClone"></param>
        public void Reset(MethodDefinition methodBody, MethodDefinition methodClone);
    }
}
