using System.Collections.Generic;
using Faultify.Core.Extensions;
using Mono.Cecil;
using Mono.Cecil.Cil;
using MonoMod.Utils;

namespace Faultify.Analyzers.ArrayMutationStrategy
{
    /// <summary>
    ///     Builder for building arrays in IL-code.
    /// </summary>
    public class ArrayBuilder
    {
        private RandomValueGenerator _randomValueGenerator;

        /// <summary>
        ///     Creates array with the given length and array type.
        /// </summary>
        /// <param name="processor"></param>
        /// <param name="length"></param>
        /// <param name="arrayType"></param>
        /// <returns></returns>
        public List<Instruction> CreateArray(ILProcessor processor, int length, TypeReference arrayType)
        {
            _randomValueGenerator = new RandomValueGenerator();
            var opcodeTypeValueAssignment = arrayType.GetLdcOpCodeByTypeReference();
            var stelem = arrayType.GetStelemByTypeReference();
            if (arrayType.ToSystemType() == typeof(long) || arrayType.ToSystemType() == typeof(ulong))
                opcodeTypeValueAssignment = OpCodes.Ldc_I4;

            var list = new List<Instruction>
            {
                processor.Create(OpCodes.Ldc_I4, length),
                processor.Create(OpCodes.Newarr, arrayType)
            };
            for (var i = 0; i < length; i++)
            {
                var random = _randomValueGenerator.GenerateValueForField(arrayType.ToSystemType(), 0);

                list.Add(processor.Create(OpCodes.Dup));

                if (length > 2147483647 && length < -2147483647) list.Add(processor.Create(OpCodes.Ldc_I8, i));
                else list.Add(processor.Create(OpCodes.Ldc_I4, i));
                list.Add(processor.Create(opcodeTypeValueAssignment, random));

                if (arrayType.ToSystemType() == typeof(long) || arrayType.ToSystemType() == typeof(ulong))
                    list.Add(processor.Create(OpCodes.Conv_I8));
                list.Add(processor.Create(stelem));
            }

            return list;
        }
    }
}