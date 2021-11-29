using System;
using System.Collections.Generic;
using System.Linq;
using Faultify.Core.Extensions;
using ICSharpCode.Decompiler.CSharp.Syntax;
using Microsoft.VisualBasic.FileIO;

namespace Faultify.Analyze
{
    /// <summary>
    ///     Random value generator that can be used for generating random values for IL-instructions.
    /// </summary>
    public class RandomValueGenerator
    {
        private readonly Random _rng;

        public RandomValueGenerator()
        {
            _rng = new Random(base.GetHashCode());
        }

        /// <summary>
        ///     Generates a random value for the given field type.
        /// </summary>
        /// <param name="fieldType">the type of the field for which a random value is to be generated</param>
        /// <param name="fieldReference">a il-reference to the field that contains the originalField value</param>
        /// <returns>The random value.</returns>
        public object GenerateValueForField(Type fieldType, object fieldReference)
        {
            if (fieldType == typeof(bool))
                return Convert.ToInt32(FlipBool(Convert.ToBoolean(fieldReference)));
            if (fieldType == typeof(string))
                return GenerateString();
            if (fieldType == typeof(char))
                return GenerateChar(fieldReference);
            if (fieldType.IsNumericType())
                return GenerateNumber(fieldType, fieldReference);

            return null;
        }

        /// <summary>
        ///     Returns a flipped version of the passed boolean.
        /// </summary>
        /// <param name="original"></param>
        /// <returns></returns>
        public bool FlipBool(bool original)
        {
            return !original;
        }

        /// <summary>
        ///     Returns a random string.
        /// </summary>
        /// <returns></returns>
        public string GenerateString()
        {
            return Guid.NewGuid().ToString();
        }

        /// <summary>
        ///     Generates a new random character.
        /// </summary>
        /// <param name="originalRef">Reference to the orginial char</param>
        /// <returns>The random character</returns>
        private object GenerateChar(object originalRef)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            var original = Convert.ToChar(originalRef);

            while (true)
            {
                var generatedChar = chars[_rng.Next(chars.Length)];

                // if the original equals the generated char, try again
                if (original == generatedChar) continue;

                // return the int value of the char
                return (int)char.GetNumericValue(generatedChar);
            }
        }

        /// <summary>
        ///     Returns a random number for the given originalField field.
        /// </summary>
        /// <param name="fieldType"></param>
        /// <param name="originalField"></param>
        /// <returns></returns>
        public object GenerateNumber(Type fieldType, object originalField)
        {
            while (true)
            {
                object newNumber = null;

                // Generate a new number based on the fieldType
                // If the type has limits higher or lower then that of Int32, just use the limits of Int32
                // This is done because _rng.Next only takes arguments of Int32
                if (fieldType == typeof(double))
                {
                    newNumber = _rng.NextDouble();
                }
                else if (fieldType == typeof(float))
                {
                    newNumber = (float) _rng.NextDouble();
                }
                else if (fieldType == typeof(sbyte))
                {
                    newNumber = _rng.Next(sbyte.MinValue, sbyte.MaxValue);
                }
                else if (fieldType == typeof(byte))
                {
                    newNumber = _rng.Next(byte.MinValue, byte.MaxValue);
                }
                else if (fieldType == typeof(ushort))
                {
                    newNumber = _rng.Next(ushort.MinValue, ushort.MaxValue);
                }
                else if (fieldType == typeof(short))
                {
                    newNumber = _rng.Next(short.MinValue, short.MaxValue);
                }
                else if (fieldType == typeof(int) || fieldType == typeof(long) || fieldType == typeof(nint))
                {
                    newNumber = _rng.Next(int.MinValue, int.MaxValue);
                }
                else if (fieldType == typeof(uint) || fieldType == typeof(ulong) || fieldType == typeof(nuint))
                {
                    newNumber = _rng.Next(0, int.MaxValue);
                }

                // if the generated number equals the orginal, try again
                if (originalField == newNumber) continue;
                return newNumber;
            }
        }
    }
}