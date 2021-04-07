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
            switch (Type.GetTypeCode(reference.ToSystemType()))
            {
                case TypeCode.Int32:        //int
                case TypeCode.UInt32:       //uint
                case TypeCode.Boolean:      //bool
                case TypeCode.Byte:         //byte
                case TypeCode.SByte:        //sbyt
                case TypeCode.Int16:        //short
                case TypeCode.UInt16:       //ushort
                case TypeCode.Char:         //char
                    return OpCodes.Ldc_I4;
                case TypeCode.Int64:        //long
                case TypeCode.UInt64:       //ulong
                    return OpCodes.Ldc_I8;
                case TypeCode.Single:       //float
                    return OpCodes.Ldc_R4;
                case TypeCode.Double:       //double
                    return OpCodes.Ldc_R8;
                case TypeCode.String:       //string
                    return OpCodes.Ldloc_0;
                default:
                    return OpCodes.Nop;
            }
        }

        public static OpCode GetStelemByTypeReference(this TypeReference reference)
        {

            switch (Type.GetTypeCode(reference.ToSystemType()))
            {
                case TypeCode.Int32:        //int
                case TypeCode.UInt32:       //uint
                case TypeCode.Boolean:      //bool
                    return OpCodes.Stelem_I4;
                case TypeCode.Byte:         //byte
                case TypeCode.SByte:        //sbyte
                    return OpCodes.Stelem_I1;
                case TypeCode.Int16:        //short
                case TypeCode.UInt16:       //ushort
                case TypeCode.Char:         //char
                    return OpCodes.Stelem_I2;
                case TypeCode.Int64:        //long
                case TypeCode.UInt64:       //ulong
                    return OpCodes.Stelem_I8;
                case TypeCode.Single:       //float
                    return OpCodes.Stelem_R4;
                case TypeCode.Double:       //double
                    return OpCodes.Stelem_R8;
                case TypeCode.String:       //string
                    return OpCodes.Stelem_Ref;
                default:
                    return OpCodes.Nop;
            }
        }

        //dictionary linking Ldc opcodes to integers
        static System.Collections.Generic.IDictionary<OpCode, int> OpCodeDict = new System.Collections.Generic.Dictionary<OpCode, int>
        {
            {OpCodes.Ldc_I4_1, 0},
            {OpCodes.Ldc_I4_2, 1},
            {OpCodes.Ldc_I4_3, 2},
            {OpCodes.Ldc_I4_4, 3},
            {OpCodes.Ldc_I4_5, 4},
            {OpCodes.Ldc_I4_6, 5},
            {OpCodes.Ldc_I4_7, 6},
            {OpCodes.Ldc_I4_8, 7},
            {OpCodes.Ldc_I4_0, 8},
            {OpCodes.Ldc_I4, 9},
            {OpCodes.Ldc_I8, 10},
            {OpCodes.Ldc_R4, 11},
            {OpCodes.Ldc_R8, 12},
            {OpCodes.Ldc_I4_M1, 13},
            {OpCodes.Ldc_I4_S, 14}
        };

        public static bool IsLdc(this Instruction instruction)
        {
            return (OpCodeDict[instruction.OpCode]) switch
            {
                0 => true,      //OpCodes.Ldc_I4_1
                1 => true,      //OpCodes.Ldc_I4_2
                2 => true,      //OpCodes.Ldc_I4_3
                3 => true,      //OpCodes.Ldc_I4_4
                4 => true,      //OpCodes.Ldc_I4_5
                5 => true,      //OpCodes.Ldc_I4_6
                6 => true,      //OpCodes.Ldc_I4_7
                7 => true,      //OpCodes.Ldc_I4_8
                8 => true,      //OpCodes.Ldc_I4_0
                9 => true,      //OpCodes.Ldc_I4
                10 => true,     //OpCodes.Ldc_I8
                11 => true,     //OpCodes.Ldc_R4
                12 => true,     //OpCodes.Ldc_R8
                13 => true,     //OpCodes.Ldc_I4_M1
                14 => true,     //OpCodes.Ldc_I4_S
                _ => false
            };
        }
    }
}