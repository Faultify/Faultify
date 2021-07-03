using System;
using System.Collections.Generic;

namespace Faultify.Analyze
{
    /// <summary>
    ///     Collection of Types.
    /// </summary>
    public class TypeChecker
    {
        public static readonly ICollection<Type> NumericTypes = new HashSet<Type>
        {
            typeof(float), typeof(double),
            typeof(sbyte), typeof(byte),
            typeof(short), typeof(ushort),
            typeof(int), typeof(uint),
            typeof(long), typeof(ulong),
        };

        // TODO: Why is this never used?
        public static readonly ICollection<Type> stringTypes = new HashSet<Type>
        {
            typeof(string), typeof(char),
        };

        /// <summary>
        ///     Specifies wether or not the given type is valid for an array target
        /// </summary>
        /// <param name="t">Type to be checked</param>
        /// <returns>True if a valid array type, false otherwise</returns>
        public static bool IsArrayType(Type t)
        {
            ISet<Type> arrayTypes = new HashSet<Type>();
            arrayTypes.UnionWith(NumericTypes);
            arrayTypes.Add(typeof(bool));

            return arrayTypes.Contains(t);
        }


        /// <summary>
        ///     Specifies wether or not the given type is valid for a variable target
        /// </summary>
        /// <param name="t">Type to be checked</param>
        /// <returns>True if a valid variable type, false otherwise</returns>
        public static bool IsVariableType(Type t)
        {
            ISet<Type> arrayTypes = new HashSet<Type>();
            // TODO: May cause problems
            arrayTypes.UnionWith(NumericTypes);
            arrayTypes.Add(typeof(bool));

            return arrayTypes.Contains(t);
        }


        /// <summary>
        ///     Specifies wether or not the given type is valid for a constant target
        /// </summary>
        /// <param name="t">Type to be checked</param>
        /// <returns>True if a valid constant type, false otherwise</returns>
        public static bool IsConstantType(Type t)
        {
            ISet<Type> arrayTypes = new HashSet<Type>();
            arrayTypes.UnionWith(NumericTypes);
            arrayTypes.Add(typeof(bool));
            arrayTypes.Add(typeof(string));

            return arrayTypes.Contains(t);
        }
    }
}
