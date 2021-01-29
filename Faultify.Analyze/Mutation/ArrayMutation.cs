using Faultify.Analyze.ArrayMutationStrategy;
using Mono.Cecil;
using MonoMod.Utils;

namespace Faultify.Analyze.Mutation
{
    /// <summary>
    ///     Array Mutation, receives specific Strategy and MethodDefinition. The logic for the methods depends on a given
    ///     Strategy.
    /// </summary>
    public class ArrayMutation : IMutation
    {
        private readonly IArrayMutationStrategy _arrayMutationStrategy;
        private readonly MethodDefinition _methodDefClone;
        private readonly MethodDefinition _methodDefinitionToMutate;

        public ArrayMutation(IArrayMutationStrategy mutationStrategy, MethodDefinition methodDef)
        {
            _arrayMutationStrategy = mutationStrategy;
            _methodDefinitionToMutate = methodDef;
            _methodDefClone = _methodDefinitionToMutate.Clone();
        }

        /// <summary>
        ///     Mutates Array. Mutate logic depends on given Strategy.
        /// </summary>
        public void Mutate()
        {
            _arrayMutationStrategy.Mutate();
        }

        /// <summary>
        ///     Undo functionality for mutation array.
        /// </summary>
        public void Reset()
        {
            _arrayMutationStrategy.Reset(_methodDefinitionToMutate, _methodDefClone);
        }
    }
}