using System.Collections.Generic;
using Faultify.Core.Extensions;
using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono.Cecil.Rocks;

namespace Faultify.Analyze.ArrayMutationStrategy
{
    /// <summary>
    ///     Contains Mutating Strategy for Dynamic Arrays.
    /// </summary>
    public class DynamicArrayRandomizerStrategy : IArrayMutationStrategy
    {
        private readonly RandomizedArrayBuilder _randomizedArrayBuilder;
        private readonly MethodDefinition _methodDefinition;
        private TypeReference _type;

        public DynamicArrayRandomizerStrategy(MethodDefinition methodDefinition)
        {
            _randomizedArrayBuilder = new RandomizedArrayBuilder();
            _methodDefinition = methodDefinition;
        }

        public void Reset(MethodDefinition mutatedMethodDef, MethodDefinition methodClone)
        {
            mutatedMethodDef.Body.Instructions.Clear();
            foreach (var instruction in methodClone.Body.Instructions)
                mutatedMethodDef.Body.Instructions.Add(instruction);
        }

        /// <summary>
        ///     Mutates a dynamic array by creating a new array with random values with the arraybuilder.
        /// </summary>
        public void Mutate()
        {
            var processor = _methodDefinition.Body.GetILProcessor();
            _methodDefinition.Body.SimplifyMacros();

            var length = 0;
            var beforeArray = new List<Instruction>();
            var afterArray = new List<Instruction>();
            
            int index = 0;

            // Find array to replace
            foreach (var instruction in _methodDefinition.Body.Instructions)
            {
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
                    _type = (TypeReference)instruction.Operand;

                    var previous = instruction.Previous;
                    var call = instruction.Next.Next.Next;
                    // get length of array.
                    length = (int)previous.Operand;

                    // Add all other nodes to the list.

                    if (_type.FullName == "System.Boolean" || _type.ToSystemType() == typeof(string))
                    {
                        var instructionNumber = _methodDefinition.Body.Instructions.Count - 6;
                        while (instructionNumber < _methodDefinition.Body.Instructions.Count)
                        {
                            afterArray.Add(_methodDefinition.Body.Instructions[instructionNumber]);
                            instructionNumber++;
                        }
                    }
                    else
                    {
                        var next = call.Next;
                        while (next != null)
                        {
                            afterArray.Add(next);
                            next = next.Next;
                        }
                    }

                    break;
                }
                index++;
            }


            object[] data = new object[length];;
            if(_type.ToSystemType() == typeof(bool))
            {
                while (index < _methodDefinition.Body.Instructions.Count)
                {
                    if(_methodDefinition.Body.Instructions[index].OpCode.Name == OpCodes.Ldc_I4.ToString())
                    {
                        data[(int)_methodDefinition.Body.Instructions[index].Operand] = true;
                        index++;
                    }
                    index++;
                }
            }
            

            processor.Clear();

            // append everything before array.
            foreach (var before in beforeArray) processor.Append(before);

            var newArray = _randomizedArrayBuilder.CreateRandomizedArray(processor, length, _type, data);

            // append new array
            foreach (var newInstruction in newArray) processor.Append(newInstruction);

            // append after array.
            /*afterArray = new List<Instruction>();*/
            

            foreach (var after in afterArray) processor.Append(after);
            _methodDefinition.Body.OptimizeMacros();
        }
    }
}