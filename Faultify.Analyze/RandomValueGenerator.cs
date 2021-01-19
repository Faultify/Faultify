using System;
using Faultify.Core.Extensions;

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
            if (fieldType.IsNumericType()) return GenerateNumberFor(fieldType, fieldReference);

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
        ///     Returns a random number for the given originalField field.
        /// </summary>
        /// <param name="originalField"></param>
        /// <returns></returns>
        public object GenerateNumberFor(Type fieldType, object originalField)
        {
            object generated = 0;

            if (fieldType == typeof(sbyte))
                generated = _rng.Next(sbyte.MinValue, sbyte.MaxValue);
            if (fieldType == typeof(short))
                generated = _rng.Next(short.MinValue, short.MaxValue);
            if (fieldType == typeof(int) || fieldType == typeof(long))
                generated = _rng.Next(int.MinValue, int.MaxValue);

            if (fieldType == typeof(byte))
                generated = _rng.Next(byte.MinValue, byte.MaxValue);
            if (fieldType == typeof(ushort))
                generated = _rng.Next(ushort.MinValue, ushort.MaxValue);

            if (fieldType == typeof(uint) || fieldType == typeof(ulong))
                generated = _rng.Next((int) uint.MinValue, int.MaxValue);
            if (fieldType == typeof(double))
                generated = _rng.NextDouble();
            if (fieldType == typeof(float))
                generated = (float) _rng.NextDouble();

            if (fieldType == typeof(char))
                generated = _rng.Next(char.MinValue, char.MaxValue);


            // Recursive generate until generated is not equal to the original.
            if (generated == originalField)
                return GenerateNumberFor(fieldType, originalField);

            return generated;
        }
    }
}