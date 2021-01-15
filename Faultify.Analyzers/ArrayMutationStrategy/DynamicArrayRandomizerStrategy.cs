using System.Collections.Generic;
using Faultify.Core.Extensions;
using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono.Cecil.Rocks;

namespace Faultify.Analyzers.ArrayMutationStrategy
{
    /// <summary>
    ///     Contains Mutating Strategy for Dynamic Arrays.
    /// </summary>
    public class DynamicArrayRandomizerStrategy : ArrayMutationStrategy
    {
        private readonly ArrayBuilder _arrayBuilder;
        private readonly MethodDefinition _methodDefinition;
        private TypeReference _type;

        public DynamicArrayRandomizerStrategy(MethodDefinition methodDefinition)
        {
            _arrayBuilder = new ArrayBuilder();
            _methodDefinition = methodDefinition;
        }

        /// <summary>
        ///     Mutates a dynamic array by creating a new array with random values with the arraybuilder.
        /// </summary>
        public override void Mutate()
        {
            var processor = _methodDefinition.Body.GetILProcessor();
            _methodDefinition.Body.SimplifyMacros();

            var length = 0;
            var beforeArray = new List<Instruction>();
            var afterArray = new List<Instruction>();

            // Find array to replace
            foreach (var instruction in _methodDefinition.Body.Instructions)
                // add all instruction before dynamic array to list.
                if (!instruction.IsDynamicArray())
                {
                    beforeArray.Add(instruction);
                }

                // if dynamic array add all instructions after the array initialisation.
                else if (instruction.IsDynamicArray())
                {
                    beforeArray.Remove(instruction.Previous);
                    // get type of array
                    _type = (TypeReference) instruction.Operand;

                    var previous = instruction.Previous;
                    var call = instruction.Next.Next.Next;
                    // get length of array.
                    length = (int) previous.Operand;

                    // Add all other nodes to the list.
                    var next = call.Next;
                    while (next != null)
                    {
                        afterArray.Add(next);
                        next = next.Next;
                    }

                    break;
                }

            processor.Clear();

            // append everything before array.
            foreach (var before in beforeArray) processor.Append(before);

            var newArray = _arrayBuilder.CreateArray(processor, length, _type);

            // append new array
            foreach (var newInstruction in newArray) processor.Append(newInstruction);

            // append after array.
            foreach (var after in afterArray) processor.Append(after);
            _methodDefinition.Body.OptimizeMacros();
        }
    }
}