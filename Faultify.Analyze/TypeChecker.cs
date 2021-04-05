using System;
using System.Collections;
using System.Collections.Generic;

namespace Faultify.Analyze
{
    /// <summary>
    ///     Collection of Types.
    /// </summary>
    public class TypeChecker
    {

        private static readonly ISet<Type> _integerTypes = new HashSet<Type>
        {
            typeof(float), typeof(double),
            typeof(byte), typeof(short), typeof(int), typeof(long),
            typeof(sbyte), typeof(ushort), typeof(uint), typeof(ulong)
        };

        // TODO: Why is this never used?
        private static readonly ISet<Type> _stringTypes = new HashSet<Type>
        {
            typeof(string), typeof(char)
        };

        /// <summary>
        /// Specifies wether or not the given type is valid for an array target
        /// </summary>
        /// <param name="t">Type to be checked</param>
        /// <returns>True if a valid array type, false otherwise</returns>
        public static bool IsArrayType(Type t)
        {
            ISet<Type> arrayTypes = new HashSet<Type>();
            arrayTypes.UnionWith(_integerTypes);
            arrayTypes.Add(typeof(bool));
            arrayTypes.Add(typeof(char));

            return arrayTypes.Contains(t);
        }


        /// <summary>
        /// Specifies wether or not the given type is valid for a variable target
        /// </summary>
        /// <param name="t">Type to be checked</param>
        /// <returns>True if a valid variable type, false otherwise</returns>
        public static bool IsVariableType(Type t)
        {
            // TODO: Why just bools?
            return t == typeof(bool);
        }


        /// <summary>
        /// Specifies wether or not the given type is valid for a constant target
        /// </summary>
        /// <param name="t">Type to be checked</param>
        /// <returns>True if a valid constant type, false otherwise</returns>
        public static bool IsConstantType(Type t)
        {
            return _integerTypes.Contains(t);
        }
    }
}