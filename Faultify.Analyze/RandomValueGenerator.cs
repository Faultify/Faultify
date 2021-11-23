using System;
using System.Linq;
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
            var generated = Convert.ChangeType(_rng.Next(), type);

            return fieldType == generated ? GenerateNumber(fieldType, originalField) : generated;
        }
    }
}