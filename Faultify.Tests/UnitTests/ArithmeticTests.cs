using System.IO;
using Faultify.Analyze.Analyzers.Analyzers;
using Faultify.Tests.UnitTests.Utils;
using Mono.Cecil.Cil;
using NUnit.Framework;

namespace Faultify.Tests.UnitTests
{
    internal class ArithmeticTests
    {
        private readonly string _folder = Path.Combine("UnitTests", "TestSource", "ArithmeticTarget.cs");
        private readonly string _nameSpace = "Faultify.Tests.UnitTests.TestSource.ArithmeticTarget";

        [TestCase("Addition", 3, 1, 4)]
        [TestCase("Substraction", 10, 2, 8)]
        [TestCase("Multiplication", 10, 5, 50)]
        [TestCase("Division", 21, 7, 3)]
        [TestCase("Modulo", 22, 7, 1)]
        public void Arithmetic_PreMutation(string methodName, object argument1, object argument2, int expected)
        {
            // Arrange
            var binary = DllTestHelper.CompileTestBinary(_folder);

            // Act
            using (var binaryInteractor = new DllTestHelper(binary))
            {
                var actual = (int) binaryInteractor.DynamicMethodCall(_nameSpace, methodName.FirstCharToUpper(),
                    new[] {argument1, argument2});

                // Assert
                Assert.AreEqual(expected, actual);
            }
        }

        [TestCase("Addition", 3, 1, nameof(OpCodes.Sub), 2)]
        [TestCase("Addition", 10, 2, nameof(OpCodes.Div), 5)]
        [TestCase("Addition", 10, 5, nameof(OpCodes.Mul), 50)]
        [TestCase("Addition", 22, 7, nameof(OpCodes.Rem), 1)]
        [TestCase("Substraction", 3, 1, nameof(OpCodes.Add), 4)]
        [TestCase("Substraction", 10, 2, nameof(OpCodes.Div), 5)]
        [TestCase("Substraction", 10, 5, nameof(OpCodes.Mul), 50)]
        [TestCase("Substraction", 22, 7, nameof(OpCodes.Rem), 1)]
        [TestCase("Multiplication", 3, 1, nameof(OpCodes.Add), 4)]
        [TestCase("Multiplication", 10, 2, nameof(OpCodes.Div), 5)]
        [TestCase("Multiplication", 11, 5, nameof(OpCodes.Sub), 6)]
        [TestCase("Multiplication", 22, 7, nameof(OpCodes.Rem), 1)]
        [TestCase("Division", 3, 1, nameof(OpCodes.Add), 4)]
        [TestCase("Division", 10, 2, nameof(OpCodes.Mul), 20)]
        [TestCase("Division", 11, 5, nameof(OpCodes.Sub), 6)]
        [TestCase("Division", 22, 7, nameof(OpCodes.Rem), 1)]
        [TestCase("Modulo", 3, 1, nameof(OpCodes.Add), 4)]
        [TestCase("Modulo", 10, 2, nameof(OpCodes.Mul), 20)]
        [TestCase("Modulo", 11, 5, nameof(OpCodes.Sub), 6)]
        [TestCase("Modulo", 22, 7, nameof(OpCodes.Div), 3)]
        public void Arithmetic_PostMutation(string methodName, object argument1, object argument2,
            string expectedOpCodeName, int expected)
        {
            // Arrange
            var binary = DllTestHelper.CompileTestBinary(_folder);
            var opCodeExpected = expectedOpCodeName.ParseOpCode();

            // Act
            var mutatedBinary =
                DllTestHelper.MutateMethod<ArithmeticAnalyzer>(binary, methodName, opCodeExpected);
            using (var binaryInteractor = new DllTestHelper(mutatedBinary))
            {
                var actual = (int) binaryInteractor.DynamicMethodCall(_nameSpace, methodName.FirstCharToUpper(),
                    new[] {argument1, argument2});

                // Assert
                Assert.AreEqual(expected, actual);
            }
        }
    }
}