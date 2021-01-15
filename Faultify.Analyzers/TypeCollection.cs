using System;
using System.Collections.Generic;

namespace Faultify.Analyzers
{
    /// <summary>
    ///     Collection of Types.
    /// </summary>
    public class TypeCollection
    {
        private static readonly List<Type> _integerTypes = new List<Type>
        {
            typeof(float), typeof(double),
            typeof(byte), typeof(short), typeof(int), typeof(long),
            typeof(sbyte), typeof(ushort), typeof(uint), typeof(ulong)
        };

        private static readonly List<Type> _stringTypes = new List<Type>
        {
            typeof(string), typeof(char)
        };

        private static readonly List<Type> _booleanTypes = new List<Type>
        {
            typeof(bool)
        };

        /// <summary>
        ///     The collection of types.
        /// </summary>
        public HashSet<Type> Types;

        public TypeCollection()
        {
            Types = new HashSet<Type>();
        }

        /// <summary>
        ///     Adds boolean type to the storage.
        /// </summary>
        public void AddBooleanTypes()
        {
            foreach (var type in BooleanTypes()) Types.Add(type);
        }

        /// <summary>
        ///     Adds all integer types to the storage.
        /// </summary>
        public void AddIntegerTypes()
        {
            foreach (var type in IntegerTypes()) Types.Add(type);
        }

        /// <summary>
        ///     Adds string and char types to the storage.
        /// </summary>
        public void WithStringTypes()
        {
            foreach (var type in StringTypes()) Types.Add(type);
        }

        public static List<Type> IntegerTypes()
        {
            return _integerTypes;
        }

        public static List<Type> StringTypes()
        {
            return _stringTypes;
        }

        public static List<Type> BooleanTypes()
        {
            return _booleanTypes;
        }
    }
}