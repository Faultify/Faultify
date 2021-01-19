using System.Collections.Generic;
using Mono.Cecil;
using Mono.Cecil.Cil;

namespace Faultify.Tests.UnitTests.Utils
{
    /// <summary>
    ///     Generator for a conditional branch statement.
    ///     This generator will only work for the following method structure:
    ///     * Returns true if condition is met
    ///     * Returns false if condition is not met
    ///     * Two int parameters.
    ///     * One if check with the condition.
    ///     ```
    ///     public bool ConditionCheck(int lhs, int rhs)
    ///     {
    ///     if (lhs (ANY CONDITION) rhs)
    ///     {
    ///     return true;
    ///     }
    ///     return false;
    ///     }
    ///     ```
    ///     In some cases the compiler can generate 'comparison' like clt/cgt instead of the branching variants blt/bgt.
    ///     This generator is able to override a method body were an comparison is occurring with the branching variant.
    /// </summary>
    internal class ConditionalBranchGenerator
    {
        private readonly List<Instruction> _instructions;

        /// <summary>
        ///     Generates a new instance by passing in the branching operator that will be used in the generation process.
        ///     Accepts 'only' a branching operators like bgt/blt/beq etc..
        /// </summary>
        /// <param name="comparison"></param>
        public ConditionalBranchGenerator(OpCode comparison)
        {
            var ia = Instruction.Create(OpCodes.Ldloc_1);
            var ib = Instruction.Create(OpCodes.Ret);
            var i7 = Instruction.Create(OpCodes.Ldc_I4_1);

            // load the two parameter booleans
            var i1 = Instruction.Create(OpCodes.Ldarg_1);
            var i2 = Instruction.Create(OpCodes.Ldarg_2);

            // if comparison true branch to i7.
            var i3 = Instruction.Create(comparison, i7);

            // if comparison false load '0' (false) and branch to return. 
            var i4 = Instruction.Create(OpCodes.Ldc_I4_0);
            var i5 = Instruction.Create(OpCodes.Stloc_1);
            var i6 = Instruction.Create(OpCodes.Br_S, ia);

            var i8 = Instruction.Create(OpCodes.Stloc_1);
            var i9 = Instruction.Create(OpCodes.Br_S, ia);

            i1.Next = i2;
            i2.Next = i3;
            i3.Next = i4;
            i4.Next = i5;
            i5.Next = i6;
            i6.Next = i7;
            i7.Next = i8;
            i8.Next = i9;
            i9.Next = ia;
            ia.Next = ib;

            i2.Previous = i1;
            i3.Previous = i2;
            i4.Previous = i3;
            i5.Previous = i4;
            i6.Previous = i5;
            i7.Previous = i6;
            i8.Previous = i7;
            i9.Previous = i8;
            ia.Previous = i9;
            ib.Previous = ia;

            i1.Offset = 0;
            i2.Offset = 1;
            i3.Offset = 2;
            i4.Offset = 3;
            i5.Offset = 4;
            i6.Offset = 5;
            i7.Offset = 6;
            i8.Offset = 7;
            i9.Offset = 8;
            ia.Offset = 9;
            ib.Offset = 10;

            _instructions = new List<Instruction>
            {
                i1, i2, i3, i4, i5, i6, i7, i8, i9, ia, ib
            };
        }

        /// <summary>
        ///     Replace a method body were a comparison is occurring with the branching variant.
        ///     Note that the method definition must ad-hear to strict rules noted in the class-level comments.
        /// </summary>
        /// <param name="method"></param>
        public void ReplaceMethodInstructions(MethodDefinition method)
        {
            method.Body.Instructions.Clear();

            foreach (var instruction in _instructions)
                method.Body.Instructions.Add(instruction);
        }
    }
}