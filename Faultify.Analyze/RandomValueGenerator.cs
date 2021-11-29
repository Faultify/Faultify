using System;
using System.Collections.Generic;
using System.Linq;
using Faultify.Core.Extensions;
using ICSharpCode.Decompiler.CSharp.Syntax;

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
            var original = Convert.ToChar(originalRef);
            var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            var generated = chars[_rng.Next(chars.Length)];

            return original == generated ? GenerateChar(original) : generated;
        }

        /// <summary>
        ///     Returns a random number for the given originalField field.
        /// </summary>
        /// <param name="originalField"></param>
        /// <returns></returns>
        public object GenerateNumber(Type fieldType, object originalField)
        {
            var type = TypeChecker.NumericTypes.First(type => type == fieldType);

            // Convert.ChangeType throws an exception if changing the type will result in information loss
            // therefore, we need to make it impossible to generate a number that goes outside of
            // the range of the target type.
            object generatedObj;

            if (fieldType == typeof(double))
                generated = _rng.NextDouble();
            if (fieldType == typeof(float))
                generated = (float)_rng.NextDouble();

            (int min, int max) = TypeLimits[fieldType];
            generatedObj = Convert.ChangeType(_rng.Next(min, max), type);
            int generated = Convert.ToInt32(generatedObj);

            return originalField == generatedObj ? GenerateNumber(fieldType, originalField) : generated;
        }

        /// <summary>
        ///     Contains the limits for all numeric types, as integers.
        ///     If a type has a higher limit than int.MaxValue, we simply use int.Maxvalue.
        /// </summary>
        private static Dictionary<Type, (int, int)> TypeLimits = new Dictionary<Type, (int, int)>()
        {
            {typeof(sbyte),  (sbyte.MinValue, sbyte.MaxValue)},
            {typeof(byte),   (byte.MinValue, byte.MaxValue)},
            {typeof(short),  (short.MinValue, short.MaxValue)},
            {typeof(ushort), (ushort.MinValue, ushort.MaxValue)},
            {typeof(int),    (int.MinValue, int.MaxValue)},
            {typeof(long),   (int.MinValue, int.MaxValue)},
            {typeof(nint),   (int.MinValue, int.MaxValue)},
            {typeof(uint),   (0, int.MaxValue)},
            {typeof(ulong),  (0, int.MaxValue)},
            {typeof(nuint),  (0, int.MaxValue)}
        };
    }
}