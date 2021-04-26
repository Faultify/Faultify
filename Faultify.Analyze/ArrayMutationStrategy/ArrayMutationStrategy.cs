using System;
using Mono.Cecil;
using Mono.Cecil.Cil;

namespace Faultify.Analyze.ArrayMutationStrategy
{
    /// <summary>
    ///     Abstract class with reset, optimize and simplify logic which are the same for all strategies.
    /// </summary>
    [Obsolete("Use DynamicArrayMutationStrategy", true)]
    public abstract class ArrayMutationStrategy : IArrayMutationStrategy
    {
        public abstract void Mutate();

        /// <summary>
        ///     Clears the mutated method body and pushes the original method body.
        /// </summary>
        /// <param name="mutatedMethodDef"></param>
        /// <param name="methodClone"></param>
        public void Reset(MethodDefinition mutatedMethodDef, MethodDefinition methodClone)
        {
            mutatedMethodDef.Body.Instructions.Clear();
            foreach (Instruction instruction in methodClone.Body.Instructions)
                mutatedMethodDef.Body.Instructions.Add(instruction);
        }
    }
}
