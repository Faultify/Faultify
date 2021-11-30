using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono.Cecil.Rocks;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Faultify.Analyze.ArrayMutationStrategy
{
    /// <summary>
    /// Contains Mutating Strategy to make all arrays empty at initialization
    /// </summary>
    public class EmptyArrayStrategy : IArrayMutationStrategy
    {
        private readonly MethodDefinition _methodDefinition;
        private TypeReference _type;
        public EmptyArrayStrategy(MethodDefinition methodDefinition)
        {
            _methodDefinition = methodDefinition;
        }

        public void Mutate()
        {
            var processor = _methodDefinition.Body.GetILProcessor();
            _methodDefinition.Body.SimplifyMacros();

            for (int i = 0; i < _methodDefinition.Body.Instructions.Count; i++)
            {
                var instruction = _methodDefinition.Body.Instructions[i];

                if (instruction.Next != null && instruction.Next.OpCode == OpCodes.Newarr )
                {
                    processor.Body.Instructions[i].OpCode = OpCodes.Ldc_I4_0;
                }
            }

            //processor.Clear();
            _methodDefinition.Body.OptimizeMacros();
        }

        public void Reset(MethodDefinition methodBody, MethodDefinition methodClone)
        {
            methodBody.Body.Instructions.Clear();
            foreach (var instruction in methodClone.Body.Instructions)
                methodBody.Body.Instructions.Add(instruction);
        }
    }
}
