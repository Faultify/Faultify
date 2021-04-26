using System;
using System.Linq;
using Faultify.Core.Extensions;
using NLog;

namespace Faultify.Analyze
{
    /// <summary>
    ///     Random value generator that can be used for generating random values for IL-instructions.
    /// </summary>
    public class RandomValueGenerator
    {
        private readonly Random _rng;
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

        public RandomValueGenerator()
        {
            _rng = new Random();
        }

        /// <summary>
        ///     Generates a random value for the given field type.
        /// </summary>
        /// <param name="type">the type of the field for which a random value is to be generated</param>
        /// <param name="reference">a il-reference to the field that contains the originalField value</param>
        /// <returns>The random value.</returns>
        public object GenerateValueForField(Type type, object reference)
        {
            object newRef = null;

            try
            {
                if (type == typeof(bool))
                {
                    newRef = ChangeBoolean(reference);
                    _logger.Trace($"Changing boolean value from {reference} to {newRef}");
                }
                else if (type == typeof(string))
                {
                    newRef = ChangeString();
                    _logger.Trace($"Changing string value from {reference} to {newRef}");
                }
                else if (type == typeof(char))
                {
                    newRef = ChangeChar(reference);
                    _logger.Trace($"Changing char value from {reference} to {newRef}");
                }
                else if (type.IsNumericType())
                {
                    newRef = ChangeNumber(type, reference);
                    _logger.Trace($"Changing numeric value from {reference} to {newRef}");
                }
                else
                {
                    _logger.Warn($"Type {type} is not supported");
                }
            }
            catch (Exception e)
            {
                _logger.Warn(e, "There was probably an error casting a value, defaulting to null");
            }

            return newRef;
        }

        /// <summary>
        ///     Returns a flipped version of the passed boolean.
        /// </summary>
        /// <param name="original"></param>
        /// <returns></returns>
        private object ChangeBoolean(object reference)
        {
            bool value = Convert.ToBoolean(reference);
            return Convert.ToInt32(!value);
        }

        /// <summary>
        ///     Generates a new random string.
        /// </summary>
        /// <returns>The random string</returns>
        private object ChangeString()
        {
            var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            var stringChars = new char[32];

            for (int i = 0; i < stringChars.Length; i++)
            {
                stringChars[i] = chars[_rng.Next(chars.Length)];
            }

            return new string(stringChars);
        }

        /// <summary>
        ///     Generates a new random character.
        /// </summary>
        /// <param name="originalRef">Reference to the orginial char</param>
        /// <returns>The random character</returns>
        private object ChangeChar(object originalRef)
        {
            var original = Convert.ToChar(originalRef);
            var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            var generated = chars[_rng.Next(chars.Length)];

            return (original == generated) ? ChangeChar(original) : generated;
        }



        /// <summary>
        /// Generates a new value for the given number reference
        /// </summary>
        /// <param name="originalType">Type of the number</param>
        /// <param name="original">Original number</param>
        /// <returns>The random numeric object</returns>
        private object ChangeNumber(Type originalType, object original)
        {
            Type type = TypeChecker.NumericTypes.First(type => type == originalType);
            object generated = Convert.ChangeType(_rng.Next(), type);

            return (original == generated) ? ChangeNumber(originalType, original) : generated;
        }
    }
}