using System;
using System.Linq;
using Mono.Cecil.Cil;

namespace Faultify.Tests.UnitTests.Utils
{
    public static class StringExtensions
    {
        public static string FirstCharToUpper(this string input)
        {
            return input switch
            {
                null => throw new ArgumentNullException(nameof(input)),
                "" => throw new ArgumentException($"{nameof(input)} cannot be empty", nameof(input)),
                _ => input.First().ToString().ToUpper() + input.Substring(1)
            };
        }

        public static OpCode ParseOpCode(this string opCode)
        {
            return (OpCode) typeof(OpCodes).GetField(opCode).GetValue(null);
        }

        public static string CleanUpCode(this string input)
        {
            return input.Replace("\t", "").Replace("\n", "").Replace("\r", "").Replace(" ", "");
        }
    }
}