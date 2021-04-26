using System;
using System.Collections.Generic;
using Mono.Cecil;
using Mono.Cecil.Cil;

namespace Faultify.Core.Extensions
{
    public static class InstructionExtensions
    {
        //list of opcodes
        private static readonly List<OpCode> OpCodeList = new List<OpCode>
        {
            OpCodes.Ldc_I4_1,
            OpCodes.Ldc_I4_2,
            OpCodes.Ldc_I4_3,
            OpCodes.Ldc_I4_4,
            OpCodes.Ldc_I4_5,
            OpCodes.Ldc_I4_6,
            OpCodes.Ldc_I4_7,
            OpCodes.Ldc_I4_8,
            OpCodes.Ldc_I4_0,
            OpCodes.Ldc_I4,
            OpCodes.Ldc_I8,
            OpCodes.Ldc_R4,
            OpCodes.Ldc_R8,
            OpCodes.Ldc_I4_M1,
            OpCodes.Ldc_I4_S,
        };

        public static bool IsDynamicArray(this Instruction instruction)
        {
            try
            {
                return
                    instruction.OpCode == OpCodes.Newarr
                    && instruction.Next.OpCode == OpCodes.Dup
                    && instruction.Next.Next.OpCode == OpCodes.Ldtoken
                    && instruction.Next.Next.Next.OpCode == OpCodes.Call;
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
            switch (Type.GetTypeCode(reference.ToSystemType()))
            {
                case TypeCode.Int32: //int
                case TypeCode.UInt32: //uint
                case TypeCode.Boolean: //bool
                case TypeCode.Byte: //byte
                case TypeCode.SByte: //sbyt
                case TypeCode.Int16: //short
                case TypeCode.UInt16: //ushort
                case TypeCode.Char: //char
                    return OpCodes.Ldc_I4;
                case TypeCode.Int64: //long
                case TypeCode.UInt64: //ulong
                    return OpCodes.Ldc_I8;
                case TypeCode.Single: //float
                    return OpCodes.Ldc_R4;
                case TypeCode.Double: //double
                    return OpCodes.Ldc_R8;
                case TypeCode.String: //string
                    return OpCodes.Ldloc_0;
                default:
                    return OpCodes.Nop;
            }
        }

        public static OpCode GetStelemByTypeReference(this TypeReference reference)
        {
            switch (Type.GetTypeCode(reference.ToSystemType()))
            {
                case TypeCode.Int32: //int
                case TypeCode.UInt32: //uint
                case TypeCode.Boolean: //bool
                    return OpCodes.Stelem_I4;
                case TypeCode.Byte: //byte
                case TypeCode.SByte: //sbyte
                    return OpCodes.Stelem_I1;
                case TypeCode.Int16: //short
                case TypeCode.UInt16: //ushort
                case TypeCode.Char: //char
                    return OpCodes.Stelem_I2;
                case TypeCode.Int64: //long
                case TypeCode.UInt64: //ulong
                    return OpCodes.Stelem_I8;
                case TypeCode.Single: //float
                    return OpCodes.Stelem_R4;
                case TypeCode.Double: //double
                    return OpCodes.Stelem_R8;
                case TypeCode.String: //string
                    return OpCodes.Stelem_Ref;
                default:
                    return OpCodes.Nop;
            }
        }

        public static bool IsLdc(this Instruction instruction)
        {
            return OpCodeList.Contains(instruction.OpCode);
        }
    }
}
