using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono.Cecil.Rocks;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Faultify.Core.Extensions;

namespace Faultify.Analyze.ArrayMutationStrategy
{
    /// <summary>
    /// Contains Mutating Strategy to make all arrays empty at initialization
    /// </summary>
    public class EmptyArrayStrategy : IArrayMutationStrategy
    {
        private readonly ArrayBuilder _arrayBuilder;
        private readonly MethodDefinition _methodDefinition;
        private TypeReference _type;

        public EmptyArrayStrategy(MethodDefinition methodDefinition)
        {
            _arrayBuilder = new ArrayBuilder();
            _methodDefinition = methodDefinition;
        }

        public void Reset(MethodDefinition methodBody, MethodDefinition methodClone)
        {
            methodBody.Body.Instructions.Clear();
            foreach (var instruction in methodClone.Body.Instructions)
                methodBody.Body.Instructions.Add(instruction);
        }

        public void Mutate()
        {
            var processor = _methodDefinition.Body.GetILProcessor();
            _methodDefinition.Body.SimplifyMacros();

            var beforeArray = new List<Instruction>();
            var afterArray = new List<Instruction>();

            foreach (var instruction in _methodDefinition.Body.Instructions)
            {
                if (!instruction.IsDynamicArray())
                {
                    beforeArray.Add(instruction);
                }

                // if dynamic array add all instructions after the array initialisation.
                else if (instruction.IsDynamicArray())
                {
                    beforeArray.Remove(instruction.Previous);
                    // get type of array
                    _type = (TypeReference)instruction.Operand;

                    var call = instruction.Next.Next.Next;

                    // Add all other nodes to the list.
                    var next = call.Next;
                    while (next != null)
                    {
                        afterArray.Add(next);
                        next = next.Next;
                    }

                    break;
                }
            }

            processor.Clear();

            // append everything before array.
            foreach (var before in beforeArray) processor.Append(before);

            var newArray = _arrayBuilder.CreateArray(processor, 0, _type);

            // append new array
            foreach (var newInstruction in newArray) processor.Append(newInstruction);

            // append after array.
            foreach (var after in afterArray) processor.Append(after);

            _methodDefinition.Body.OptimizeMacros();
        }
    }
}
