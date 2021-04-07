using System;
using Mono.Cecil;
using Mono.Cecil.Cil;

namespace Faultify.Core.Extensions
{
    public static class InstructionExtensions
    {
        public static bool IsDynamicArray(this Instruction instruction)
        {
            try
            {
                return
                    instruction.OpCode == OpCodes.Newarr &&
                    instruction.Next.OpCode == OpCodes.Dup &&
                    instruction.Next.Next.OpCode == OpCodes.Ldtoken &&
                    instruction.Next.Next.Next.OpCode == OpCodes.Call;
            }
            catch
            {
                return false;
            }
        }

        public static Type ToSystemType(this TypeReference typeRef)
        {
            return Type.GetType(typeRef.FullName);
        }

        public static OpCode GetLdcOpCodeByTypeReference(this TypeReference reference)
        {
            var systemType = reference.ToSystemType();

            if (systemType == typeof(int) || systemType == typeof(uint) || systemType == typeof(bool)
                || systemType == typeof(byte) || systemType == typeof(sbyte)
                || systemType == typeof(short) || systemType == typeof(ushort) || systemType == typeof(char))
                return OpCodes.Ldc_I4;
            if (systemType == typeof(long) || systemType == typeof(ulong))
                return OpCodes.Ldc_I8;
            if (systemType == typeof(float))
                return OpCodes.Ldc_R4;
            if (systemType == typeof(double))
                return OpCodes.Ldc_R8;
            if (systemType == typeof(string)) return OpCodes.Ldloc_0;

            return OpCodes.Nop;
        }

        public static OpCode GetStelemByTypeReference(this TypeReference reference)
        {
            var systemType = reference.ToSystemType();

            if (systemType == typeof(int) || systemType == typeof(bool) || systemType == typeof(uint))
                return OpCodes.Stelem_I4;
            if (systemType == typeof(byte) || systemType == typeof(sbyte))
                return OpCodes.Stelem_I1;
            if (systemType == typeof(short) || systemType == typeof(ushort) || systemType == typeof(char))
                return OpCodes.Stelem_I2;
            if (systemType == typeof(long) || systemType == typeof(ulong))
                return OpCodes.Stelem_I8;
            if (systemType == typeof(float))
                return OpCodes.Stelem_R4;
            if (systemType == typeof(double))
                return OpCodes.Stelem_R8;
            if (systemType == typeof(string)) return OpCodes.Stelem_Ref;

            return OpCodes.Nop;
        }

        public static bool IsLdc(this Instruction instruction)
        {
            var o = instruction.OpCode;
            return o == OpCodes.Ldc_I4_1
                   || o == OpCodes.Ldc_I4_2
                   || o == OpCodes.Ldc_I4_3
                   || o == OpCodes.Ldc_I4_4
                   || o == OpCodes.Ldc_I4_5
                   || o == OpCodes.Ldc_I4_6
                   || o == OpCodes.Ldc_I4_7
                   || o == OpCodes.Ldc_I4_8
                   || o == OpCodes.Ldc_I4_0
                   || o == OpCodes.Ldc_I4
                   || o == OpCodes.Ldc_I8
                   || o == OpCodes.Ldc_R4
                   || o == OpCodes.Ldc_R8
                   || o == OpCodes.Ldc_I4_M1
                   || o == OpCodes.Ldc_I4_S;
        }
    }
}